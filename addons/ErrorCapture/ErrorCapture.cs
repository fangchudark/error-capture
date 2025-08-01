using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Godot;

namespace ErrorCapture;

public enum ScriptSource
{
    Unknown,
    Cpp,
    GDScript,
    CSharp,
}

public partial class ErrorCapture : CanvasLayer
{
    public static ErrorCapture Instance { get; private set; }
    /// <summary>
    /// 捕获到错误时触发
    /// </summary>
    /// <param name="message"></param>
    [Signal]
    public delegate void ErrorCaughtEventHandler(ErrorData error);

    /// <summary>
    /// 错误信息的显示颜色
    /// </summary>
    [Export]
    public Color ErrorColor { get; set; } = Colors.Red;

    /// <summary>
    /// 每帧最大的处理行数
    /// </summary>
    [Export]
    public ulong MaxLinePerFrame { get; set; } = 500;

    // 从项目设置中获取日志文件路径
    private string _logPath = ProjectSettings.GetSetting("debug/file_logging/log_path", "user://logs/godot.log").AsString();

    // 用于显示日志消息
    private RichTextLabel _messageOut;
    // 日志文件的文件流对象
    private FileAccess _logFileStream;

    // 记录上次修改时间，用于检测是否有新消息被推送到日志文件
    private ulong _lastModifiedTime;

    private bool _inError = false;
    private string _summary = string.Empty;
    private readonly StringBuilder _raw = new();
    private readonly List<string> _stackTrace = [];
    private ScriptSource _source = ScriptSource.Unknown;
    private ulong linesRead;
        
    public override void _Ready()
    {
        // 日志文件不存在
        if (!FileAccess.FileExists(_logPath))
            return;

        _messageOut = GetNodeOrNull<RichTextLabel>("%out");
        // 引用节点失败
        if (_messageOut == null)
            return;

        _logFileStream = FileAccess.Open(_logPath, FileAccess.ModeFlags.Read);
        // 创建文件流失败
        if (_logFileStream == null)
            return;

        // 获取文件最后修改时间
        _lastModifiedTime = FileAccess.GetModifiedTime(_logPath);
        Instance = this;
        _messageOut.Clear();
        Visible = false;
        _messageOut.GuiInput += OnOutputGUIInput;
    }

    public override void _Process(double delta)
    {
        // 创建文件流失败
        if (_logFileStream == null)
            return;

        ulong currentModTime = FileAccess.GetModifiedTime(_logPath);
        // 文件未被修改，即没有新的消息被推送到日志文件
        if (currentModTime < _lastModifiedTime)
            return;


        // 逐行读取并追加到RichTextLabel控件中
        while (_logFileStream.GetPosition() < _logFileStream.GetLength())
        {
            var message = _logFileStream.GetLine();
            linesRead++;


            if (_inError) // 在错误信息块中
            {
                // 调用栈3个空格 + at
                // 当前行是调用栈
                if (MatchStackTrace().IsMatch(message))
                {                    
                    _source = GetScriptSource(message);

                    _stackTrace.Add(message);
                    _raw.AppendLine(message);
                }
                else // 非调用栈，即结束错误信息块
                {
                    ShowErrorMessage();

                    RestErrorBlock();
                }
            }

            // 捕获到错误信息，刚好在ERROR/SCRIPT ERROR这一行
            if ((message.StartsWith("ERROR:") || message.StartsWith("SCRIPT ERROR:")) && !_inError)
            {
                _inError = true;
                _summary = message.Substring(message.IndexOf("ERROR:") + 6);
                _raw.AppendLine(message);
            }

            if (linesRead >= MaxLinePerFrame)
            {                
                linesRead = 0;
                break;
            }
        }

        bool finishedReading = _logFileStream.GetPosition() >= _logFileStream.GetLength();
        if (finishedReading)
        {
            // 如果读取完毕时仍处于错误块
            if (_inError)
            {
                ShowErrorMessage();

                RestErrorBlock();
            }
            _lastModifiedTime = currentModTime;
        }
    }

    private ScriptSource GetScriptSource(string message)
    {
        if (message.Contains(".cpp:"))
            return ScriptSource.Cpp;
        if (message.Contains(".cs:"))
            return ScriptSource.CSharp;
        if (message.Contains(".gd:"))
            return ScriptSource.GDScript;

        return ScriptSource.Unknown;
    }

    private void RestErrorBlock()
    {
        _inError = false;
        _raw.Clear();
        _summary = string.Empty;
        _stackTrace.Clear();
    }

    private void ShowErrorMessage()
    {
        var raw = _raw.ToString();
        EmitSignalErrorCaught(
            new(
                _source,
                raw,
                _summary,
                _stackTrace.ToArray()
            )
        );
        Visible = true;
        _messageOut.Clear();
        _messageOut.PushColor(ErrorColor);
        if (_stackTrace.Count == 0)
            _messageOut.AppendText("This message may not be an actual script error. It could be user output or a system log without a stack trace.\n");
        _messageOut.AppendText(raw);
    }

    private void OnOutputGUIInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
            {
                Visible = false;
            }
        }
    }

    // 退出树时释放资源
    public override void _ExitTree()
    {
        _logFileStream?.Dispose();
    }

    /// <summary>
    /// 匹配以 一个或多个空格 + 'at' 开头的字符串 
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"^\s+at")]
    private static partial Regex MatchStackTrace();

}

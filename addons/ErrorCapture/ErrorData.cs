using System.Text;
using Godot;

namespace ErrorCapture;

[GlobalClass]
public partial class ErrorData : RefCounted
{

    [Export] public ScriptSource Source { get; private set; }
    [Export] public string RawText { get; private set; }
    [Export] public string Summary { get; private set; }
    [Export] public string[] StackTrace { get; private set; }
    [Export] public bool IsLikelyRealError { get; private set; }

    public ErrorData(ScriptSource source, string rawText, string summary, string[] stackTrace)
    {
        Source = source;
        RawText = rawText;
        Summary = summary;
        StackTrace = stackTrace;
        IsLikelyRealError = stackTrace.Length > 0;
    }

    public override string ToString()
    {
        StringBuilder stack = new();
        foreach (var call in StackTrace)
        {
            stack.AppendLine(call);
        }

        string warning = IsLikelyRealError
            ? string.Empty
            : "\nThis message may not be an actual script error. It could be user output or a system log without a stack trace.\n";

        return $@"A runtime error has occurred:
Script Source: {Source}
Summary: {Summary}
Stack Trace:
{stack}{warning}";
    }
}
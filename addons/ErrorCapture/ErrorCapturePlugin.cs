#if TOOLS
using Godot;

namespace ErrorCapture;

[Tool]
public partial class ErrorCapturePlugin : EditorPlugin
{
	public override void _EnterTree()
	{
		AddAutoloadSingleton("ErrorCapture", "ErrorCapture.tscn");		
		ProjectSettings.SetSetting("debug/file_logging/enable_file_logging", true);
		ProjectSettings.SetSetting("debug/file_logging/enable_file_logging.pc", true);
	}

	public override void _ExitTree()
	{
		RemoveAutoloadSingleton("ErrorCapture");
	}
}
#endif

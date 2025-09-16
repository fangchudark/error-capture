### **[简体中文](README_cn.md) | [English](README.MD)**

**In Godot4.5 or higher, it is recommended to use a custom Logger achieve this function, see [GodotLoggerSimple](https://github.com/fangchudark/godot-logger-simple)**
---
# Error Capture


A simple plugin that supports capturing error information at runtime and displaying it on the UI.

## Features

- Real-time capture of game runtime error logs
- Support for multiple scripting language error identification (C#、GDScript、C++)
- Automatic parsing of error stack trace information
- Visual error information display
- Provides error data structure for further processing

## Installation

1. Copy the `addons/ErrorCapture` folder to the `addons` directory in your Godot project
2. In the Godot editor, go to `Project -> Project Settings -> Plugins`
3. Find the "Error Capture" plugin and enable it

## Usage

After enabling the plugin, it will automatically start monitoring the log file for error information. When an error is detected:

1. The error information will be displayed on the visual interface on the screen
2. You can hide the error information interface by clicking on it
3. The plugin emits an `ErrorCaught` signal, which you can connect to handle the error data

Example code:
```csharp
public override void _Ready()
{
    ErrorCapture.Instance.ErrorCaught += (errorData) => {
        GD.Print(errorData.ToString());
    };
}
```
```gdscript
func _ready():
    ErrorCapture.ErrorCaught.connect(
        func(data):
            print(data)
    )

```

# API
## Signals
- `ErrorCaught(ErrorData error)`: Triggered when an error is caught  

## Properties
- `ErrorColor`: Color of error information display (default is red)
- `MaxLinePerFrame`: Maximum number of log lines processed per frame (default is 500)

## Classes

`ErrorData`

Data class containing detailed error information:

- `Source`: Error source (`C#`, `GDScript`, `C++` or `Unknown`)
- `RawText`: Raw error text
- `Summary`: Error summary
- `StackTrace`: Array of stack trace information
- `IsLikelyRealError`: Whether it is a real error (based on whether there is a stack trace)

# Requirements
- Godot 4.x
- .NET 6.0 or higher

# License

MIT License

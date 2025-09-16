### **[简体中文](README_cn.md) | [English](README.MD)**

**在Godot4.5或更高版本中，建议使用自定义Logger实现此功能，请参见[GodotLoggerSimple](https://github.com/fangchudark/godot-logger-simple)**
---
# Error Capture

一个简单的插件，支持在运行时捕获错误信息，并显示在UI上。

## 功能

- 实时捕获游戏运行时错误日志
- 支持多种脚本语言错误识别（C#、GDScript、C++）
- 自动解析错误堆栈信息
- 可视化错误信息显示
- 提供错误数据结构便于进一步处理

## 安装

1. 将 `addons/ErrorCapture` 文件夹复制到您的 Godot 项目中的 `addons` 目录下
2. 在 Godot 编辑器中，进入 `Project -> Project Settings -> Plugins`
3. 找到 "Error Capture" 插件并启用它

## 使用方法

启用插件后，它会自动开始监控日志文件中的错误信息。当检测到错误时：

1. 错误信息会显示在屏幕上的可视化界面中
2. 可以通过点击错误信息界面来隐藏它
3. 插件会发出 `ErrorCaught` 信号，您可以连接此信号来处理错误数据

示例代码：
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
## 信号
- `ErrorCaught(ErrorData error)`: 当捕获到错误时触发  

## 属性
- `ErrorColor`: 错误信息显示颜色（默认为红色）
- `MaxLinePerFrame`: 每帧最大处理日志行数（默认为500）

## 类

`ErrorData`

包含错误详细信息的数据类：

- `Source`: 错误来源（`C#`、`GDScript`、`C++` 或 `Unknown`）
- `RawText`: 原始错误文本
- `Summary`: 错误摘要
- `StackTrace`: 堆栈跟踪信息数组
- `IsLikelyRealError`: 是否为真实错误（基于是否有堆栈跟踪）

# 要求
- Godot 4.x
- .NET 6.0 或更高版本

# 许可证

MIT License

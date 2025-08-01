using System;
using Godot;

public partial class Test : Node
{
    public override void _Ready()
    {
        ErrorCapture.ErrorCapture.Instance.ErrorCaught += (msg) => GD.Print(msg);
        Foo();
    }

    public void Foo()
    {
        Boo();
    }

    public void Boo()
    {
        Bar();
    }

    public void Bar()
    {
        Baz();
    }

    public void Baz()
    {        
        throw new NullReferenceException();
    }
}
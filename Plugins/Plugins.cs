namespace Plugins;
using CommandLib;

[PluginLoad]
public class FirstPlugin : ICommand
{
    public void Execute()
    {
        Console.WriteLine("FirstPlugin");
    }
}


[PluginLoad("FirstPlugin")]
public class Inheritor2 : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Inheritor2");
    }
}

[PluginLoad]
public class SecondPlugin : ICommand
{
    public void Execute()
    {
        Console.WriteLine("SecondPlugin");
    }
}

[PluginLoad("FirstPlugin")]
public class Inheritor1 : ICommand
{
    public void Execute()
    {
        Console.WriteLine("Inheritor1");
    }
}
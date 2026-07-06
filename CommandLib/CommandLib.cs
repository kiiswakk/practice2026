﻿namespace CommandLib;
public interface ICommand
{
    void Execute();
}

[AttributeUsage(AttributeTargets.Class)]
public class PluginLoadAttribute : Attribute
{
    public string Depends {get;}
    public PluginLoadAttribute(string _depends = "")
    {
        Depends = _depends;
    }
}
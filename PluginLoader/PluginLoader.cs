using CommandLib;
using System.Reflection;

namespace PluginLoader;

class Program
{
    static List<Type> allPlugins = new();
    static List<Type> sortedPlugins = new();
    static HashSet<string> visited = new();
    static void SortPlugins(Type type)
    {
        if (visited.Contains(type.Name))
            return;

        visited.Add(type.Name);

        var attribute = type.GetCustomAttribute<PluginLoadAttribute>();

        if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Depends))
        {
            foreach (var plugin in allPlugins)
            {
                if (plugin.Name == attribute.Depends)
                {
                    SortPlugins(plugin);
                    break;
                }
            }
        }
        sortedPlugins.Add(type);
    }
    static void Main()
    {
        string dllPath = @"Plugins/bin/Debug/net10.0/Plugins.dll";
        Assembly assembly = Assembly.LoadFrom(dllPath);

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass)
                continue;

            if (!typeof(ICommand).IsAssignableFrom(type))
                continue;

            if (type.GetCustomAttribute<PluginLoadAttribute>() == null)
                continue;

            allPlugins.Add(type);
        }

        foreach (var type in allPlugins)
        {
            SortPlugins(type);
        }

        foreach (var type in sortedPlugins)
        {
            ICommand command = (ICommand)Activator.CreateInstance(type)!;
            command.Execute();
        }
    }
}
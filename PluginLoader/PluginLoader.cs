using CommandLib;
using System.Reflection;

namespace PluginLoader;

public class Program
{
    static List<Type> allPlugins = new();
    static List<Type> sortedPlugins = new();

    static HashSet<string> visiting = new();
    static HashSet<string> visited = new();

    static void SortPlugins(Type type)
    {
        if (visited.Contains(type.Name))
            return;

        if (visiting.Contains(type.Name))
            throw new InvalidOperationException($"Обнаружена циклическая зависимость: {type.Name}");

        visiting.Add(type.Name);

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

        visiting.Remove(type.Name);
        visited.Add(type.Name);
        sortedPlugins.Add(type);
    }

    public static List<Type> GetSortedPlugins(List<Type> plugins)
    {
        allPlugins = plugins;
        sortedPlugins.Clear();
        visiting.Clear();
        visited.Clear();

        foreach (var plugin in allPlugins)
            SortPlugins(plugin);

        return sortedPlugins;
    }
    
    static void Main()
    {
        string solutionDir = Path.GetFullPath(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", ".."));

        string pluginsDir = Path.Combine(solutionDir, "Plugins", "bin", "Debug", "net10.0");

        string[] dllFiles = Directory.GetFiles(pluginsDir, "*.dll");
        foreach (string dllPath in dllFiles)
        {
            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);

                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsClass || type.IsAbstract)
                        continue;

                    if (!typeof(ICommand).IsAssignableFrom(type))
                        continue;

                    if (type.GetCustomAttribute<PluginLoadAttribute>() == null)
                        continue;

                    allPlugins.Add(type);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось загрузить {Path.GetFileName(dllPath)}: {ex.Message}");
            }
        }

        try
        {
            foreach (var type in allPlugins)
            {
                SortPlugins(type);
            }

            Console.WriteLine($"Найдено плагинов: {sortedPlugins.Count}\n");

            foreach (var type in sortedPlugins)
            {
                if (Activator.CreateInstance(type) is ICommand command)
                {
                    command.Execute();
                }
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Ошибка: {ex.Message}");
            Console.ResetColor();
        }   
    }   
}

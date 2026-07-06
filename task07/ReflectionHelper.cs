namespace task07;
using System.Reflection;

public static class ReflectionHelper
{
    public static void PrintTypeInfo(Type type)
    {
        var displayName = type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayName != null)
        {
            Console.WriteLine($"Название класса: {displayName.DisplayName}");
        }

        var version = type.GetCustomAttribute<VersionAttribute>();
        if (version != null)
        {
            Console.WriteLine($"Версия: {version.Major}.{version.Minor}");
        }

        Console.WriteLine("Методы:");

        foreach (var method in type.GetMethods(
                     BindingFlags.Public |
                     BindingFlags.Instance |
                     BindingFlags.DeclaredOnly))
        {
            var attribute = method.GetCustomAttribute<DisplayNameAttribute>();

            if (attribute != null)
                Console.WriteLine($"{method.Name} - {attribute.DisplayName}");
            else
                Console.WriteLine(method.Name);
        }

        Console.WriteLine("Свойства:");

        foreach (var property in type.GetProperties())
        {
            var attribute = property.GetCustomAttribute<DisplayNameAttribute>();

            if (attribute != null)
                Console.WriteLine($"{property.Name} - {attribute.DisplayName}");
            else
                Console.WriteLine(property.Name);
        }
    }
}
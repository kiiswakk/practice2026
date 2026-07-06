using System;
using System.IO;
using System.Linq;
using System.Reflection;

if (args.Length == 0)
{
    Console.WriteLine("Укажите путь к dll файлу");
    return;
}

string dllPath = args[0];

if (!File.Exists(dllPath))
{
    Console.WriteLine("Файл не найден!!!");
    return;
}

try
{
    Assembly assembly = Assembly.LoadFrom(dllPath);
    Console.WriteLine($"Библиотека: {assembly.GetName().Name}");
    Console.WriteLine(new string('-', 40));
    Type[] types;
    try
    {
        types = assembly.GetTypes();
    }
    catch (ReflectionTypeLoadException ex)
    {
        Console.WriteLine("Не удалось загрузить некоторые типы из-за отсутствующих зависимостей.");
        
        Console.WriteLine("Ошибки загрузки:");
        foreach (var loaderException in ex.LoaderExceptions)
        {
            Console.WriteLine($" - {loaderException?.Message}");
        }
        Console.WriteLine(new string('-', 40));
        types = ex.Types.OfType<Type>().ToArray();
    }

    foreach (Type type in types)
    {
        Console.WriteLine($"Класс: {type.FullName}");

        // Атрибуты
        var attributes = type.GetCustomAttributes().ToArray();
        if (attributes.Length > 0)
        {
            Console.WriteLine("  Атрибуты:");
            foreach (var attribute in attributes)
            {
                Console.WriteLine($"    [{attribute.GetType().Name}]");
            }
        }
        var constructors = type.GetConstructors();
        if (constructors.Length > 0)
        {
            Console.WriteLine("  Конструкторы:");
            foreach (ConstructorInfo constructor in constructors)
            {
                var parameters = constructor.GetParameters()
                    .Select(p => $"{p.ParameterType.Name} {p.Name}");
                Console.WriteLine($"    ctor({string.Join(", ", parameters)})");
            }
        }
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        if (methods.Length > 0)
        {
            Console.WriteLine("  Методы:");
            foreach (MethodInfo method in methods)
            {
                var parameters = method.GetParameters()
                    .Select(p => $"{p.ParameterType.Name} {p.Name}");
                Console.WriteLine($"    {method.ReturnType.Name} {method.Name}({string.Join(", ", parameters)})");
            }
        }
        
        Console.WriteLine();
    }
}
catch (BadImageFormatException)
{
    Console.WriteLine("Ошибка: Файл не является валидной .NET сборкой.");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}
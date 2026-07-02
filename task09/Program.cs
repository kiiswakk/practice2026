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

Assembly assembly = Assembly.LoadFrom(dllPath);

Console.WriteLine($"Библиотека: {assembly.GetName().Name}");

foreach (Type type in assembly.GetTypes())
{
    Console.WriteLine($"Класс: {type.Name}");

    Console.WriteLine("Атрибуты:");
    foreach (var attribute in type.GetCustomAttributes())
    {
        Console.WriteLine($"{attribute.GetType().Name}");
    }

    Console.WriteLine("Конструкторы:");
    foreach (ConstructorInfo constructor in type.GetConstructors())
    {
        Console.WriteLine($"{constructor.Name}");

        foreach (ParameterInfo parameter in constructor.GetParameters())
        {
            Console.WriteLine($"{parameter.ParameterType.Name} {parameter.Name}");
        }
    }

    Console.WriteLine("Методы:");
    foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
    {
        Console.WriteLine($"{method.Name}");
        foreach (ParameterInfo parameter in method.GetParameters())
        {
            Console.WriteLine($"{parameter.ParameterType.Name} {parameter.Name}");
        }
    }
}
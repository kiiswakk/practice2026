using System;
using System.IO;
using System.Linq;
using System.Reflection;
using task07; 

namespace task09
{
    class Program
    {   
        static void PrintParameters(ParameterInfo[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                if (i < parameters.Length - 1) Console.Write(", ");
            }
        }

        static void Main(string[] args)
        {
            string dllPath;

            if (args.Length > 0)
            {
                dllPath = args[0];
            }
            else
            {
                dllPath = Assembly.GetExecutingAssembly().Location;
            }

            if (!File.Exists(dllPath))
            {
                Console.WriteLine($"Ошибка. Файл не найден по пути: {dllPath}");
                return;
            }

            try
            {
                Assembly assembly = Assembly.LoadFrom(dllPath);
                Console.WriteLine($"Анализ сборки: {assembly.GetName().Name}");
                Console.WriteLine(new string('-', 50));

                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("[Предупреждение] Не удалось загрузить некоторые типы из-за отсутствия внешних зависимостей.");
                    Console.WriteLine("Ошибки загрузчика:");
                    foreach (var loaderEx in ex.LoaderExceptions)
                    {
                        if (loaderEx != null) Console.WriteLine($" - {loaderEx.Message}");
                    }
                    Console.WriteLine(new string('-', 50));

                    types = ex.Types.OfType<Type>().ToArray();
                }

                foreach (Type type in types)
                {
                    if (!type.IsClass) continue;

                    if (type.Name.StartsWith("<>")) continue;

                    Console.WriteLine($"Класс: {type.FullName}");

                    var classAttributes = type.GetCustomAttributes();
                    foreach (var attr in classAttributes)
                    {
                        if (attr.GetType().Name.StartsWith("Nullable")) continue;

                        if (attr is task07.DisplayNameAttribute dna)
                        {
                            Console.WriteLine($"  Атрибут: [DisplayName(\"{dna.DisplayName}\")]");
                        }
                        else if (attr is task07.VersionAttribute va)
                        {
                            Console.WriteLine($"  Атрибут: [Version({va.Major}, {va.Minor})]");
                        }
                        else
                        {
                            Console.WriteLine($"  Атрибут: [{attr.GetType().Name}]");
                        }
                    }

                    Console.WriteLine("  Конструкторы:");
                    ConstructorInfo[] constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var ctor in constructors)
                    {
                        string visibility = ctor.IsPublic ? "public" : "private/protected";
                        Console.Write($"    - {visibility} {type.Name}(");
                        PrintParameters(ctor.GetParameters());
                        Console.WriteLine(")");
                    }

                    Console.WriteLine("  Методы:");
                    MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        if (method.IsSpecialName) continue;

                        string modifiers = method.IsPublic ? "public" : "private/internal";
                        if (method.IsStatic) modifiers += " static";

                        Console.Write($"    - {modifiers} {method.ReturnType.Name} {method.Name}(");
                        PrintParameters(method.GetParameters());
                        Console.WriteLine(")");

                        var methodAttrs = method.GetCustomAttributes();
                        foreach (var attr in methodAttrs)
                        {
                            if (attr is task07.DisplayNameAttribute dna)
                            {
                                Console.WriteLine($"      Атрибут метода: [DisplayName(\"{dna.DisplayName}\")]");
                            }
                        }
                    }
                    Console.WriteLine(new string('-', 50));
                }
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine("Ошибка: Указанный файл не является валидной .NET сборкой.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Непредвиденная ошибка при анализе: {ex.Message}");
            }
        }
    }
}
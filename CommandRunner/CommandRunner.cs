using System.Reflection;
using CommandLib;

var dllPath = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory,
    "FileSystemCommands.dll");

if (!File.Exists(dllPath))
{
    Console.WriteLine("Не удалось найти файл");
    return;
}

try
{
    Assembly assembly = Assembly.LoadFrom(dllPath);

    var commandTypes = assembly.GetTypes()
        .Where(type =>
            typeof(ICommand).IsAssignableFrom(type) &&
            !type.IsInterface &&
            !type.IsAbstract);

    foreach (var type in commandTypes)
    {
        try
        {
            ICommand? command = null;

            if (type.Name == "DirectorySizeCommand")
            {
                command = (ICommand?)Activator.CreateInstance(type, ".");
            }
            else if (type.Name == "FindFilesCommand")
            {
                command = (ICommand?)Activator.CreateInstance(type, ".", "*.cs");
            }

            if (command != null)
            {
                command.Execute();
            }            
        }
        catch (MissingMethodException)
        {
            Console.WriteLine($"[Ошибка] У типа {type.Name} не найден подходящий конструктор.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Ошибка] Не удалось инициализировать или выполнить команду {type.Name}: {ex.Message}");
        }
    }
}

catch (FileLoadException ex)
{
    Console.WriteLine($"[Критическая ошибка] Не удалось загрузить сборку: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"[Непредвиденная ошибка]: {ex.Message}");
}
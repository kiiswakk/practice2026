using System.Reflection;

var dllPath = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory,
    "FileSystemCommands.dll");

if (!File.Exists(dllPath))
{
    Console.WriteLine("Не удалось найти файл");
    return;
}

Assembly assembly = Assembly.LoadFrom(dllPath);

var commandTypes = assembly.GetTypes()
    .Where(type =>
        typeof(ICommand).IsAssignableFrom(type) &&
        !type.IsInterface &&
        !type.IsAbstract);

foreach (var type in commandTypes)
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

    command?.Execute();
}
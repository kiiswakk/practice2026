using CommandLib;
namespace FileSystemCommands;
public class DirectorySizeCommand: ICommand
{
    private readonly string _directory;
    public long Size {get; private set;}
    public DirectorySizeCommand(string directory)
    {
        _directory = directory;
    }  
    public void Execute()
    {
        Size = Directory
            .GetFiles(_directory, "*", SearchOption.AllDirectories)
            .Sum(file => new FileInfo(file).Length);
    }
}

public class FindFilesCommand : ICommand
{
    private readonly string _directory;
    private readonly string _mask;

    public string[] FoundFiles { get; private set; } = [];

    public FindFilesCommand(string directory, string mask)
    {
        _directory = directory;
        _mask = mask;
    }

    public void Execute()
    {
        FoundFiles = Directory.GetFiles(
            _directory,
            _mask,
            SearchOption.AllDirectories);
    }
}

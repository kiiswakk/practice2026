using System;

namespace task17;

public class TestCommand : ILongCommand
{
    private readonly int _id;
    private int _counter;

    public TestCommand(int id)
    {
        _id = id;
    }

    public bool IsCompleted => _counter >= 3;

    public void Execute()
    {
        Console.WriteLine($"Поток {_id} вызов {++_counter}");
    }
}
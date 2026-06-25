namespace task04;

public class Fighter: ISpaceship
{
    public int FirePower => 50;
    public int Speed => 100;
    public void MoveForward()
    {
        Console.WriteLine("Fighter полетел вперед");
    }
    public void Rotate(int angle)
    {
        Console.WriteLine($"Fighter повернул на {angle} градусов");
    }
    public void Fire()
    {
        Console.WriteLine("Fighter выстрелил ракетой");
    }
}
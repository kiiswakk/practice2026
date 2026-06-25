namespace task04;

public class Cruiser: ISpaceship
{
    public int FirePower => 100;
    public int Speed => 50;
    public void MoveForward()
    {
        Console.WriteLine("Cruiser поплыл вперед");
    }
    public void Rotate(int angle)
    {
        Console.WriteLine($"Cruiser повернул на {angle} градусов");
    }
    public void Fire()
    {
        Console.WriteLine("Cruiser выстрелил ракетой");
    }
}
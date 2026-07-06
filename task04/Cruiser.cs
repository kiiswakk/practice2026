namespace task04;

public class Cruiser: ISpaceship
{
    public int FirePower => 100;
    public int Speed => 50;
    public void MoveForward()
    {
        OnActionExecuted?.Invoke("Cruiser поплыл вперед");
    }
    public void Rotate(int angle)
    {
        OnActionExecuted?.Invoke($"Cruiser повернул на {angle} градусов");
    }
    public void Fire()
    {
        OnActionExecuted?.Invoke("Cruiser выстрелил ракетой");
    }
}
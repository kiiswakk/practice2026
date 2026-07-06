namespace task04;

public class Fighter: ISpaceship
{
    public event Action<string>? OnActionExecuted; 
    public int FirePower => 50;
    public int Speed => 100;
    public void MoveForward()
    {
        OnActionExecuted?.Invoke("Fighter полетел вперед");
    }
    public void Rotate(int angle)
    {
        OnActionExecuted?.Invoke($"Fighter повернул на {angle} градусов");
    }
    public void Fire()
    {
        OnActionExecuted?.Invoke("Fighter выстрелил ракетой");
    }
}
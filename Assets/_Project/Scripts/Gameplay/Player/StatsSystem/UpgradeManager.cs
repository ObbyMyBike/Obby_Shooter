using UniRx;

public class UpgradeManager
{
    public ReactiveProperty<int> Points { get; } = new ReactiveProperty<int>(0);

    public void AddPoint(int amount = 1) => Points.Value += amount;

    public bool SpendPoint()
    {
        if (Points.Value <= 0)
            return false;
        
        Points.Value--;
        
        return true;
    }
}
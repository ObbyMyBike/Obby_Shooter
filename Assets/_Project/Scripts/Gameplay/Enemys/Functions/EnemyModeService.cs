using UniRx;

public class EnemyModeService 
{
    public ReactiveProperty<bool> PatrolEnabled { get; } = new ReactiveProperty<bool>(false);
    public ReactiveProperty<bool> ChaseEnabled  { get; } = new ReactiveProperty<bool>(true);
}
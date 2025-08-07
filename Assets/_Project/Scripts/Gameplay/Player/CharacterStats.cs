using UniRx;
using Zenject;

public class CharacterStats
{
    public ReactiveProperty<float> Speed { get; }
    public ReactiveProperty<float> HealthMax { get; }
    public ReactiveProperty<float> Damage { get; }

    [Inject]
    public CharacterStats(CharacterConfig config)
    {
        Speed = new ReactiveProperty<float>(config.SpeedMovement);
        HealthMax = new ReactiveProperty<float>(config.BaseHealth);
        Damage = new ReactiveProperty<float>(config.BaseDamage);
    }
    
    public void UpgradeSpeed(float delta) => Speed.Value += delta;
    
    public void UpgradeHealth(float delta) => HealthMax.Value += delta;
    
    public void UpgradeDamage(float delta) => Damage.Value += delta;
}
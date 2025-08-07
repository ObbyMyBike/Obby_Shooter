using UniRx;
using UnityEngine;

public class HealthModel
{
    public ReactiveProperty<float> Current { get; }
    public float Max { get; }

    public HealthModel(float max)
    {
        Max = max;
        Current = new ReactiveProperty<float>(max);
    }

    public void TakeDamage(float damage) => Current.Value = Mathf.Max(0, Current.Value - damage);

    public void Heal(float amount) => Current.Value = Mathf.Min(Max, Current.Value + amount);
}
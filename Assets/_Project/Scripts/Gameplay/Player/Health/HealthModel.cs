using UniRx;
using UnityEngine;

public class HealthModel
{
    public ReactiveProperty<float> Current { get; }
    public ReactiveProperty<float> Max { get; }

    public HealthModel(float max)
    {
        Max = new ReactiveProperty<float>(Mathf.Max(1f, max));
        Current = new ReactiveProperty<float>(Max.Value);
    }

    public void TakeDamage(float damage) => Current.Value = Mathf.Max(0, Current.Value - damage);

    public void Heal(float amount) => Current.Value = Mathf.Min(Max.Value, Current.Value + amount);
    
    public void SetMax(float newMax, bool keepRatio = true)
    {
        newMax = Mathf.Max(1f, newMax);
        
        float previewMax = Max.Value;
        float previewCurrent = Current.Value;

        Max.Value = newMax;

        if (keepRatio && previewMax > 0f)
        {
            float ratio = previewCurrent / previewMax;
            Current.Value = Mathf.Clamp01(ratio) * newMax;
        }
        else
        {
            Current.Value = Mathf.Min(previewCurrent, newMax);
        }
    }
}
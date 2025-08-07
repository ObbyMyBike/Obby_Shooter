using System;
using UniRx;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private HealthModel _health;
    private GenericPool<Enemy> _pool;
    private Subject<Unit> _death = new Subject<Unit>();
    
    public IObservable<Unit> Death => _death.AsObservable();

    private void OnDestroy()
    {
        disposables.Dispose();
        _death?.Dispose();
    }
    
    public void Initialize(GenericPool<Enemy> pool, EnemyConfig config)
    {
        _pool = pool;
        
        disposables.Clear();
        _death = new Subject<Unit>();
        _health = new HealthModel(config.BaseHealth);

        _health.Current.Subscribe(health =>
        {
            if (health <= 0f)
            {
                _death.OnNext(Unit.Default);
                _death.OnCompleted();

                gameObject.SetActive(false);
                _pool.Recycle(this);
            }
        }).AddTo(disposables);

        _health.Heal(_health.Max);
        gameObject.SetActive(true);
    }

    public void TakeDamage(float amount)
    {
        if (_health == null)
            return;
        
        _health.TakeDamage(amount);
    }
}
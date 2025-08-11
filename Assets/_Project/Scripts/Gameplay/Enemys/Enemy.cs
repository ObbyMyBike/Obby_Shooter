using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    
    private HealthModel _health;
    private EnemyMover _mover;
    private GenericPool<Enemy> _pool;
    private Subject<Unit> _death = new Subject<Unit>();
    
    public IObservable<Unit> Death => _death.AsObservable();

    private void OnDestroy()
    {
        _mover?.Dispose();
        disposables.Dispose();
        _death?.Dispose();
    }
    
    private void Update()
    {
        _mover?.Tick();
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
                _mover?.Dispose();
                _pool.Recycle(this);
            }
        }).AddTo(disposables);

        _health.SetMax(config.BaseHealth, keepRatio: false);
        gameObject.SetActive(true);
    }

    public void SetupMover(EnemyConfig config, EnemyModeService modeService, Transform player, List<Transform> patrolPoints)
    {
        _mover?.Dispose();

        TryGetComponent(out CharacterController controller);
        
        _mover = new EnemyMover(modeService, config, player, transform, controller);
        
        _mover.SetPatrolPoints(patrolPoints);
    }
    
    public void TakeDamage(float amount)
    {
        if (_health == null)
            return;
        
        _health.TakeDamage(amount);
    }
    
    private void OnDrawGizmos()
    {
        _mover?.DrawGizmos();
    }
}
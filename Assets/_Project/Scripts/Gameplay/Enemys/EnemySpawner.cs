using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour, IInitializable
{
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private float _spawnInterval = 5f;

    private readonly Dictionary<Transform, Enemy> active = new Dictionary<Transform, Enemy>();

    private EnemyConfig _enemyConfig;
    private GenericPool<Enemy> _enemyPool;
    private CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(GenericPool<Enemy> enemyPool, EnemyConfig enemyConfig)
    {
        _enemyPool = enemyPool;
        _enemyConfig = enemyConfig;
    }

    public void Initialize()
    {
        TrySpawnAll();
        
        Observable.Interval(TimeSpan.FromSeconds(_spawnInterval)).Subscribe(_ => TrySpawnAll()).AddTo(_disposables);
    }

    private void TrySpawnAll()
    {
        foreach ( Transform point in _spawnPoints)
        {
            // Если на точке никого нет или предыдущий убит
            if (!active.ContainsKey(point))
            {
                Enemy enemy = _enemyPool.Create(point.position, point.rotation);
                enemy.Initialize(_enemyPool, _enemyConfig);
                active[point] = enemy;
                
                Debug.Log($"[EnemySpawner] Spawned enemy at {point.name}");
                
                enemy.Death.Take(1).Subscribe(__ =>
                    {
                        Debug.Log($"[EnemySpawner] Enemy at {point.name} died, freeing spawn");
                        active.Remove(point);
                    }).AddTo(_disposables);
            }
        }
    }

    public void Dispose() => _disposables.Dispose();
}
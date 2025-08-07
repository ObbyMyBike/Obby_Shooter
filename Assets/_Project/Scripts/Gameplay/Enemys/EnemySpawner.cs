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
    private UpgradeManager _upgrades;
    private GenericPool<Enemy> _enemyPool;
    private CompositeDisposable _disposables = new CompositeDisposable();

    [Inject]
    public void Construct(GenericPool<Enemy> enemyPool, EnemyConfig enemyConfig, UpgradeManager upgrades)
    {
        _enemyPool = enemyPool;
        _enemyConfig = enemyConfig;
        _upgrades = upgrades;
    }

    public void Initialize()
    {
        TrySpawnAll();

        Observable.Interval(TimeSpan.FromSeconds(_spawnInterval)).Subscribe(_ => TrySpawnAll()).AddTo(_disposables);
    }

    private void TrySpawnAll()
    {
        foreach (Transform point in _spawnPoints)
        {
            if (!active.ContainsKey(point))
            {
                Enemy enemy = _enemyPool.Create(point.position, point.rotation);
                enemy.Initialize(_enemyPool, _enemyConfig);
                active[point] = enemy;

                enemy.Death.Take(1).Subscribe(__ =>
                {
                    active.Remove(point);
                    _upgrades.AddPoint();
                }).AddTo(_disposables);
            }
        }
    }

    public void Dispose() => _disposables.Dispose();
}
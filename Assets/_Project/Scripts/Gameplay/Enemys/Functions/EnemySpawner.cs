using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Zenject;

public class EnemySpawner : MonoBehaviour, IInitializable
{
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private List<Transform> _patrolPoints;
    [SerializeField] private float _spawnInterval = 5f;
    [SerializeField] private int _maxAlive = 20;

    private readonly Dictionary<Transform, Enemy> _active = new Dictionary<Transform, Enemy>();
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    
    private EnemyConfig _enemyConfig;
    private UpgradeManager _upgrades;
    private GenericPool<Enemy> _enemyPool;
    private EnemyModeService _modeService;
    private Transform _player;

    [Inject]
    public void Construct(GenericPool<Enemy> enemyPool, EnemyConfig enemyConfig, UpgradeManager upgrades, EnemyModeService modeService, Camera playerCamera)
    {
        _enemyPool = enemyPool;
        _enemyConfig = enemyConfig;
        _upgrades = upgrades;
        _modeService = modeService;
        _player = playerCamera != null ? playerCamera.transform : null;
    }

    public void Initialize()
    {
        TrySpawnAll();

        Observable.Interval(TimeSpan.FromSeconds(_spawnInterval)).Subscribe(_ => TrySpawnAll()).AddTo(_disposables);
    }

    private void TrySpawnAll()
    {
        if (_active.Count >= _maxAlive)
            return;
        
        foreach (Transform point in _spawnPoints)
        {
            if (_active.Count >= _maxAlive)
                break;
            
            if (point == null)
                continue;
            
            if (_active.ContainsKey(point)) continue;
            
            if (!_active.ContainsKey(point))
            {
                float overlapRadius = 0.5f;
                Collider[] colliders = Physics.OverlapSphere(point.position, overlapRadius);
                
                if (Array.Exists(colliders, collider => collider.GetComponentInParent<Enemy>() != null))
                    continue;
                
                Enemy enemy = _enemyPool.Create(point.position, point.rotation);
                enemy.Initialize(_enemyPool, _enemyConfig);
                enemy.SetupMover(_enemyConfig, _modeService, _player, _patrolPoints);
                _active[point] = enemy;

                enemy.Death.Take(1).Subscribe(_ =>
                {
                    _active.Remove(point);
                    _upgrades.AddPoint();
                }).AddTo(_disposables);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        if (_spawnPoints != null)
        {
            Gizmos.color = Color.green;
            
            foreach (var p in _spawnPoints)
            {
                if (p == null)
                    continue;
                
                Gizmos.DrawWireCube(p.position, Vector3.one * 0.5f);
            }
        }
        
        if (_patrolPoints != null)
        {
            Gizmos.color = Color.cyan;
            Transform prev = null;
            
            foreach (var p in _patrolPoints)
            {
                if (p == null)
                    continue;
                
                Gizmos.DrawWireSphere(p.position, 0.35f);
                
                if (prev != null)
                    Gizmos.DrawLine(prev.position, p.position);
                
                prev = p;
            }
            
            if (_patrolPoints.Count > 2 && _patrolPoints[0] != null && prev != null)
                Gizmos.DrawLine(prev.position, _patrolPoints[0].position);
        }
    }
}
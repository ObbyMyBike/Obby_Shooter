using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEditor;

public class EnemyMover : IDisposable
{
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private readonly System.Random _random = new System.Random();
    
    private EnemyModeService _modeService;
    private CharacterController _characterController;
    private EnemyConfig _config;
    private Transform _self;
    private Transform _player;
    
    private List<Transform> _patrolPoints;
    private int _currentIndex = -1;
    
    public EnemyMover(EnemyModeService modeService, EnemyConfig config, Transform player, Transform self, CharacterController characterController)
    {
        _modeService = modeService;
        _config = config;
        _player = player;
        _self = self;
        _characterController = characterController;
        
        _modeService.PatrolEnabled.Merge(_modeService.ChaseEnabled).Subscribe(_ => _currentIndex = -1).AddTo(_disposables);
    }

    public IReadOnlyList<Transform> PatrolPoints => _patrolPoints;
    public Transform CurrentPatrolTarget => (_patrolPoints != null && _currentIndex >= 0 && _currentIndex < _patrolPoints.Count) ? _patrolPoints[_currentIndex] : null;
    public float DetectionRadius => _config.DetectionRadius;
    public float PointOccupyRadius => _config.PointOccupyRadius;
    
    public void SetPatrolPoints(List<Transform> points)
    {
        _patrolPoints = points;
        _currentIndex = -1;
    }
    
    public void Tick()
    {
        if (_modeService.ChaseEnabled.Value && _player != null)
        {
            float dist = Vector3.Distance(PlanePosition(_self.position), PlanePosition(_player.position));
            
            if (dist <= _config.DetectionRadius)
            {
                TickChase();
                
                return;
            }
        }
        
        TickPatrol();
    }

    public void Dispose() => _disposables.Dispose();

    private void TickPatrol()
    {
        if (_patrolPoints == null || _patrolPoints.Count == 0)
            return;

        if (_currentIndex < 0 || _currentIndex >= _patrolPoints.Count)
            _currentIndex = FindBestFreePointIndex(preferNearest: true);
        
        if (IsPointOccupied(_patrolPoints[_currentIndex]))
        {
            int next = FindBestFreePointIndex(preferNearest: false, excludeIndex: _currentIndex);
            
            if (next >= 0)
                _currentIndex = next;
        }

        Transform target = _patrolPoints[_currentIndex];
        MoveTowards(target.position, _config.PointReachDistance);
        
        if (Vector3.Distance(PlanePosition(_self.position), PlanePosition(target.position)) <= _config.PointReachDistance)
        {
            int next = FindBestFreePointIndex(preferNearest: false, excludeIndex: _currentIndex);
            
            if (next >= 0)
                _currentIndex = next;
            else
                PickNextPointRandom(_currentIndex);
        }
    }

    private void TickChase()
    {
        if (_player == null)
            return;
        
        float distance = Vector3.Distance(PlanePosition(_self.position), PlanePosition(_player.position));
        
        if (distance > _config.StopDistance)
            MoveTowards(_player.position, _config.StopDistance);
        
        FaceTo(_player.position);
    }

    private void MoveTowards(Vector3 worldTarget, float stopDistance)
    {
        Vector3 target = PlanePosition(worldTarget) - PlanePosition(_self.position);
        float distance = target.magnitude;
        
        if (distance <= stopDistance)
            return;
        
        Vector3 direction = target / distance;
        
        FaceDirection(direction);
        
        Vector3 delta = direction * (_config.MoveSpeed * Time.deltaTime);
        
        if (_characterController != null && _characterController.enabled)
            _characterController.Move(delta);
        else
            _self.position += delta;
    }

    private void FaceTo(Vector3 worldTarget)
    {
        Vector3 direction = PlanePosition(worldTarget) - PlanePosition(_self.position);

        if (direction.sqrMagnitude > 0.0001f)
            FaceDirection(direction.normalized);
    }

    private void FaceDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        _self.rotation = Quaternion.Slerp(_self.rotation, targetRotation, _config.TurnSpeed * Time.deltaTime);
    }
    
    private bool IsPointOccupied(Transform point)
    {
        if (point == null)
            return false;

        Collider[] hits = Physics.OverlapSphere(point.position, _config.PointOccupyRadius, _config.EnemyMask.value == 0 ? ~0 : _config.EnemyMask);
        
        for (int i = 0; i < hits.Length; i++)
        {
            Enemy enemy = hits[i].GetComponentInParent<Enemy>();
            
            if (enemy != null && enemy.transform != _self)
                return true;
        }
        
        return false;
    }

    private int FindBestFreePointIndex(bool preferNearest, int? excludeIndex = null)
    {
        if (_patrolPoints == null || _patrolPoints.Count == 0)
            return -1;

        List<int> free = new List<int>(_patrolPoints.Count);
        
        for (int i = 0; i < _patrolPoints.Count; i++)
        {
            if (excludeIndex.HasValue && i == excludeIndex.Value)
                continue;
            
            Transform point = _patrolPoints[i];
            
            if (point == null)
                continue;

            if (!IsPointOccupied(point))
                free.Add(i);
        }

        if (free.Count == 0)
            return -1;

        if (preferNearest)
        {
            float best = float.MaxValue;
            int bestIndex = free[0];
            Vector3 enemyPosition = PlanePosition(_self.position);
            
            for (int k = 0; k < free.Count; k++)
            {
                int index = free[k];
                float distance = Vector3.Distance(enemyPosition, PlanePosition(_patrolPoints[index].position));
                
                if (distance < best)
                {
                    best = distance;
                    bestIndex = index;
                }
            }
            
            return bestIndex;
        }
        else
        {
            return free[_random.Next(0, free.Count)];
        }
    }
    
    private void PickNextPointRandom(int? excludeIndex = null)
    {
        if (_patrolPoints == null || _patrolPoints.Count == 0)
            return;

        int next;

        if (_patrolPoints.Count == 1)
        {
            next = 0;
        }
        else
        {
            do {next = _random.Next(0, _patrolPoints.Count);}
            while (excludeIndex.HasValue && next == excludeIndex.Value);
        }
        
        _currentIndex = next;
    }
    
    private Vector3 PlanePosition(Vector3 vector) => new Vector3(vector.x, 0f, vector.z);
    
    public void DrawGizmos()
    {
        if (_self == null || _config == null)
            return;
        
        Handles.color = Color.yellow;
        Handles.DrawWireDisc(_self.position, Vector3.up, _config.DetectionRadius);
        
        if (_player != null)
        {
            float dist = Vector3.Distance(PlanePosition(_self.position), PlanePosition(_player.position));
            
            if (dist <= _config.DetectionRadius)
            {
                Handles.color = Color.red;
                Handles.DrawLine(_self.position, _player.position);
            }
        }
        
        if (_patrolPoints != null && _currentIndex >= 0 && _currentIndex < _patrolPoints.Count)
        {
            Transform target = _patrolPoints[_currentIndex];
            
            if (target != null)
            {
                Handles.color = Color.cyan;
                Handles.DrawWireDisc(target.position, Vector3.up, _config.PointOccupyRadius);
                Handles.DrawDottedLine(_self.position, target.position, 2f);
            }
        }
    }
}
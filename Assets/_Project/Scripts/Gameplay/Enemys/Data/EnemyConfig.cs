using UnityEngine;

[CreateAssetMenu(menuName = "Game/Settings Enemy", fileName = "New Enemy Settings")]
public class EnemyConfig : ScriptableObject
{
    public float BaseHealth = 50f;

    [Header("Movement")]
    public float MoveSpeed = 2.5f;
    public float StopDistance = 3f;
    public float TurnSpeed = 10f;
    public float PointReachDistance = 0.25f;
    public float DetectionRadius = 10f;
    public float PointOccupyRadius = 0.7f;
    public LayerMask EnemyMask;
    
    [Header("Pool Settings")]
    public Enemy EnemyPrefab;
    public int EnemyPoolSize = 10;
}
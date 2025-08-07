using UnityEngine;

[CreateAssetMenu(menuName = "Game/Settings Enemy", fileName = "New Enemy Settings")]
public class EnemyConfig : ScriptableObject
{
    public float BaseHealth = 50f;

    [Header("Pool Settings")]
    public Enemy EnemyPrefab;
    public int EnemyPoolSize = 10;
}
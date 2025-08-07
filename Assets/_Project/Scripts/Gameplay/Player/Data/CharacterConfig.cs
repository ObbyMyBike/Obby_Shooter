using UnityEngine;

[CreateAssetMenu(menuName = "Game/Settings Player", fileName = "New Player Settings")]
public class CharacterConfig : ScriptableObject
{
    public float SpeedMovement = 5f;
    public float BaseHealth = 100f;
    
    [Header("Fire Settings")]
    public float BaseDamage = 10f;
    public float BaseBulletSpeed = 100f;
    public float FireRate = 5f;
    public float Range = 100f; 
    public int BulletPoolSize = 20;
    
    [field: SerializeField] public Bullet BulletPrefab { get; private set; }
    [field: SerializeField] public KeyCode FireButton { get; private set; }
}
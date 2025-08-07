using UnityEngine;

[CreateAssetMenu(menuName = "Game/Settings Player", fileName = "New Player Settings")]
public class CharacterConfig : ScriptableObject
{
    public float SpeedMovement;
    public float BaseHealth;
    
    [Header("Fire Settings")]
    public float BaseDamage = 10f;
    public float BaseBulletSpeed = 5f;
    public float FireRate;
    public float Range = 100f; 
    public int BulletPoolSize = 20;
    
    [field: SerializeField] public KeyCode FireButton { get; private set; }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Settings Player", fileName = "New Player Settings")]
public class CharacterConfig : ScriptableObject
{
    public float CurrentBaseSpeedMovement = 5f;
    public float CurrentBaseHealth = 100f;

    [Header("Fire Settings")]
    public float CurrentBaseDamage = 10f;
    public float BulletSpeed = 100f;
    public float FireRate = 5f;
    public float Range = 100f;
    public int BulletPoolSize = 20;

    [Header("Max Values")]
    public float MaxSpeed = 10f;
    public float MaxHealth = 200f;
    public float MaxDamage = 20f;


    [field: SerializeField] public Bullet BulletPrefab { get; private set; }
    [field: SerializeField] public KeyCode FireButton { get; private set; }
}
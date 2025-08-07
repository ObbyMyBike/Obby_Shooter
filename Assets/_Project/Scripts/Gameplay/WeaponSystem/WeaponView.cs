using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private LayerMask _collisionMask;

    public Transform SpawnPoint => _spawnPoint;
    public LayerMask CollisionLayers => _collisionMask;
}
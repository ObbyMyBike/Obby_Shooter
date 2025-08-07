using UnityEngine;

public class WeaponView : MonoBehaviour
{
    [SerializeField] private Transform _spawnPoint; 
    [SerializeField] private LayerMask _hitLayers;

    public Transform SpawnPoint => _spawnPoint;
    public LayerMask HitLayers => _hitLayers;
}
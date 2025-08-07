using UnityEngine;

public class ShotgunController : WeaponBase
{
    protected ShotgunController(IInputService input, CharacterConfig config, Camera mainCamera, WeaponView view) : base(input, config, mainCamera, view)
    {
    }

    protected override void Fire()
    {
        Vector3 origin  = MuzzlePoint.position;
        Vector3 direction = MainCamera.transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, Range, HitMask))
        {
            if (hit.collider.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(Damage);
        }
    }
}
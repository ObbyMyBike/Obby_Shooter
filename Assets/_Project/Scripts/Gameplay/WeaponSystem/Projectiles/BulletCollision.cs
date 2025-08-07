using UnityEngine;

public class BulletCollision
{
    public void Collision(Bullet bullet, Collider other, IDamageDealer shooter)
    {
        if (other.TryGetComponent(out IDamageable damageable))
            damageable.TakeDamage(shooter.Damage);
    }
}
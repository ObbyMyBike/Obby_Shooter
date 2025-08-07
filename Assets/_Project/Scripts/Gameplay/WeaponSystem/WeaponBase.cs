using System;
using UniRx;
using UnityEngine;
using Zenject;

public class WeaponBase : IInitializable, IDisposable, IDamageDealer
{
    private readonly BulletCollision bulletCollision;
    private readonly IInputService input;
    private readonly GenericPool<Bullet> bulletPool;
    private readonly CompositeDisposable disposables = new();
    private readonly Camera mainCamera;
    private readonly Transform muzzlePoint;
    private readonly LayerMask collisionMask;
    
    private readonly float fireRate;
    private readonly float range;
    private readonly float bulletSpeed;
    
    protected WeaponBase(IInputService input, CharacterConfig config, Camera mainCamera, WeaponView view, GenericPool<Bullet> bulletPool, BulletCollision bulletCollision)
    {
        this.mainCamera = mainCamera;
        this.input = input;
        this.bulletPool = bulletPool;
        this.bulletCollision = bulletCollision;
        this.muzzlePoint = view.SpawnPoint;
        this.collisionMask = view.CollisionLayers;
        this.fireRate = config.FireRate;
        this.range = config.Range;
        this.bulletSpeed = config.BaseBulletSpeed;
        Damage = config.BaseDamage;
    }

    public float Damage { get; }
    
    public virtual void Initialize()
    {
        input.Fire.ThrottleFirst(TimeSpan.FromSeconds(1f / fireRate)).Subscribe(_ => Fire()).AddTo(disposables);
    }

    public virtual void Dispose() => disposables.Dispose();
    
    protected virtual void Fire()
    {
        Vector3 origin = muzzlePoint.position;
        Vector3 direction = mainCamera.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(direction);

        Bullet bullet = bulletPool.Create(origin, rotation);
        bullet.Initialize(bulletPool, this, bulletSpeed, direction, collisionMask, bulletCollision, range);
        
        if (Physics.Raycast(origin, direction, out var hit, range, collisionMask))
        {
            if (hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(Damage);
            }
        }
    }
}
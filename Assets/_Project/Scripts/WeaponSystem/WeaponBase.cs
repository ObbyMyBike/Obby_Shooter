using System;
using UniRx;
using UnityEngine;
using Zenject;

public class WeaponBase : IInitializable, IDisposable
{
    public readonly IInputService Input;
    public readonly Camera MainCamera;
    public readonly Transform MuzzlePoint;
    public readonly LayerMask HitMask;
    public readonly float FireRate;
    public readonly float Damage;
    public readonly float Range;
    public readonly CompositeDisposable Disposables = new();

    protected WeaponBase(IInputService input, CharacterConfig config, Camera mainCamera, WeaponView view)
    {
        Input = input;
        MainCamera = mainCamera;
        MuzzlePoint = view.SpawnPoint;
        HitMask = view.HitLayers;

        FireRate = config.FireRate;
        Damage = config.BaseDamage;
        Range = config.Range;
    }

    public virtual void Initialize()
    {
        Input.Fire.ThrottleFirst(TimeSpan.FromSeconds(1f / FireRate)).Subscribe(_ => Fire()).AddTo(Disposables);
    }

    public virtual void Dispose() => Disposables.Dispose();
    
    protected virtual void Fire()
    {
        Vector3 origin = MuzzlePoint.position;
        Vector3 direction = MainCamera.transform.forward;

        if (Physics.Raycast(origin, direction, out var hit, Range, HitMask))
        {
            if (hit.collider.TryGetComponent(out IDamageable damageable))
                damageable.TakeDamage(Damage);
        }
    }
}
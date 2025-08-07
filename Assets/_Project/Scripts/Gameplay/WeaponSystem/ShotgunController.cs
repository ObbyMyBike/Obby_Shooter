using UnityEngine;
using Zenject;

public class ShotgunController : WeaponBase
{
    [Inject]
    public ShotgunController(IInputService input, CharacterConfig config, Camera mainCamera, WeaponView view, GenericPool<Bullet> bulletPool, BulletCollision collision)
        : base(input, config, mainCamera, view, bulletPool, collision)
    {
    }
}
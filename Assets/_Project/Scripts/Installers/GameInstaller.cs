using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CharacterConfig _playerSettings;
    [SerializeField] private EnemyConfig _enemySettings;
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private WeaponView _weaponView;

    [Header("UI Settings")]
    [SerializeField] private Joystick _joystick;
    [SerializeField] private Button _fireButton;
    [SerializeField] private Canvas _uiCanvas;
    [SerializeField] private RectTransform _aimZoneRect;

    public override void InstallBindings()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        _uiCanvas.gameObject.SetActive(false);
#elif UNITY_ANDROID || UNITY_IOS
        _uiCanvas.gameObject.SetActive(true);
#endif
            
        Container.Bind<ILocalizationService>().To<LocalizationData>().FromScriptableObjectResource("Game/Localization").AsSingle();

        Container.Bind<GenericPool<Bullet>>().AsSingle().WithArguments(_playerSettings.BulletPrefab, (Transform)null, _playerSettings.BulletPoolSize);
        Container.Bind<BulletCollision>().AsSingle();
        
        Container.Bind<GenericPool<Enemy>>().AsSingle().WithArguments(_enemySettings.EnemyPrefab, (Transform)null, _enemySettings.EnemyPoolSize);
        Container.BindInstance(_enemySettings).AsSingle();
        Container.Bind<UpgradeManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<EnemySpawner>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        Container.Bind<HealthBarView>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<MenuController>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<UpgradeUIController>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<CharacterStatsPanel>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        Container.Bind<IStatPersistenceService>().To<StatPersistenceService>().AsSingle();
        
        Container.BindInstance(_playerSettings).AsSingle();
        Container.BindInstance(_joystick).AsSingle();
        Container.BindInstance(_fireButton).AsSingle();
        Container.BindInstance(_aimZoneRect).AsSingle();
        Container.BindInstance(_uiCanvas).AsSingle();

#if UNITY_STANDALONE || UNITY_EDITOR
        Container.BindInterfacesAndSelfTo<DesktopInput>().AsSingle().NonLazy();
#elif UNITY_ANDROID || UNITY_IOS
        Container.BindInterfacesAndSelfTo<MobileInput>().AsSingle().NonLazy();
#endif

        Container.Bind<Camera>().FromInstance(_playerCamera).AsSingle();

        Container.BindInstance(_weaponView).AsSingle();
        Container.BindInterfacesAndSelfTo<ShotgunController>().AsSingle().NonLazy();

        Container.Bind<PlayerMovement>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<FirstPersonCamera>().FromComponentInHierarchy().AsSingle().NonLazy();
        
        Container.Bind<HealthModel>().AsSingle().WithArguments(_playerSettings.CurrentBaseHealth);
        Container.Bind<CharacterStats>().AsSingle();
    }
}
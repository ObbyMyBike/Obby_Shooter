using System;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController _characterController;
    private CharacterConfig _settings;
    private Joystick _joystick;
    private Camera _camera;
    private IInputService _input;
    private IDisposable _moveSubscription;

    [Inject]
    public void Construct(IInputService inputService, CharacterConfig characterConfig, Joystick joystick, Camera camera)
    {
        _input = inputService;
        _settings = characterConfig;
        _joystick = joystick;
        _camera = camera;
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        _moveSubscription = _input.MoveAxis.Subscribe(direction =>
            {
                Vector3 forward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
                Vector3 right = Vector3.ProjectOnPlane(_camera.transform.right,   Vector3.up).normalized;
                Vector3 moveDirection = right * direction.x + forward * direction.y;

                _characterController.Move(moveDirection * (_settings.SpeedMovement * Time.deltaTime));
            });
#endif
    }
    
    private void Update()
    {
#if !(UNITY_STANDALONE || UNITY_EDITOR)
        Vector2 direction = _joystick.Direction;
        
        if (direction.sqrMagnitude >= _joystick.DeadZone * _joystick.DeadZone)
        {
            Vector3 forward = Vector3.ProjectOnPlane(_camera.transform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(_camera.transform.right, Vector3.up).normalized;
            Vector3 motion = right * direction.x + forward * direction.y;
            _characterController.Move(motion * (_settings.SpeedMovement * Time.deltaTime));
        }
#endif
    }
    
    private void OnDestroy()
    {
        _moveSubscription?.Dispose();
    }
}
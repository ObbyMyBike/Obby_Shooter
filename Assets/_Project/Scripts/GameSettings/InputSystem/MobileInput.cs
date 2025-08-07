using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UniRx;
using Zenject;

public class MobileInput : IInputService, IInitializable, ITickable
{
    private readonly Joystick _joystick;
    private readonly Button _fireButton;
    private readonly RectTransform _aimZoneRect;
    private readonly Canvas _uiCanvas;
    private readonly Subject<Vector2> _moveSubscription = new Subject<Vector2>();
    private readonly Subject<Vector2> _aimSubscription = new Subject<Vector2>();
    
    private IObservable<Unit> _fireStream;

    [Inject]
    public MobileInput(Joystick joystick, Button fireButton, RectTransform aimZoneRect, Canvas uiCanvas)
    {
        _joystick = joystick;
        _fireButton = fireButton;
        _aimZoneRect = aimZoneRect;
        _uiCanvas = uiCanvas;
    }

    public void Initialize()
    {
        _fireStream = _fireButton.OnClickAsObservable();
    }

    public void Tick()
    {
        Vector2 direction = _joystick.Direction;
        
        if (direction.sqrMagnitude > _joystick.DeadZone * _joystick.DeadZone)
            _moveSubscription.OnNext(direction);
        
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Moved)
            {
                bool inZone = RectTransformUtility.RectangleContainsScreenPoint(_aimZoneRect, touch.position, _uiCanvas.renderMode == RenderMode.ScreenSpaceCamera ? _uiCanvas.worldCamera : null);
                bool overUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);

                if (inZone && !overUI)
                    _aimSubscription.OnNext(touch.deltaPosition);
            }
        }
    }
    
    public IObservable<Vector2> MoveAxis => _moveSubscription;

    public IObservable<Vector2> AimAxis => _aimSubscription;

    public IObservable<Unit> Fire => _fireStream;
}
using System;
using UnityEngine;
using UniRx;
using Zenject;

public class DesktopInput : IInputService, ITickable
{
    private const string HORIZONTAL_INPUT = "Horizontal";
    private const string VERTICAL_INPUT = "Vertical";
    private const string MOUSE_X = "Mouse X";
    private const string MOUSE_Y = "Mouse Y";

    private readonly CharacterConfig _settings;
    private readonly Subject<Vector2> _move = new Subject<Vector2>();
    private readonly Subject<Vector2> _aim = new Subject<Vector2>();
    private readonly Subject<Unit> _fire = new Subject<Unit>();

    [Inject]
    public DesktopInput(CharacterConfig settings)
    {
        _settings = settings;
    }

    public IObservable<Vector2> MoveAxis => _move;
    public IObservable<Vector2> AimAxis => _aim;
    public IObservable<Unit> Fire => _fire;

    public void Tick()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        _move.OnNext(new Vector2(Input.GetAxis(HORIZONTAL_INPUT), Input.GetAxis(VERTICAL_INPUT)));
        
        _aim.OnNext(new Vector2(Input.GetAxis(MOUSE_X), Input.GetAxis(MOUSE_Y)));

        if (Input.GetKeyDown(_settings.FireButton))
            _fire.OnNext(Unit.Default);
    }
}
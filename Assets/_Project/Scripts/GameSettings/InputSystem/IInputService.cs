using System;
using UnityEngine;
using UniRx;

public interface IInputService
{
    public IObservable<Vector2> MoveAxis { get; }
    public IObservable<Vector2> AimAxis { get; }
    public IObservable<Unit> Fire { get; }
}
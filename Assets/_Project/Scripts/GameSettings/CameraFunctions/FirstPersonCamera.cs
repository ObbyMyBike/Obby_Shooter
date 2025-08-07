using UnityEngine;
using UniRx;
using Zenject;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] Transform _bodyYawPivot;
    [SerializeField] Transform _cameraPitchPivot;
    [SerializeField] private float _sensitivityDesktop = 100f;
    [SerializeField] private float _sensitivityMobile = 50f;

    private CompositeDisposable _subscription = new CompositeDisposable();
    private float _sensitivity;
    private float _pitch;
    
    [Inject] private IInputService _input;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

#if UNITY_STANDALONE || UNITY_EDITOR
        _sensitivity = _sensitivityDesktop;
#else
        _sensitivity = _sensitivityMobile;
#endif
        
        _input.AimAxis.Subscribe(OnLook).AddTo(_subscription);
    }
    
    void OnLook(Vector2 delta)
    {
        float yaw = delta.x * _sensitivity * Time.deltaTime;
        
        _bodyYawPivot.Rotate(Vector3.up * yaw, Space.Self);
        
        float pitchDelta = delta.y * _sensitivity * Time.deltaTime;
        
        _pitch = Mathf.Clamp(_pitch - pitchDelta, -90f, 90f);
        _cameraPitchPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private void OnDestroy()
    {
        _subscription.Dispose();
    }
}
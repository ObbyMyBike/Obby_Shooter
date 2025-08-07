using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Scrollbar _scrollbarHealthValue;
    [SerializeField] private Image _handleImage;

    private float _initialMax;
    private float _initialWidth;
    private RectTransform _trackRect;

    [Inject] private HealthModel _health;

    private void Awake()
    {
        _trackRect = _scrollbarHealthValue.GetComponent<RectTransform>();
        _initialMax = _health.Max;
        _health.Current.Subscribe(UpdateFill).AddTo(this);
    }

    // private void Start()
    // {
    //     _initialMax = _health.Max;
    //     _health.Current.Subscribe(UpdateFill).AddTo(this);
    // }

    public void UpdateTrack(float newMax)
    {
        if (_initialWidth <= 0f)
        {
            _initialWidth = _trackRect.rect.width;
            
            if (_initialWidth <= 0f)
                _initialWidth = 1f;
        }

        float factor = newMax / _initialMax;
        
        _trackRect.sizeDelta = new Vector2(_initialWidth * factor, _trackRect.sizeDelta.y);
        
        // float factor = newMax / _initialMax;
        //
        // _trackRect.sizeDelta = new Vector2(_initialWidth * factor, _trackRect.sizeDelta.y);
    }
    
    private void UpdateFill(float current)
    {
        float size = Mathf.Clamp01(current / _health.Max);
        
        _scrollbarHealthValue.size = size;
        _handleImage.enabled = size > 0f;
    }
}
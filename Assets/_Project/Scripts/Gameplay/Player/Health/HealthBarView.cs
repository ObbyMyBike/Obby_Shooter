using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Scrollbar _scrollbarHealthValue;
    [SerializeField] private Image _handleImage;
    
    private RectTransform _trackRect;
    private float _baseTrackWidth;
    private float _baseMax;
    
    [Inject] private HealthModel _health;

    private void Awake()
    {
        _trackRect = _scrollbarHealthValue.GetComponent<RectTransform>();
        _baseTrackWidth = _trackRect.rect.width > 0 ? _trackRect.rect.width : 1f;
        
        _health.Max.Subscribe(OnMaxChanged).AddTo(this);
        _health.Current.Subscribe(UpdateFill).AddTo(this);
    }

    public void UpdateTrack(float newMax)
    {
        _health.SetMax(newMax, keepRatio: false);
    }
    
    private void OnMaxChanged(float newMax)
    {
        if (_baseMax <= 0f)
            _baseMax = newMax;
        
        float factor = Mathf.Max(0.01f, newMax / _baseMax);
        _trackRect.sizeDelta = new Vector2(_baseTrackWidth * factor, _trackRect.sizeDelta.y);
        
        UpdateFill(_health.Current.Value);
    }
    
    private void UpdateFill(float current)
    {
        float size = Mathf.Clamp01(_health.Max.Value <= 0f ? 0f : current / _health.Max.Value);
        
        _scrollbarHealthValue.size = size;
        _handleImage.enabled = size > 0f;
    }
}
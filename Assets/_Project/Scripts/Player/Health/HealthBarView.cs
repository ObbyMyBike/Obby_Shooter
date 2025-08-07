using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Zenject;

public class HealthBarView : MonoBehaviour
{
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private Image _handleImage;

    [Inject] private HealthModel _health;

    private void Awake()
    {
        _scrollbar = GetComponent<Scrollbar>();
        _scrollbar.interactable = false;
        _handleImage = _scrollbar.handleRect.GetComponent<Image>();
    }

    private void Start()
    {
        _health.Current.Subscribe(value =>
            {
                float t = Mathf.Clamp01(value / _health.Max);
                _scrollbar.size = t;
                _handleImage.enabled = t > 0f;
            }).AddTo(this);
    }
}
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class EnemyAIModePanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Toggle _patrolToggle;
    [SerializeField] private Toggle _chaseToggle;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private float _fade = 1f;
    
    private EnemyModeService _modeService;
    private bool _hovered;

    [Inject]
    public void Construct(EnemyModeService modeService)
    {
        _modeService = modeService;
    }

    private void Awake()
    {
        if (_patrolToggle != null)
            _patrolToggle.onValueChanged.AddListener(OnPatrolChanged);

        if (_chaseToggle != null)
            _chaseToggle.onValueChanged.AddListener(OnChaseChanged);
        
        SetVisible(false, instant: true);
    }

    private void OnDestroy()
    {
        if (_patrolToggle != null)
            _patrolToggle.onValueChanged.RemoveListener(OnPatrolChanged);
        
        if (_chaseToggle  != null)
            _chaseToggle.onValueChanged.RemoveListener(OnChaseChanged);
        
    }
    private void Start()
    {
        if (_patrolToggle != null)
            _patrolToggle.isOn = _modeService.PatrolEnabled.Value;
        
        if (_chaseToggle  != null)
            _chaseToggle.isOn  = _modeService.ChaseEnabled.Value;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovered = true;
        
        SetVisible(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
        
        SetVisible(false);
    }
    
    private void SetVisible(bool visible, bool instant = true)
    {
        if (_group == null)
        {
            gameObject.SetActive(visible);
            return;
        }

        _group.alpha = visible ? 1f : 0f;
        _group.blocksRaycasts = visible;
        _group.interactable = visible;
    }
    
    private void OnPatrolChanged(bool isOn) => _modeService.PatrolEnabled.Value = isOn;
    private void OnChaseChanged(bool isOn)  => _modeService.ChaseEnabled.Value  = isOn;
}
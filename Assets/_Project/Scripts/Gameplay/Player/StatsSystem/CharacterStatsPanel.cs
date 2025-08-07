using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using Zenject;

public class CharacterStatsPanel : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private TextMeshProUGUI _speedValue;
    [SerializeField] private Scrollbar _speedBar;
    [SerializeField] private Button _speedUpButton;
    [SerializeField] private Button _speedApplyButton;
    [SerializeField] private TextMeshProUGUI _speedMaxText;

    [Header("Health")]
    [SerializeField] private TextMeshProUGUI _healthValue;
    [SerializeField] private Scrollbar _healthBar;
    [SerializeField] private Button _healthUpButton;
    [SerializeField] private Button _healthApplyButton;
    [SerializeField] private TextMeshProUGUI _healthMaxText;

    [Header("Damage")]
    [SerializeField] private TextMeshProUGUI _damageValue;
    [SerializeField] private Scrollbar _damageBar;
    [SerializeField] private Button _damageUpButton;
    [SerializeField] private Button _damageApplyButton;
    [SerializeField] private TextMeshProUGUI _damageMaxText;

    [Header("Other")]
    [SerializeField] private KeyCode _resetKey = KeyCode.R;

    private CharacterStats _stats;
    private UpgradeManager _upgrades;
    private MenuController _menu;
    private CharacterConfig _config;
    private HealthBarView _healthBarView;
    private IStatPersistenceService _persist;

    private float _originalSpeed;
    private float _previewSpeed;
    private float _originalHealth;
    private float _previewHealth;
    private float _originalDamage;
    private float _previewDamage;

    [Inject]
    public void Construct(CharacterStats stats, UpgradeManager upgrades, MenuController menu, CharacterConfig config,
        HealthBarView healthBarView, IStatPersistenceService persist)
    {
        _stats = stats;
        _upgrades = upgrades;
        _menu = menu;
        _config = config;
        _healthBarView = healthBarView;
        _persist = persist;
    }

    private void Awake()
    {
        _speedBar.interactable = false;
        _healthBar.interactable = false;
        _damageBar.interactable = false;

        _speedApplyButton.interactable = false;
        _healthApplyButton.interactable = false;
        _damageApplyButton.interactable = false;

        _speedMaxText.gameObject.SetActive(false);
        _healthMaxText.gameObject.SetActive(false);
        _damageMaxText.gameObject.SetActive(false);
    }

    private void Start()
    {
        float savedSpeed = _persist.LoadSpeed();
        float savedHealth = _persist.LoadHealth();
        float savedDmg = _persist.LoadDamage();

        if (float.IsNaN(savedSpeed))
        {
            savedSpeed = _config.CurrentBaseSpeedMovement;
        }
        else
        {
            _config.CurrentBaseSpeedMovement = savedSpeed;
            _stats.Speed.Value = savedSpeed;
        }
        
        if (float.IsNaN(savedHealth))
        {
            savedHealth = _config.CurrentBaseHealth;
        }
        else
        {
            _config.CurrentBaseHealth = savedHealth;
            _stats.HealthMax.Value = savedHealth;
        }

        if (float.IsNaN(savedDmg))
        {
            savedDmg = _config.CurrentBaseDamage;
        }
        else
        {
            _config.CurrentBaseDamage = savedDmg;
            _stats.Damage.Value = savedDmg;
        }
        
        _healthBarView.UpdateTrack(_stats.HealthMax.Value);

        _originalSpeed = _previewSpeed = _stats.Speed.Value;
        _originalHealth = _previewHealth = _stats.HealthMax.Value;
        _originalDamage = _previewDamage = _stats.Damage.Value;

        _stats.Speed.Subscribe(value => _speedValue.text = value.ToString("F1")).AddTo(this);
        _stats.HealthMax.Subscribe(value => _healthValue.text = value.ToString("F1")).AddTo(this);
        _stats.Damage.Subscribe(value => _damageValue.text = value.ToString("F1")).AddTo(this);

        _speedUpButton.onClick.AddListener(() => StageUpgrade(1f, ref _previewSpeed, _config.MaxSpeed, _upgrades,
            _speedBar, _speedValue, _speedApplyButton));
        _healthUpButton.onClick.AddListener(() => StageUpgrade(10f, ref _previewHealth, _config.MaxHealth, _upgrades,
            _healthBar, _healthValue, _healthApplyButton));
        _damageUpButton.onClick.AddListener(() => StageUpgrade(1f, ref _previewDamage, _config.MaxDamage, _upgrades,
            _damageBar, _damageValue, _damageApplyButton));

        _speedApplyButton.onClick.AddListener(() => ApplyUpgrade(_previewSpeed, _originalSpeed, _config.MaxSpeed,
            newValue =>
            {
                _stats.UpgradeSpeed(newValue);
                _config.CurrentBaseSpeedMovement = _originalSpeed = _previewSpeed;
                _persist.SaveSpeed(_originalSpeed);
            }, _speedBar, _speedUpButton, _speedApplyButton, _speedMaxText));
        _healthApplyButton.onClick.AddListener(() => ApplyUpgrade(_previewHealth, _originalHealth, _config.MaxHealth,
            newValue =>
            {
                _stats.UpgradeHealth(newValue);
                _originalHealth = _previewHealth;
                _config.CurrentBaseHealth = _originalHealth;
                _healthBarView.UpdateTrack(_previewHealth);
                _persist.SaveHealth(_originalHealth);
            }, _healthBar, _healthUpButton, _healthApplyButton, _healthMaxText));
        _damageApplyButton.onClick.AddListener(() => ApplyUpgrade(_previewDamage, _originalDamage, _config.MaxDamage,
            newValue =>
            {
                _stats.UpgradeDamage(newValue);
                _config.CurrentBaseDamage = _originalDamage = _previewDamage;
                _persist.SaveDamage(_originalDamage);
            }, _damageBar, _damageUpButton, _damageApplyButton, _damageMaxText));

        _menu.MenuOpened += RefreshBars;
        _menu.MenuClosed += RevertAll;

        RefreshBars();
    }

    private void Update()
    {
        if (Input.GetKeyDown(_resetKey))
            OnReset();
    }
    
    private void OnReset()
    {
        _persist.ResetAll();
        
        _stats.Speed.Value = _config.CurrentBaseSpeedMovement;
        _stats.HealthMax.Value = _config.CurrentBaseHealth;
        _stats.Damage.Value = _config.CurrentBaseDamage;

        _healthBarView.UpdateTrack(_config.CurrentBaseHealth);
        
        _originalSpeed = _previewSpeed = _config.CurrentBaseSpeedMovement;
        _originalHealth = _previewHealth = _config.CurrentBaseHealth;
        _originalDamage = _previewDamage = _config.CurrentBaseDamage;

        RefreshBars();
    }

    private void RefreshBars()
    {
        _healthBarView.UpdateTrack(_originalHealth);
        
        _speedBar.size = _originalSpeed / _config.MaxSpeed;
        _healthBar.size = _originalHealth / _config.MaxHealth;
        _damageBar.size = _originalDamage / _config.MaxDamage;

        _speedValue.text = _originalSpeed.ToString("F1");
        _healthValue.text = _originalHealth.ToString("F1");
        _damageValue.text = _originalDamage.ToString("F1");

        if (_originalSpeed >= _config.MaxSpeed)
            ShowMaxState(_speedBar, _speedUpButton, _speedApplyButton, _speedMaxText);

        if (_originalHealth >= _config.MaxHealth)
            ShowMaxState(_healthBar, _healthUpButton, _healthApplyButton, _healthMaxText);

        if (_originalDamage >= _config.MaxDamage)
            ShowMaxState(_damageBar, _damageUpButton, _damageApplyButton, _damageMaxText);
    }

    private void StageUpgrade(float delta, ref float preview, float maximum, UpgradeManager upgrades, Scrollbar bar,
        TextMeshProUGUI valueText, Button applyButton)
    {
        if (preview + delta > maximum)
            return;

        if (!upgrades.SpendPoint())
            return;

        preview += delta;

        bar.size = preview / maximum;
        valueText.text = preview.ToString("F1");
        applyButton.interactable = true;
    }


    private void ApplyUpgrade(float preview, float original, float maximum, Action<float> commit, Scrollbar bar,
        Button upButton, Button applyButton, TextMeshProUGUI maxText)
    {
        float delta = preview - original;

        commit(delta);
        applyButton.interactable = false;

        if (Mathf.Approximately(preview, maximum))
            ShowMaxState(bar, upButton, applyButton, maxText);
    }

    private void ShowMaxState(Scrollbar bar, Button upButton, Button applyButton, TextMeshProUGUI maxText)
    {
        bar.gameObject.SetActive(false);
        upButton.gameObject.SetActive(false);
        applyButton.gameObject.SetActive(false);
        maxText.gameObject.SetActive(true);
    }

    private void RevertAll()
    {
        int refund = Mathf.RoundToInt((_previewSpeed - _originalSpeed) / 1f)
                     + Mathf.RoundToInt((_previewHealth - _originalHealth) / 10f)
                     + Mathf.RoundToInt((_previewDamage - _originalDamage) / 1f);

        _upgrades.Points.Value += refund;

        _previewSpeed = _originalSpeed;
        _previewHealth = _originalHealth;
        _previewDamage = _originalDamage;

        _speedApplyButton.interactable = false;
        _healthApplyButton.interactable = false;
        _damageApplyButton.interactable = false;

        RefreshBars();
    }
}
using UnityEngine;
using TMPro;
using UniRx;
using Zenject;

public class UpgradeUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _pointsText;
    
    private UpgradeManager _upgrades;

    [Inject]
    public void Construct(UpgradeManager upgrades)
    {
        _upgrades = upgrades;
    }

    private void Start()
    {
        _upgrades.Points.Subscribe(point => _pointsText.text = $"Points: {point}").AddTo(this);
    }
}
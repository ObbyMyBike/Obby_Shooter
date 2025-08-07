using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public event Action MenuOpened;
    public event Action MenuClosed;
        
    [SerializeField] private Canvas _menuCanvas;
    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private KeyCode _openKey;

    [Header("Sections")]
    [SerializeField] private List<Button> _selectionButtons;
    [SerializeField] private List<GameObject> _sectionPanels;
    
    private bool _isOpen;

    private void Awake()
    {
        if (_openButton != null)
            _openButton.onClick.AddListener(ToggleMenu);
        
        if (_closeButton != null)
            _closeButton.onClick.AddListener(CloseMenu);

        for (int i = 0; i < _selectionButtons.Count; i++)
        {
            int index = i;
            _selectionButtons[i].onClick.AddListener(() => ShowPanel(index));
        }
        
        _menuCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        #if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetKeyDown(_openKey))
            ToggleMenu();
        #endif
    }

    private void ToggleMenu()
    {
        if (_isOpen)
            CloseMenu();
        else
            OpenMenu();
    }

    private void ShowPanel(int index)
    {
        for (int i = 0; i < _sectionPanels.Count; i++)
            _sectionPanels[i].SetActive(i == index);
    }
    
    private void OpenMenu()
    {
        _isOpen = true;
        _menuCanvas.gameObject.SetActive(true);
        
        Time.timeScale = 0f;
        
#if UNITY_STANDALONE || UNITY_EDITOR
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
#endif
        
        ShowPanel(0);
        
        MenuOpened?.Invoke();
    }

    private void CloseMenu()
    {
        _isOpen = false;
        _menuCanvas.gameObject.SetActive(false);
        
        Time.timeScale = 1f;
        
#if UNITY_STANDALONE || UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
#endif
        
        MenuClosed?.Invoke();
    }
}
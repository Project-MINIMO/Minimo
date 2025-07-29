using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using LeTai.Asset.TranslucentImage;

public class UIManager : ManagerBase
{
    [SerializeField] private GameObject _blurImg;
    [SerializeField] private GameObject _produceBlurImg;
    [SerializeField] private GetItemPanel _getItem;
    public GetItemPanel GetItem => _getItem;
    
    public bool IsOnlyDefaultPanelsInStack =>
        _uiStack.Count > 0 &&
        _uiStack.All(panel => panel.IsDefaultPanel);
    
    private Dictionary<Type, UIBase> _uiDictionary;
    private Stack<UIBase> _uiStack;

    protected override void Awake()
    {
        base.Awake();

        var uiPanels = GetComponentsInChildren<UIBase>(true);

        _uiDictionary = new(uiPanels.Length);
        _uiStack = new(uiPanels.Length);

        _uiDictionary = uiPanels.ToDictionary(p => p.GetType(), p => p);

        _blurImg.GetComponent<TranslucentImage>().source = Camera.main.GetComponent<TranslucentImageSource>();
    }

    private void Start()
    {
        foreach (var panel in _uiDictionary.Values)
        {
            try
            {
                panel.gameObject.SetActive(true);
                
                panel.Initialize(this); 
                
                if (panel.IsDefaultPanel)
                {
                    _uiStack.Push(panel);
                }
                else
                {
                    panel.gameObject.SetActive(false);
                }
            }
            catch (Exception error)
            { Debug.LogError($"ERROR: {error.Message}\n{error.StackTrace}"); }
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }
    }

    #region Get Panel
    public T GetPanel<T>() where T : UIBase
    {
        if (_uiDictionary.TryGetValue(typeof(T), out var panel))
        {
            return panel as T;
        }

        Debug.LogError($"Panel of type {typeof(T)} not found.");
        return null;
    }
    #endregion

    #region Manage Stack
    public void PushPanel(UIBase panel)
    {
        if (_uiStack.Count > 0 && _uiStack.Peek() == panel) return;
        
        foreach (var peek in _uiStack)
        {
            peek.Hide(false);
        }
        
        _blurImg.SetActive(panel.IsUseBlur);
        _produceBlurImg.SetActive(panel.IsUseProduceBlur);
        panel.Show(true);
        _uiStack.Push(panel);
    }
    
    public void PopPanel(UIBase panel)
    {
        if (_uiStack.Count == 0 || _uiStack.Peek() != panel) return;

        _uiStack.Pop();
        panel.Hide(true);

        if (_uiStack.Count > 0)
        {
            if (_uiStack.Peek().IsDefaultPanel)
            {
                _blurImg.SetActive(false);
                _produceBlurImg.SetActive(false);
                foreach (var peek in _uiStack)
                {
                    peek.Show(false);
                }
            }
            else
            {
                var newPanel = _uiStack.Peek();
                newPanel.Show(false);
                _blurImg.SetActive(newPanel.IsUseBlur);
                _produceBlurImg.SetActive(newPanel.IsUseProduceBlur);
            }
        }
    }

    public void PopAllPanels()
    {
        while (_uiStack.Count > 0 && !_uiStack.Peek().IsDefaultPanel)
        {
            var top = _uiStack.Peek();
            top.ClosePanel();
        }
    }

    private void Back()
    {
        if (_uiStack.Count == 0) return;
        if (_uiStack.Peek().IsDefaultPanel) return;
        
        PopPanel(_uiStack.Peek());
    }
    #endregion
}

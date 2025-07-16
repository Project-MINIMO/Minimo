using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : ManagerBase
{
    [SerializeField] private Image _blackBlur;
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
        
        FadeOut(1);
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
                foreach (var peek in _uiStack)
                {
                    peek.Show(false);
                }
            }
            else
            {
                _uiStack.Peek().Show(false);
            }
        }
    }

    public void Back()
    {
        if (_uiStack.Count == 0) return;
        if (_uiStack.Peek().IsDefaultPanel) return;
        
        PopPanel(_uiStack.Peek());
    }
    #endregion
    
    #region Fade In / Out
    public void FadeIn(Action onComplete = null)
    {
        _blackBlur.gameObject.SetActive(true);

        _blackBlur.DOKill();
        _blackBlur.DOFade(1f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void FadeOut(float duration, Action onComplete = null)
    {
        _blackBlur.DOKill();
        _blackBlur.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            _blackBlur.gameObject.SetActive(false);
            onComplete?.Invoke();
        });
    }

    public void FadeInOut(float duration, Action midAction = null)
    {
        _blackBlur.gameObject.SetActive(true);

        _blackBlur.DOKill();
        _blackBlur.DOFade(1f, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            midAction?.Invoke();
            FadeOut(duration);
        });
    }
    #endregion
}

using UnityEngine;
using UnityEngine.UI;

public class ScrollCtrl : MonoBehaviour
{
    [Header("Scroll")]
    [SerializeField] ScrollRect _scrollRect;
    [SerializeField] private GameObject _scrollUpObj;
    [SerializeField] private GameObject _scrollDownObj;

    private RectTransform _contentRect;
    private RectTransform _viewportRect;

    private void Start()
    {
        _contentRect = _scrollRect.content;
        _viewportRect = _scrollRect.viewport;
    }

    private void OnEnable()
    {
        _scrollRect.verticalNormalizedPosition = 1;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (CanScrollDown())
        {
            _scrollUpObj.SetActive(false);
            _scrollDownObj.SetActive(true);
        }
        else if (CanScrollUp())
        {
            _scrollUpObj.SetActive(true);
            _scrollDownObj.SetActive(false);
        }
        else
        {
            _scrollUpObj.SetActive(false);
            _scrollDownObj.SetActive(false);
        }
    }

    private bool CanScrollDown()
    {
        float contentHeight = _contentRect.rect.height;
        float viewportHeight = _viewportRect.rect.height;

        return _contentRect.anchoredPosition.y < (contentHeight - viewportHeight);
    }

    private bool CanScrollUp()
    {
        return _contentRect.anchoredPosition.y > 0.01f;
    }
}

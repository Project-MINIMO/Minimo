using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SortDropdown : TMP_Dropdown
{
    private Image _labelImg;
    private Sprite _clearSprite;
    private Sprite _startSprite;
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        DisplayDropdown();
    }
    
    private void DisplayDropdown()
    {
        Show();
        
        var dropdownListRect = transform.GetChild(transform.childCount - 1).GetComponent<RectTransform>();
        var contentRect = dropdownListRect.GetComponent<ScrollRect>().content;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
        dropdownListRect.sizeDelta = new Vector2(dropdownListRect.sizeDelta.x, contentRect.sizeDelta.y);
    }
    
    protected override GameObject CreateDropdownList(GameObject template)
    {
        _labelImg.sprite = _clearSprite;
        
        return base.CreateDropdownList(template);
    }

    protected override void DestroyDropdownList(GameObject dropdownList)
    {
        base.DestroyDropdownList(dropdownList);

        _labelImg.sprite = _startSprite;
    }

    public void SetLabel(Image label, Sprite sprite)
    {
        _labelImg = label;
        _clearSprite = sprite;
        _startSprite = _labelImg.sprite;
    }
}
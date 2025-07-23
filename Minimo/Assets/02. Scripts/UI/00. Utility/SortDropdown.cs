using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SortDropdown : TMP_Dropdown
{
    private Image _labelImg;
    private Sprite _clearSprite;
    private Sprite _startSprite;
    
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
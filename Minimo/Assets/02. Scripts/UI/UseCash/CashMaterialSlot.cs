using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CashMaterialSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _countTMP;
    [SerializeField] private Image _iconImage;

    public void SetData(Item material, int count)
    {
        _nameTMP.text = material.Name;
        _countTMP.text = count.ToString();
        _iconImage.sprite = null;//material.Icon;
        
        gameObject.SetActive(true);
    }

    public void SetNull()
    {
        gameObject.SetActive(false);
    }
}

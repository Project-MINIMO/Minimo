using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CashMaterialSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameTMP;
    [SerializeField] private TextMeshProUGUI _countTMP;
    [SerializeField] private Image _iconImage;
    
    private TitleData _titleData;

    private void Awake()
    {
        _titleData = App.GetData<TitleData>();
    }
    
    public void SetData(ItemData material, int count)
    {
        _nameTMP.text = _titleData.GetString(material.Name);
        _countTMP.text = count.ToString();
        _iconImage.sprite = null;//material.Icon;
        
        gameObject.SetActive(true);
    }

    public void SetNull()
    {
        gameObject.SetActive(false);
    }
}

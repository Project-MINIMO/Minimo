using TMPro;
using UnityEngine;

public class GoldInfoUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private readonly string _goldInfo = $"돈 : {0} / 캐시 : {1}";

    private void Update()
    {
        _text.SetText(string.Format(_goldInfo, AccountInfo.Instance.Gold.Count, AccountInfo.Instance.Cash));
    }
}

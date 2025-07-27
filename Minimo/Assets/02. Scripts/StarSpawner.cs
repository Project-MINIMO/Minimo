using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarSpawner : MonoBehaviour
{
    [SerializeField] private Image _fillImg;
    [SerializeField] private GameObject _starObj;

    private void Update()
    {
        _fillImg.fillAmount = AccountInfo.Instance.Star / 100f;

        if (AccountInfo.Instance.Star >= 100)
        {
            AccountInfo.Instance.Star = 0;
            Instantiate(_starObj);
        }
    }
}

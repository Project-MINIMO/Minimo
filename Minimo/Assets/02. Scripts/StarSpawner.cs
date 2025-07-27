using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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
            Instantiate(_starObj, new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0), Quaternion.identity);
        }
    }
}

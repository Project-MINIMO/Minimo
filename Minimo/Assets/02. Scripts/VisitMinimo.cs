using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

public class VisitMinimo : MonoBehaviour
{
    [SerializeField] private Image _itemImg;
    
    [SerializeField] private GameObject _bubbleObj;
    [SerializeField] private Button _bubbleBtn;
    
    [SerializeField] private TextMeshProUGUI _rewardTxt;
    
    private List<Item> _items = new List<Item>();
    private Item _currentItem;
    private bool _questCompleted;
    private Coroutine _coroutine;
    
    private void Awake()
    {
        _items = App.GetData<TitleData>().Item.Values.Where(x => x.Level == 2).ToList();
        
        _bubbleObj.SetActive(false);
        _bubbleBtn.onClick.AddListener(OnBubbleClicked);

        _coroutine = StartCoroutine(BubbleLoop());
        _rewardTxt.gameObject.SetActive(false);
    }

    private void OnBubbleClicked()
    {
        if (_questCompleted) return;

        if (_currentItem.Count > 0)
        {
            _currentItem.AddCount(-1);
            _questCompleted = true;
            StopCoroutine(_coroutine);
            StartCoroutine(EndQuest());
        }
    }

    private IEnumerator BubbleLoop()
    {
        while (!_questCompleted)
        {
            _rewardTxt.gameObject.SetActive(false);
            _questCompleted = false;
            _currentItem = _items[Random.Range(0, _items.Count)];
            _itemImg.sprite = _currentItem.Icon;
            _bubbleObj.SetActive(true);
            yield return new WaitForSeconds(5f);
            
            _bubbleObj.SetActive(false);
            yield return new WaitForSeconds(3);
        }
    }
    
    private IEnumerator EndQuest()
    {
        var reward = Random.Range(8, 25);
        _rewardTxt.gameObject.SetActive(true);
        _rewardTxt.text = reward.ToString();
        AccountInfo.Instance.Star += reward;
        
        _bubbleObj.SetActive(false);
        yield return new WaitForSeconds(3);
        
        _questCompleted = false;
        _coroutine = StartCoroutine(BubbleLoop());
    }
}

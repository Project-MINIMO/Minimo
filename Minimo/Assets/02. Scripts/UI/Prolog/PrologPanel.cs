using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PrologPanel : MonoBehaviour
{
    [SerializeField] private PrologBase[] _prologBases;

    private bool _isKeyDownZ = false;
    
    private void Start()
    {
        StartCoroutine(ShowProlog());
    }

    private void Update()
    {
        if (_isKeyDownZ) return;
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            _isKeyDownZ = true;
            
            StopAllCoroutines();
            App.LoadScene(SceneName.Game);
        }
    }

    private IEnumerator ShowProlog()
    {
        foreach (var prolog in _prologBases)
        {
            prolog.StartProlog();
            yield return new WaitUntil(() => prolog.IsEnd);
        }
        
        App.LoadScene(SceneName.Game);
    }
}

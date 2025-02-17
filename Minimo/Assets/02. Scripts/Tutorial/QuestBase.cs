using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class QuestBase : MonoBehaviour
{
    [SerializeField] private Button _clearBtn;
    
    public abstract string ID { get; }
    protected abstract bool IsClear { get; }
    
    public virtual void StartQuest()
    {
        gameObject.SetActive(true);
        _clearBtn.onClick.AddListener(EndQuest);
        _clearBtn.gameObject.SetActive(false);
        
        StartCoroutine(WaitForClear());
    }
    
    protected virtual void ClearQuest()
    {
        EndQuest();
    }
    
    private void EndQuest()
    {
        gameObject.SetActive(false);
        App.GetManager<QuestManager>().EndQuest(ID);
    }
    
    private IEnumerator WaitForClear()
    {
        yield return new WaitUntil(() => IsClear);
        
        ClearQuest();
    }
}

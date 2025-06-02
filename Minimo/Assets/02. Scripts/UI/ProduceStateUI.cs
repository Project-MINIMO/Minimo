using UnityEngine;

public class ProduceStateUI : MonoBehaviour
{
    [SerializeField] private GameObject _produceBack;
    [SerializeField] private GameObject _completeBack;

    public void ShowUI(ProduceState state)
    {
        switch (state)
        {
            case ProduceState.Idle:
                _produceBack.SetActive(false);
                _completeBack.SetActive(false);
                break;
            
            case ProduceState.Produce:
                _produceBack.SetActive(true);
                _completeBack.SetActive(false);
                break;
            
            case ProduceState.Complete:
                _produceBack.SetActive(false);
                _completeBack.SetActive(true);
                break;
        }
    }
}

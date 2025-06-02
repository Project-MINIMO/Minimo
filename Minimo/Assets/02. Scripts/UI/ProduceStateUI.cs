using UnityEngine;

public class ProduceStateUI : MonoBehaviour
{
    [SerializeField] private GameObject _produceBack;
    [SerializeField] private GameObject _completeBack;

    private ProduceObject _object;
    private float _lastUpdateTime;
    
    private void Start()
    {
        _object = GetComponent<ProduceObject>();
        _lastUpdateTime = Time.time;
    }
    
    private void Update()
    {
        if (Time.time - _lastUpdateTime < 0.1f) return;

        _lastUpdateTime = Time.time;

        if (_object.AllTasks.Count == 0)
        {
            ShowUI(ProduceState.Idle);
        }
        else if (_object.ActiveTask != null)
        {
            ShowUI(ProduceState.Produce);
        }
        else
        {
            ShowUI(ProduceState.Complete);
        }
    }
    
    private void ShowUI(ProduceState state)
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

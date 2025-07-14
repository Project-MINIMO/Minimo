public class ProduceManager : ManagerBase
{
    public ProduceObject CurrentProduceObject { get; private set; }
    
    public void ActiveProduce(ProduceObject produceObject)
    {
        if (CurrentProduceObject && CurrentProduceObject != produceObject)
        {
            CurrentProduceObject.CloseUI();
        }
        
        CurrentProduceObject = produceObject;
        CurrentProduceObject.OpenUI();
    }
    
    public void DeactiveProduce()
    {
        CurrentProduceObject.CloseUI();
        CurrentProduceObject = null;
    }
}

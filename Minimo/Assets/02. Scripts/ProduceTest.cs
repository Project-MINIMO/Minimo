using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public class ProduceTest : MonoBehaviour
{
    public Dictionary<EBuilding, ProduceData> ProduceDatas;
    private TitleData _titleData;

    private void Start()
    {
        ProduceDatas = App.GetData<TitleData>().Produce;
    }
    
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            //_titleData.SetProduceData(ProduceDatas);
        }
    }
}
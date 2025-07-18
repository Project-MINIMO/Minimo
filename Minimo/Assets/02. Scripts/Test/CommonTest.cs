using System.Collections.Generic;

using UnityEngine;
using UnityEditor;


public class CommonTest : MonoBehaviour
{
    public Dictionary<string, int> CommonDatas;
    private TitleData _titleData;
    
    private void Start()
    {
        CommonDatas = App.GetData<TitleData>().Common;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            //_titleData.SetCommonData(CommonDatas);
        }
    }
}



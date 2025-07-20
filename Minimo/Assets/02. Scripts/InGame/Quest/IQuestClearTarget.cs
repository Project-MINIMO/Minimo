using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestClearTarget
{
    int ID { get; }
    string Name { get; }
    int Count { get; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccountInfo : Singleton<AccountInfo>
{
    public Dictionary<ItemData, int> items = new();
    public int level { get; private set; } = 2;
}

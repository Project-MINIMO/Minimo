using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : Singleton<OrderManager>
{
    public List<int> OrderItems = new List<int>();
}

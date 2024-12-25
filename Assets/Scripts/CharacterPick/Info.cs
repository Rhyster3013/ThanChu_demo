using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info
{
    public int Order { get; set; }
    public string PlayerName { get; set; }
    public bool IsReady { get; set; }

    public Info(int order, string playerName, bool isReady)
    {
        Order = order;
        PlayerName = playerName;
        IsReady = isReady;
    }
}

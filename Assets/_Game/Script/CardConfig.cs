using UnityEngine;
using System;
[Serializable]
public class CardConfig
{
    public int id;
    public Sprite sprite;

    [Tooltip("Base payout x100 for 3-of-a-kind")]
    public int value; 
    public int weight = 1;
}

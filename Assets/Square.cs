using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    public Player player;
    public Tuple<int, int> Position;
    public GameObject symbol;

    public void Clear()
    {
        player = null;
        Destroy(symbol);
    }
}

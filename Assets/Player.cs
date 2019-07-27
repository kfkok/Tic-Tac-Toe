using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public int id;
    public string playerName;
    public Color color;
    public GameObject symbol;
    protected Tuple<int, int> selectedSquarePosition;

    public abstract IEnumerator SelectingSquare(State state);

    protected abstract bool SquareSelected();

    public abstract Tuple<int, int> GetSelectedSquarePosition();
}

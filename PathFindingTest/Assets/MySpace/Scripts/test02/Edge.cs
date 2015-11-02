using UnityEngine;
using System.Collections;


[System.Serializable]
public class Edge
{
    public Int2 nodes;
    public int cost;
}

[System.Serializable]
public class Int2
{
    public int p, q;
}

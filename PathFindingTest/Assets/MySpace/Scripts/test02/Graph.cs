using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Graph : MonoBehaviour
{
    public Node goal;
    public Node[] nodes;
    public Edge[] edges;



    /// <summary>
    /// Return actual cost between 2 nodes
    /// </summary>
    /// <param name="current"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public int Cost(Node current, Node next)
    {
        return 1;
    }

    public int Heuristic(Node goal, Node r)
    {
        return Mathf.FloorToInt((r.transform.position - goal.transform.position).magnitude);
    }

    public IEnumerable<Node> Neighbors(Node n)
    {
        throw new NotImplementedException();
    }
}

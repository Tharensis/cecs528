using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : System.IComparable<Node>
{
    int id;

    public Vector3 position;
    
    public List<Edge> edgelist = new List<Edge>();
    public List<Node> NextNodes = new List<Node>();

	public Node path = null;
	
    // Fields for AStar search from start node to end node
    public float g;     // cost(start, this-node) 
    public float h;     // heuristic_cost(this_node, end)
    public float f;     // f = g + h
	public Node cameFrom;

    // for djikstra search
    public bool visited = false;
    public float dist = Single.MaxValue; // ie., infinity

    public Color color = Color.yellow;
    public float dotSize = 0.01f;

	public Node(int i, Vector3 pos, float heu = 0)
	{
		id = i;
        position = pos;
        h = heu;
		path = null;
	}

    public Node(int i, float x, float y, float z, float heu = 0)
    {
        id = i;
        position = new Vector3(x, y, z);
        h = heu;
        path = null;
    }
	
	public int ID
	{
        get { return id; }	
	}

    public Vector3 Position
    {
        get { return position; }
    }

    public void Clear()
    {
        edgelist.Clear();
        NextNodes.Clear();
    }

    public int CompareTo(Node obj)
    {
        if (this.f < obj.f)
            return -1;
        else if (this.f > obj.f)
            return 1;
        else
            return 0;
    }

}

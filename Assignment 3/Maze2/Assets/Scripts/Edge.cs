using UnityEngine;
using System.Collections;

public class Edge
{
	public Node startNode;
	public Node endNode;
    public float Weight = 0;
    public Color color = Color.yellow;
	
	public Edge(Node from, Node to)
	{
		startNode = from;
		endNode = to;
	}
}

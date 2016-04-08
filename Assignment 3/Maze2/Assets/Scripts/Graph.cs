using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
//  Undirected graph
//
public class Graph
{
	public List<Edge> edges = new List<Edge>();
	public List<Node> nodes = new List<Node>();
	public List<Node> pathList = new List<Node>();
	
	public Graph(){}

    #region Utility methods

    public void Clear()
    {
        edges.Clear();

        foreach (Node node in nodes)
            node.Clear();
        nodes.Clear();

        pathList.Clear();
    }

	public void AddNode(int id, Vector3 pos)
	{
        if (FindNode(id) != null) return;

        Node node = new Node(id, pos);
		nodes.Add(node);
	}

    public void AddNode(Node node)
    {
        if (FindNode(node.ID) != null) return;  // Duplicate node

        nodes.Add(node);
    }

	
	public void AddEdge(int fromNode, int toNode, float w)
	{
		Node from = FindNode(fromNode);
		Node to = FindNode(toNode);
		
		if(from != null && to != null)
		{
			Edge e = new Edge(from, to);
            e.Weight = w;
			edges.Add(e);
			from.edgelist.Add(e);
            to.edgelist.Add(e);     // Assume undirected graph

            from.NextNodes.Add(to);
            to.NextNodes.Add(from);
		}	
	}
	
	public Node FindNode(int id)
	{
		foreach (Node n in nodes) 
		{
			if(n.ID == id)
				return n;
		}
		return null;
	}

    public Edge FindEdge(int node1, int node2)
    {
        foreach (Edge e in edges)
        {
            if ((e.startNode.ID == node1 && e.endNode.ID == node2) ||
                (e.startNode.ID == node2 && e.endNode.ID == node1))
                return e;
        }

        return null;
    }

    public int getPathLength()
	{
		return pathList.Count;	
	}
	
	public int getPathPoint(int index)
	{
		return pathList[index].ID;
	}
	
	public void printPath()
	{
		foreach(Node n in pathList)
		{	
			Debug.Log(n.ID);	
		}
	}

    #endregion Utility methods

    #region Path search methods

    public Node[] DepthFirstSearch(int startId, int endId)
    {
        Node start = FindNode(startId);
        Node end = FindNode(endId);

        if (start == null || end == null) return null;

        Stack<Node> stack = new Stack<Node>();
        //Dictionary<Node, bool> visited = new Dictionary<Node, bool>();
        Node currentNode;
        List<Node> nodesToMove = new List<Node>();
        System.Random rand = new System.Random((int)DateTime.Now.Ticks);

        foreach (Node node in nodes)
            node.visited = false;

        currentNode = start;
        currentNode.visited= true;
        int nvisited = 1;       // for performance measure
      
        while (currentNode != end)
        {
            nodesToMove.Clear();
            foreach (Node node in currentNode.NextNodes)
            {
                if (!node.visited)
                    nodesToMove.Add(node);
            }
            int n = nodesToMove.Count;
            if (n > 0)
            {
                stack.Push(currentNode);
                currentNode.visited = true;

                // Randomly select a node to move
                currentNode = nodesToMove[rand.Next() % n];
                currentNode.visited = true;
                nvisited++;
            }
            else // If no nodes to move (i.e. this currentNode is not on the solution path), 
                 // pop the last saved currentNode if stack is non-empty.
            {
                if (stack.Count == 0) break; // no solution path 
                   // (i.e. start and end are not in the same component of the graph).
          
                currentNode = stack.Pop();
                if (!currentNode.visited)
                {
                    currentNode.visited = true;
                    nvisited++;
                }
            }
        } // end of while

        Node[] path = null;

        if (stack.Count > 0)
        {
            // Note the end node is not in the stack and 
            // the soluition path in the stack is in reversed order 
            // (i.e. traced back from the end node).
            path = new Node[stack.Count + 1];
            int count = stack.Count;
            for (int i = 0, j = count-1; i < count; i++)
                path[j--] = stack.Pop();
            path[path.Length - 1] = end;
        }

        Debug.Log("DFS : Number of nodes visited = " + nvisited.ToString());
        Debug.Log("DFS : Number of nodes in DFS path = " + path.Length.ToString());

        return path;
    }

    public Node[] DijkstraSearch(int startId, int endId)
    {
        Node start = FindNode(startId);
        Node end = FindNode(endId);
        if (start == null || end == null) return null;

        List<Node> Q = new List<Node>();
        float w;
        int nvisited = 1; // for performance measure purpose 

        resetNodesForSearch(end);

        // process start first
        start.visited = true;
        start.dist = 0;
        Q.Add(start);

        // Continue to find shortest path (start, end) while Q is not empty;
        // Return path if found in the while loop.
        while (Q.Count > 0)
        {
            // Find the next node to process (node in Q with lowest cost)
            float lowest_cost = Single.MaxValue; // ie., infinity
            Node currentNode = null;
            foreach (Node n in Q)
            {
                if (n.dist < lowest_cost)
                {
                    currentNode = n;
                    lowest_cost = n.dist;
                }
            }
            Q.Remove(currentNode);

            if (currentNode == end) // shortest path from start to end is found
            {
                reconstructPath(start, end);

                Debug.Log("Dijkstra : Number of nodes visited = " + nvisited.ToString());
                Debug.Log("Dijkstra : Number of nodes in Dijkstra path = " + pathList.Count.ToString());

                // Maze expects an array, not a list, so convert it
                return pathList.ToArray();
            }

            foreach (Node neighbor in currentNode.NextNodes)
            {
                w = FindEdge(currentNode.ID, neighbor.ID).Weight;

                if (!neighbor.visited)
                {
                    neighbor.visited = true;
                    neighbor.dist = currentNode.dist + w;
                    neighbor.cameFrom = currentNode;
                    Q.Add(neighbor);
                    nvisited++;
                }
                else if (Q.Contains(neighbor))
                // Relaxing: if a path going to a neighbor through the current node 
                // is cheaper than any previously found path to the neighbor, 
                // update the neighbor's distance to reflect new better path
                {
                    if (currentNode.dist + w < neighbor.dist)
                    {
                        neighbor.dist = currentNode.dist + w;
                        neighbor.cameFrom = currentNode;
                    }
                }
            }    
        }

        // if reach this point, shorest path from start to end is not found
        Debug.Log("Dijkstra : not path from start to end is found.");

        return null;
    }
/*
    public Node[] DijkstraSearch2(int startId, int endId)
    {
        Node start = FindNode(startId);
        Node end = FindNode(endId);
        if (start == null || end == null) return null;

        float w;
        int nvisited = 0; // for performance measure purpose 

        resetNodesForSearch(end);
        
        // process start first
        Node currentNode = start;
        currentNode.dist = 0;

        // stop when end is found
        while (currentNode != end)
        {
            // mark current node as visited
            currentNode.visited = true;

            // Relaxing: if a path going to a neighbor through the current node is cheaper
            // than any previously found path to the neighbor, 
            // update the neighbor's distance to reflect new better path
            foreach (Node neighbor in currentNode.NextNodes)
            {
                nvisited++;
                w = FindEdge(currentNode.ID, neighbor.ID).Weight;
                if (currentNode.dist + w < neighbor.dist)
                {
                    neighbor.dist = currentNode.dist + w; // cost to visit this node = cost to visit prev + 1
                    neighbor.cameFrom = currentNode;
                }
            }
            // find the next node to visit (lowest cost, unvisited)
            float lowest_cost = Single.MaxValue; // ie., infinity
            Node next_node = null;
            foreach (Node n in nodes)
            {
                if (!n.visited && n.dist < lowest_cost)
                {
                    next_node = n;
                    lowest_cost = n.dist;
                }
            }
            currentNode = next_node;
        }

        reconstructPath(start, end);

        Debug.Log("Dijkstra : Number of nodes visited = " + nvisited.ToString());
        Debug.Log("Dijkstra : Number of nodes in Dijkstra path = " + pathList.Count.ToString());

        // Maze expects an array, not a list, so convert it
        return pathList.ToArray(); ;
    }
*/
     public Node[] AStar(int startId, int endId)
	{
	  	Node start = FindNode(startId);
	  	Node end = FindNode(endId);
	  
	  	if(start == null || end == null)
	  		return null;

        int nvisited = 0; // for performance measure purpose 

        resetNodesForSearch(end);

	  	List<Node>	open = new List<Node>();
	  	List<Node>	closed = new List<Node>();
	  	float g_score= 0;

        start.g = 0;
	  	start.f = start.h;
	  	open.Add(start);
	  	
	  	while(open.Count > 0)
	  	{
			Node thisnode = lowestF(open);

			if(thisnode == end)  //path found
			{
				reconstructPath(start, end);

                Debug.Log("AStar : Number of nodes visited = " + nvisited.ToString());
                Debug.Log("AStar : Number of nodes in AStar path = " + pathList.Count.ToString());

				return pathList.ToArray();	
			} 	
			
			open.Remove(thisnode);
			closed.Add(thisnode);
			
            foreach (Node neighbour in thisnode.NextNodes)
			{
                if (!neighbour.visited)
                {
                    neighbour.visited = true;
                    nvisited++;

				    neighbour.g = thisnode.g + FindEdge(thisnode.ID, neighbour.ID).Weight;
                    neighbour.f = neighbour.g + neighbour.h;

                    neighbour.cameFrom = thisnode;
                    open.Add(neighbour);
                }
                else // Relaxzation
                {
                    g_score = thisnode.g + FindEdge(thisnode.ID, neighbour.ID).Weight;
				
				    if (g_score < neighbour.g) // a better path from start to neighbour is found
				    {
					    neighbour.g = g_score;
					    neighbour.f = neighbour.g + neighbour.h;
                        neighbour.cameFrom = thisnode;

                        if (closed.Contains(neighbour))
                        {
                            closed.Remove (neighbour);
                            open.Add(neighbour);
                        }   
				    }
			    }
            }
	  	}

        Debug.Log("AStar failed");
		return null;	
	}

    Node lowestF(List<Node> L)
    {
        float lowestF = Single.MaxValue;
        Node lowestFNode = null;

        foreach (Node node in L)
        {
            if (node.f < lowestF)
            {
                lowestFNode = node;
                lowestF = node.f;
            }
        }

        return lowestFNode;
    }

	public void reconstructPath(Node start, Node end)
	{
		pathList.Clear();

        // Trace path from end to start through "cameFrom" links	
		Node p = end;
		while(p != start && p != null)
		{
            pathList.Insert(0, p); ;
			p = p.cameFrom;	
		}
		pathList.Insert(0,start);
	}

    void resetNodesForSearch(Node end)
    {
        foreach (Node node in nodes)
        {
            node.g = Single.MaxValue;
            node.h = distance (node, end);
            node.f = node.g;
            node.visited = false;
            node.dist = Single.MaxValue;
            node.cameFrom = null;
        }
    }

    float distance(Node a, Node b)
    {
	  float dx = a.Position.x - b.Position.x;
	  float dy = a.Position.y - b.Position.y;
	  float dz = a.Position.z - b.Position.z;
	  float dist = dx*dx + dy*dy + dz*dz;
	  return( dist );
    }

    #endregion Path search methods

    void printPath(Node[] path)
    {
        if (path == null)
        {
            Debug.Log("No path");
            return;
        }

        Debug.Log("path length = " + path.Length.ToString());
        for (int i = 0; i < path.Length; i++)
        {
            Debug.Log(path[i].ID);
        }
    }

    public void debugDraw()
    {
      	//draw edges
    	for (int i = 0; i < edges.Count; i++)
	  	{
    		Debug.DrawLine(edges[i].startNode.Position, edges[i].endNode.Position, Color.red);
    		Vector3 to = (edges[i].startNode.Position - edges[i].endNode.Position) * 0.05f;
    		Debug.DrawRay(edges[i].endNode.Position, to, Color.blue);

	  	}	
    }
}

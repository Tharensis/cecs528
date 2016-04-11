using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class SearchAnimation : MonoBehaviour 
{
    public float speed = 0.5f;
    public float rotationSpeed = 10.0f;
    public float accuracy = 0.01f;

    Graph graph;
    Node start, end;

    List<Node> waypoints = new List<Node>();
    int nvisited;       // for performance measure

    public void setSpeed(float newSpeed)
    {
		speed = newSpeed;
    }

    public void setRotation(float newRotation)
    {
		rotationSpeed = newRotation;
    }

    #region DFS Animation

    public bool DFSAnimation(Graph g, int sID, int eID)
    {
        if (g == null || g.nodes == null || g.nodes.Count == 0) return false;
        graph = g;
        start = graph.FindNode(sID);
        end = graph.FindNode(eID);

        if (start == null || end == null) return false;

        // Set player in start node position
        transform.position = start.Position + new Vector3(-0.5f, 0, 0);

        ResetGraphForSearch();      
        StartCoroutine(DoDFSAnimation());

        return true;
    }

    IEnumerator DoDFSAnimation()
    {
        yield return 0;     // make sure Unity has updated the graph drawing

        Stack<Node> stack = new Stack<Node>();

        System.Random rand = new System.Random((int)DateTime.Now.Ticks);

        Node currentNode;
        currentNode = start;
        currentNode.visited = true;
        waypoints.Add(currentNode);
        nvisited = 1;                   // for performance measure

        Node nextNode = null;
        Node backtrackNode = null;
        List<Node> nodesToMove = new List<Node>();

        // Do animation to move the palyer from cuurent position to start node position
        yield return new WaitForSeconds(1.0f);
        Vector3 direction = start.Position - transform.position;
        while (Vector3.Distance(transform.position, start.Position) > accuracy)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
            transform.Translate(0, 0, speed * Time.deltaTime);

            yield return 0;     // yield to Unity to re-draw
        }

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
                if (n > 1) // only need to backtrack to cuurentNode if there are other alternatives
                {
                    stack.Push(currentNode);
                    currentNode.dotSize = 0.025f;    // double the dot size for backtrack node
                }
                currentNode.visited = true;

                if (n > 1) yield return new WaitForSeconds(1.0f);
                // Randomly select a node to move to
                nextNode = nodesToMove[rand.Next() % n];
                waypoints.Add(nextNode);

                // Change edge (currentNode, nextNode) to green
                Edge edge = graph.FindEdge(currentNode.ID, nextNode.ID);
                if (edge != null) edge.color = Color.green;
                // Do animation to move the palyer from cuurentNode position to nextNode position
                direction = nextNode.Position - transform.position;
                while (Vector3.Distance(transform.position, nextNode.Position) > accuracy)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, 
                        Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
                    transform.Translate(0, 0, speed * Time.deltaTime);      

                    yield return 0;     // yield to Unity to re-draw
                }

                currentNode = nextNode;
                currentNode.visited = true;
                nvisited++;         
            }
            else // If no nodes to move (i.e. this currentNode is not on the solution path), 
            // pop the last saved currentNode if stack is non-empty.
            {
                if (stack.Count == 0) break; // no solution path 
                // (i.e. start and end are not in the same component of the graph).

                backtrackNode = stack.Pop();
                if (!backtrackNode.visited)
                {
                    backtrackNode.visited = true;
                    nvisited++;
                }

                yield return new WaitForSeconds(1.0f);

                // Do backtrack animation from currentNode to backtrackNode in the waypoints path
                // Make sure that the currentNode is the last node in the waypoints path
                if (currentNode == waypoints[waypoints.Count - 1])
                {
                    List<Node> path = new List<Node>();
                    foreach (Node node in waypoints)
                        path.Add(node);
                    yield return new WaitForSeconds(1.0f);
                    for (int i = path.Count - 1; i > 0 && path[i] != backtrackNode; --i)
                    {
                        // Change edge (path[i], path[i-1]) to red
                        Edge edge = graph.FindEdge(path[i].ID, path[i - 1].ID);
                        if (edge != null) edge.color = Color.red;

                        direction = path[i-1].Position - transform.position;
                        while (Vector3.Distance(transform.position, path[i-1].Position) > accuracy)
                        {
                            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                                                rotationSpeed * Time.deltaTime);
                            transform.Translate(0, 0, speed * Time.deltaTime);        // speed = 0.5

                            yield return 0;     // yield to Unity to re-draw
                        }
                   
                        // Remove i-th node from waypoints path
                        waypoints.RemoveAt(i);
                    }
                    currentNode = backtrackNode;
                    backtrackNode.dotSize = 0.01f;    // back to the normal node dot size;
                }
                else
                    Debug.Log("wrong waypoints");
            }

            if (currentNode == end)
            {
                Debug.Log("Find path out with " + waypoints.Count.ToString() +
                    " nodes in the path");

                // Do animation to move the palyer from end node position to exit position
                yield return new WaitForSeconds(1.0f);
                Vector3 exitPos = end.Position + new Vector3(0.5f, 0, 0);
                direction = exitPos - transform.position;
                while (Vector3.Distance(transform.position, exitPos) > accuracy)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                        Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
                    transform.Translate(0, 0, speed * Time.deltaTime);

                    yield return 0;     // yield to Unity to re-draw
                }
            }

        } // end of while

        yield break;
    }

    #endregion DFS Animation

    #region Dijkstra Animation

    public bool DijkstraAnimation(Graph g, int sID, int eID)
    {
        if (g == null || g.nodes == null || g.nodes.Count == 0) return false;
        graph = g;
        start = graph.FindNode(sID);
        end = graph.FindNode(eID);

        if (start == null || end == null) return false;

        // Set player in start node position
        transform.position = start.Position + new Vector3(-0.5f, 0, 0);

        ResetGraphForSearch();
        StartCoroutine(DoDijkstraAnimation());

        return true;
    }

    IEnumerator DoDijkstraAnimation()
    {
        yield return 0;     // make sure Unity has updated the graph drawing

        List<Node> Q = new List<Node>();
        float w;
        int nvisited = 1; // for performance measure purpose 

        // process start first
        start.visited = true;
        start.dist = 0;
        start.color = Color.red;
        start.dotSize = 0.025f;             // doub le the normal size
        Q.Add(start);

        // Continue to find shortest path (start, end) while Q is not empty;
        // break from the while loop if the end node found in the loop.
        while (Q.Count > 0)
        {
            // Find the next node to process (node in Q with lowest cost)
            float lowest_cost = Single.MaxValue; // ie., infinity
            Node currentNode = null;
            foreach (Node node in Q)
            {
                if (node.dist < lowest_cost)
                {
                    currentNode = node;
                    lowest_cost = node.dist;
                }
            }
            yield return new WaitForSeconds(1.0f);

            Q.Remove(currentNode);

            // Do animation to flash the least cost node selected
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    if (i % 2 == 0)
                        currentNode.dotSize = 0;
                    else
                        currentNode.dotSize = 0.025f;
                    yield return 0;
                }
            }
            currentNode.dotSize = 0.04f;
            currentNode.color = Color.magenta;
            yield return new WaitForSeconds(0.5f);

            if (currentNode == end) // shortest path from start to end is found
            {
                //Debug.Log("Dijkstra : Number of nodes visited = " + nvisited.ToString());

                currentNode.dotSize = 0.025f;
                currentNode.color = Color.blue;

                // Draw the path found
                Node node1 = end;
                Node node2 = end.cameFrom;
                while (node1 != start && node2 != null)
                {
                    node1.color = Color.green;
                    graph.FindEdge(node1.ID, node2.ID).color = Color.green;
                    node1 = node2;
                    node2 = node2.cameFrom;
                }
                node1.color = Color.green;
                yield break;        // End this Coroutine
            }
            else            // end not found yet
            {
                foreach (Node neighbor in currentNode.NextNodes)
                {
                    w = graph.FindEdge(currentNode.ID, neighbor.ID).Weight;

                    if (!neighbor.visited)
                    {
                        neighbor.visited = true;
                        neighbor.dist = currentNode.dist + w;
                        neighbor.cameFrom = currentNode;
                        Q.Add(neighbor);
                        neighbor.color = Color.red;
                        neighbor.dotSize = 0.025f;          // double the normal node dot size
                        nvisited++;

                        graph.FindEdge(currentNode.ID, neighbor.ID).color = Color.blue;
                        yield return new WaitForSeconds(0.5f);
                    }
                    else if (Q.Contains(neighbor))
                    // Relaxing: if a path going to a neighbor through the current node 
                    // is cheaper than any previously found path to the neighbor, 
                    // update the neighbor's distance to reflect new better path
                    {
                        if (currentNode.dist + w < neighbor.dist)
                        {
                            neighbor.dist = currentNode.dist + w;

                            // Do animation to flash the relaxing edge
                            for (int i = 0; i < 10; ++i)
                            {
                                for (int j = 0; j < 10; ++j)
                                {
                                    if (i % 2 == 0)
                                        graph.FindEdge(neighbor.ID, neighbor.cameFrom.ID).color = Color.black;
                                    else
                                        graph.FindEdge(neighbor.ID, neighbor.cameFrom.ID).color = Color.blue;
                                    yield return 0;
                                }
                            }

                            graph.FindEdge(neighbor.ID, neighbor.cameFrom.ID).color = Color.red;
                            neighbor.cameFrom = currentNode;
                            graph.FindEdge(neighbor.ID, neighbor.cameFrom.ID).color = Color.blue;

                            yield return new WaitForSeconds(1.0f);
                        }
                    }
                }
            }

            currentNode.dotSize = 0.025f;
            currentNode.color = Color.blue;

        }   // end of wile (Q.count > 0)

        // if reach this point, shorest path from start to end is not found
        //Debug.Log("Dijkstra : not path from start to end is found.");

        yield break;
    }

    #endregion Dijkstra Animation

    #region AStar Animation

    public bool AStarAnimation(Graph g, int sID, int eID, bool updateH = true)
    {
        if (g == null || g.nodes == null || g.nodes.Count == 0) return false;
        graph = g;
        start = graph.FindNode(sID);
        end = graph.FindNode(eID);

        if (start == null || end == null) return false;

        // Set player in start node position
        transform.position = start.Position + new Vector3(-0.5f, 0, 0);

        ResetGraphForSearch(updateH);
        StartCoroutine(DoAStarAnimation());

        return true;
    }

    IEnumerator DoAStarAnimation()
    {
        yield return 0;     // make sure Unity has updated the graph drawing

        nvisited = 0; // for performance measure purpose 

        List<Node> open = new List<Node>();
        List<Node> closed = new List<Node>();
        float g_score = 0;

        start.g = 0;
        start.f = start.h;
        open.Add(start);
        start.visited = true;
        start.color = Color.magenta;    // Source
        start.dotSize = 0.035f;          
        end.color = Color.cyan;         // Destination
        end.dotSize = 0.035f;

        yield return new WaitForSeconds(1.0f);

        while (open.Count > 0)
        {
            Node currentNode = lowestF(open);

            // Do animation to flash the selected least cost node
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    if (i % 2 == 0)
                        currentNode.dotSize = 0;
                    else
                        currentNode.dotSize = 0.025f;
                    yield return 0;
                }
            }

            if (currentNode == end)  //path found
            {
                // Draw the path found
                Node node1 = end;
                Node node2 = end.cameFrom;
                while (node1 != start && node2 != null)
                {
                    node1.color = Color.green;
                    graph.FindEdge(node1.ID, node2.ID).color = Color.green;
                    node1 = node2;
                    node2 = node2.cameFrom;
                    yield return new WaitForSeconds(1.0f);
                }
                node1.color = Color.green;
                yield break;        // end this Coroutine
            }

            open.Remove(currentNode);
            closed.Add(currentNode);
            currentNode.color = Color.blue;

            foreach (Node neighbor in currentNode.NextNodes)
            {
                if (!neighbor.visited)
                {
                    neighbor.visited = true;
                    nvisited++;

                    neighbor.g = currentNode.g + graph.FindEdge(currentNode.ID, neighbor.ID).Weight;
                    neighbor.f = neighbor.g + neighbor.h;

                    neighbor.cameFrom = currentNode;
                    open.Add(neighbor);
                    neighbor.color = Color.red;
                    neighbor.dotSize = 0.025f;
                    graph.FindEdge(currentNode.ID, neighbor.ID).color = Color.blue;
                    yield return new WaitForSeconds(0.5f);
                }
                else // Relaxzation
                {
                    g_score = currentNode.g + graph.FindEdge(currentNode.ID, neighbor.ID).Weight;

                    if (g_score < neighbor.g) // a better path from start to neighbour is found
                    {
                        neighbor.g = g_score;
                        neighbor.f = neighbor.g + neighbor.h;

                        if (closed.Contains(neighbor))
                        {
                            closed.Remove(neighbor);
                            open.Add(neighbor);
                        }

                        // Do animation to flash the relaxing edge
                        for (int i = 0; i < 10; ++i)
                        {
                            for (int j = 0; j < 10; ++j)
                            {
                                if (i % 2 == 0)
                                    graph.FindEdge(neighbor.ID, neighbor.cameFrom.ID).color = Color.black;
                                else
                                    graph.FindEdge(neighbor.ID, neighbor.cameFrom.ID).color = Color.blue;
                                yield return 0;
                            }
                        }

                        graph.FindEdge(neighbor.ID, neighbor.cameFrom.ID).color = Color.red;
                        neighbor.cameFrom = currentNode;
                        graph.FindEdge(neighbor.ID, currentNode.ID).color = Color.blue;

                        yield return new WaitForSeconds(1.0f);
                    }
                }
            }

            yield return new WaitForSeconds(1.0f);
        }   // End of while

        yield break;
    }

    #endregion AStar Animation

    #region Utility Methods

    public void StopSearchAnimation()
    {
        StopAllCoroutines();
    }

    public void ResetGraphForSearch(bool updateH = true)
    {
        foreach (Node node in graph.nodes)
        {
            node.g = Single.MaxValue;
            if (updateH) 
                node.h = distance(node, end);
            node.f = node.g;
            node.visited = false;
            node.dist = Single.MaxValue;
            node.cameFrom = null;
            node.color = Color.yellow;
            node.dotSize = 0.01f;

            foreach (Edge edge in graph.edges)
                edge.color = Color.yellow;
        }
    }

    float distance(Node a, Node b)
    {
        float dx = a.Position.x - b.Position.x;
        float dy = a.Position.y - b.Position.y;
        float dz = a.Position.z - b.Position.z;
        float dist = dx * dx + dy * dy + dz * dz;
        return (dist);
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

    #endregion Utility Methods

}

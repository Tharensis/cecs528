using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MazeGen;

public class ManagerGraph : MonoBehaviour 
{
    public GameObject Agent;
    public GUIStyle gs;
    public Font font;
    public Material textMat;
    
    public Button createGraphButton;
    public Button createMapGraphButton;
    public InputField startField;
    public InputField endField;
    public Button dfsAnimationButton;
    public Button dijkstraAnimationButton;
    public Button aStarAnimationButton;
    public Button pauseResumeButton;
    public Button resetButton;
    public Button exitButton;
    
    Vector3 agentPos;
    Quaternion agentOrientation; 
    Graph ourGraph = null;
    string startID = "1", endID = "8";
    int start = 1, end = 8;
    int searchAnimationType = 0; // 1 ==> DFS, 2 ==> Dijkstra, 3 ==> AStar
    bool updateH = true;
    bool isPause = false;
    Material lineMaterial;
    List<GameObject> labels;

	void Start () 
    {
        //initialize arrays
        labels = new List<GameObject>();
    
        agentPos = Agent.transform.position;
        agentOrientation = Agent.transform.rotation;

        //lineMaterial = new Material (Shader.Find ("Diffuse")); 
        //--this doesn't work well (built-in shaders do not use vertex color?)

        lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
        "SubShader { Pass {" +
        "   BindChannels { Bind \"Color\",color }" +
        "   Blend SrcAlpha OneMinusSrcAlpha" +
        "   ZWrite Off Cull Off Fog { Mode Off }" +
        "} } }");
        
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	}
    
    public void enterStartID(string id)
    {
        startID = id;
        try
        {
            start = Convert.ToInt32(id);
        }
        catch (FormatException e)
        {
            Debug.Log("Couldn't parse Start ID '" + id + "' to int!");
        }
    }
    
    public void enterEndID(string id)
    {
        endID = id;
        try
        {
            end = Convert.ToInt32(id);
        }
        catch (FormatException e)
        {
            Debug.Log("Couldn't parse End ID '" + id + "' to int!");
        }
    }

    public void createGraph()
    {
        ourGraph = new Graph();

        updateH = true;

        ourGraph.AddNode(1, new Vector3(-1.2f, 0, 1.2f));
        ourGraph.AddNode(2, new Vector3(0, 0, 1.2f));
        ourGraph.AddNode(3, new Vector3(1.2f, 0, 1.2f));
        ourGraph.AddNode(4, new Vector3(-0.6f, 0, 0));
        ourGraph.AddNode(5, new Vector3(0.6f, 0, 0));
        ourGraph.AddNode(6, new Vector3(-1.2f, 0, -1.2f));
        ourGraph.AddNode(7, new Vector3(0, 0, -1.2f));
        ourGraph.AddNode(8, new Vector3(1.2f, 0, -1.2f));

        ourGraph.AddEdge(1, 2, 4.9f);
        ourGraph.AddEdge(1, 4, 15.2f);
        ourGraph.AddEdge(1, 6, 9.3f);
        ourGraph.AddEdge(2, 3, 12.1f);
        ourGraph.AddEdge(2, 4, 6.0f);
        ourGraph.AddEdge(2, 5, 5.1f);
        ourGraph.AddEdge(3, 5, 1.7f);
        ourGraph.AddEdge(3, 8, 3.26f);
        ourGraph.AddEdge(4, 5, 7.9f);
        ourGraph.AddEdge(4, 6, 8.6f);
        ourGraph.AddEdge(4, 7, 1.15f);
        ourGraph.AddEdge(5, 7, 14.0f);
        ourGraph.AddEdge(5, 8, 26.0f);
        ourGraph.AddEdge(6, 7, 2.25f);
        ourGraph.AddEdge(7, 8, 11.0f);
        
        labelEdges();
    }

    public void createMapGraph()
    {
        ourGraph = new Graph();
        Node node;

        updateH = false;

        node = new Node(1, -1.2f, 0, 1.4f, 380.0f);         // Oradea (380)
        ourGraph.AddNode(node);
        node = new Node(2, -1.4f, 0, 1.2f, 374.0f);         // Zerind (374)
        ourGraph.AddNode(node);
        node = new Node(3, -1.35f, 0, 1.0f, 366.0f);        // Arad (366)
        ourGraph.AddNode(node);
        node = new Node(4, -1.5f, 0, 0.4f, 366.0f);         // Timisoara (329)
        ourGraph.AddNode(node);
        node = new Node(5, -1.4f, 0, -0.4f, 244.0f);        // Lugoj (244)
        ourGraph.AddNode(node);
        node = new Node(6, -1.2f, 0, -0.6f, 241.0f);        // Mehadia (241)
        ourGraph.AddNode(node);
        node = new Node(7, -1.0f, 0, -1.4f, 242.0f);        // Dobreta (242)
        ourGraph.AddNode(node);
        node = new Node(8, -0.6f, 0, -1.5f, 160.0f);        // Craiova (160)
        ourGraph.AddNode(node);
        node = new Node(9, -1.2f, 0, 0.4f, 160.0f);         // Sibiu (253)
        ourGraph.AddNode(node);
        node = new Node(10, -0.2f, 0, 0.0f, 176.0f);        // Faragas (176)
        ourGraph.AddNode(node);
        node = new Node(11, -0.3f, 0, -0.6f, 193.0f);        // Rimnicu (193)
        ourGraph.AddNode(node);
        node = new Node(12, -0.1f, 0, -1.0f, 10.0f);        // Pitesti (10)
        ourGraph.AddNode(node);
        node = new Node(13, 0.4f, 0, -1.1f, 0.0f);        // Bucharest (0)
        ourGraph.AddNode(node);
        node = new Node(14, 0.0f, 0, -1.5f, 77.0f);        // Giurgui (77)
        ourGraph.AddNode(node);
        node = new Node(15, 0.8f, 0, -0.5f, 88.0f);        // Urziceni (88)
        ourGraph.AddNode(node);
        node = new Node(16, 1.2f, 0, -0.4f, 151.0f);        // Hirsova (151)
        ourGraph.AddNode(node);
        node = new Node(17, 1.2f, 0, -1.5f, 161.0f);        // Eforie (161)
        ourGraph.AddNode(node);
        node = new Node(18, 1.5f, 0, 0.6f, 199.0f);        // Vaslui (199)
        ourGraph.AddNode(node);
        node = new Node(19, 1.4f, 0, 1.2f, 226.0f);        // Iasi (226)
        ourGraph.AddNode(node);
        node = new Node(20, 1.0f, 0, 1.5f, 234.0f);        // Neamt (234)
        ourGraph.AddNode(node);

        ourGraph.AddEdge(1, 2, 71.0f);
        ourGraph.AddEdge(2, 3, 75.0f);
        ourGraph.AddEdge(3, 4, 118.0f);
        ourGraph.AddEdge(3, 9, 140.0f);
        ourGraph.AddEdge(4, 5, 111.0f);
        ourGraph.AddEdge(5, 6, 70.0f);
        ourGraph.AddEdge(6, 7, 75.0f);
        ourGraph.AddEdge(7, 8, 120.0f);
        ourGraph.AddEdge(8, 11, 146.0f);
        ourGraph.AddEdge(8, 12, 138.0f);
        ourGraph.AddEdge(9, 10, 99.0f);
        ourGraph.AddEdge(9, 11, 80.0f);
        ourGraph.AddEdge(10, 13, 211.0f);
        ourGraph.AddEdge(11, 12, 97.0f);
        ourGraph.AddEdge(12, 13, 101.0f);
        ourGraph.AddEdge(13, 14, 90.0f);
        ourGraph.AddEdge(13, 15, 86.0f);
        ourGraph.AddEdge(15, 16, 98.0f);
        ourGraph.AddEdge(16, 17, 86.0f);
        ourGraph.AddEdge(16, 18, 142.0f);
        ourGraph.AddEdge(18, 19, 92.0f);
        ourGraph.AddEdge(19, 20, 234.0f);
        
        labelEdges();
    }
    
    void labelEdges()
    {
        //clear any existing edge labels
        for (int i=0; i<labels.Count; i++)
        {
            Destroy(labels[i]);
        }
        
        //loop through all edges and label their weights visually
        foreach (Edge edge in ourGraph.edges)
        {
            GameObject label = new GameObject("EDL");
            float x = (edge.startNode.position.x + edge.endNode.position.x) * 9.1f;
            float z = (edge.startNode.position.z + edge.endNode.position.z) * 9f;
            label.transform.position = new Vector3(x - 1f, -50f, z + 0.7f);
            label.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            TextMesh tm = label.AddComponent<TextMesh>();
            tm.font = font;
            tm.fontSize = 13;
            tm.text = edge.Weight.ToString();
            tm.alignment = TextAlignment.Center;
            labels.Add(label);
        }
    }
    
    public void dfsAnimation()
    {
        searchAnimationType = 0;
        isPause = false;
        if (Agent.GetComponent<SearchAnimation>().DFSAnimation(ourGraph, start, end))
        {
            searchAnimationType = 1;
        }
    }
    
    public void dijkstraAnimation()
    {
        searchAnimationType = 0;
        isPause = false;
        if (Agent.GetComponent<SearchAnimation>().DijkstraAnimation(ourGraph, start, end))
        {
            searchAnimationType = 2;
        }
    }
    
    public void aStarAnimation()
    {
        searchAnimationType = 0;
        isPause = false;
        if (Agent.GetComponent<SearchAnimation>().AStarAnimation(ourGraph, start, end, updateH))
        {
            searchAnimationType = 3;
        }
    }
    
    public void pauseResume()
    {
        if (searchAnimationType > 0 && !isPause)
        {
            isPause = true;
            Time.timeScale = 0;
            Debug.Log("Pause");
        }
        else if (isPause)
        {
            isPause = false;
            Time.timeScale = 1;
            Debug.Log("Resume");
        }
    }
    
    public void reset()
    {
        resetAnimation();
    }
    
    public void exit()
    {
        Application.Quit();
    }

    void resetAnimation()
    {
        if (searchAnimationType == 0) return;

        searchAnimationType = 0;

        Agent.GetComponent<SearchAnimation>().StopSearchAnimation();

        Agent.transform.position = agentPos;
        Agent.transform.rotation = agentOrientation;

        Agent.GetComponent<SearchAnimation>().ResetGraphForSearch(updateH);
    }

    void OnPostRender()
    {
        // Must use the line material; otherwise un-predictable material is used.
        lineMaterial.SetPass(0); 
        DrawOurGraph();
    }

    void OnApplicationQuit()
    {
        DestroyImmediate(lineMaterial);
    }

    void DrawOurGraph()
    {
        if (ourGraph == null) return;

        // Draw nodes of the graph
        foreach (Node node in ourGraph.nodes)
        {
            GLDraw.DrawDot(node.Position.x, node.Position.z, node.color, node.dotSize);
            //Debug.Log(node.Position.x);
        }

        // Draw edges of the graph
        foreach (Edge edge in ourGraph.edges)
            GLDraw.DrawLine(edge.startNode.Position, edge.endNode.Position, edge.color);
   }
}

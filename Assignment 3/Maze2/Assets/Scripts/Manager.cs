using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MazeGen;

public class Manager : MonoBehaviour 
{
    public GameObject Agent;
    public GUIStyle gs;
    
    public Button generateMazeButton;
    public Toggle showGraphToggle;
    public Button dfsButton;
    public Button dijkstraButton;
    public Button aStarButton;
    public InputField rowsInput;
    public InputField columnsInput;
    public Button resizeButton;
    public Button dfsAnimationButton;
    public Button dijkstraAnimationButton;
    public Button aStarAnimationButton;
    public Button pauseButton;
    public Button exitButton;
    
    Maze maze;
    Graph ourGraph = null;
    bool showGraph = false;
    bool showGraphSolution = false;
    bool isPaused = false;
    string rows = "20";
    string columns = "20";

    Material lineMaterial;

	// Use this for initialization
	void Start () 
    {
        maze = new Maze(20, 20);
        //lineMaterial = new Material (Shader.Find ("Diffuse"));  // this doesn't work well (built-in shaders do not use vertex color?)
        lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
        "SubShader { Pass {" +
        "   BindChannels { Bind \"Color\",color }" +
        "   Blend SrcAlpha OneMinusSrcAlpha" +
        "   ZWrite Off Cull Off Fog { Mode Off }" +
        "} } }");
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	}
    
    public void ToggleShowGraph(bool showGraph)
    {
        this.showGraph = showGraph;
        this.showGraphSolution = showGraph;
    }
    
    public void UpdateRows(string rows)
    {
        this.rows = rows;
    }
    
    public void UpdateColumns(string columns)
    {
        this.columns = columns;
    }
    
    public void TryGenerateMaze()
    {
        maze.Init();
        if (!maze.Gen)
        {
            maze.GenerateMaze();
            maze.Init();
            maze.GenerateMaze();
            maze.GenerateGraph();            
        }
        else
        {
            maze.GenerateMaze();
            maze.GenerateGraph();
        }

        maze.GenerateSimplifiedGraph();
        
        //maze.PrintGraph();
    }
    
    public void DFSButton()
    {
        if (maze.Gen)
        {
            //maze.SolveMaze();
            maze.SolveMazeWithGraphDFS();
        }
    }
    
    public void DijkstraButton()
    {
        if (maze.Gen)
        {
            //maze.SolveMaze();
            maze.SolveMazeWithDijkstra();
        }
    }
    
    public void AStarButton()
    {
        if (maze.Gen)
        {
            //maze.SolveMaze();
            maze.SolveMazeWithAStar();
        }
    }
    
    public void ResizeButton()
    {
        int nr = 0;
        int nc = 0;
        try
        {
            nr = Convert.ToInt32(rowsInput.text);
            nc = Convert.ToInt32(columnsInput.text);
        }
        catch { }
        finally
        {
            if (nr >= 2 && nc >= 2)
            {
                maze.Resize(nr, nc);
                rows = rowsInput.text;
                columns = columnsInput.text;
            }
        }
    }
    
    public void DFSAnimationButton()
    {
        Agent.GetComponent<SearchAnimation>().DFSAnimation(maze.graph,
            maze.StartNodeID, maze.EndNodeID);
        //if (setRobotWaypoints(showGraphSolution))
        //    robot.GetComponent<WPNavigate>().SetCurrentWP(0);
        //robot.animation["run"].wrapMode = WrapMode.Loop;
        //robot.animation.Play("run");
    }
    
    public void DijkstraAnimationButton()
    {
        Agent.GetComponent<SearchAnimation>().DijkstraAnimation(maze.graph, maze.StartNodeID, maze.EndNodeID);
        //Agent.GetComponent<SearchAnimation>().DijkstraAnimation(ourGraph, 1, 8);
    }
    
    public void AStarAnimationButton()
    {
        Agent.GetComponent<SearchAnimation>().AStarAnimation(maze.graph, maze.StartNodeID, maze.EndNodeID);
    }
    
    public void PauseButton()
    {
        if (isPaused)
        {
            Time.timeScale = 1.0f;
            isPaused = false;
            pauseButton.GetComponentInChildren<Text>().text = "Pause";
        }
        else
        {
            Time.timeScale = 0.0f;
            isPaused = true;
            pauseButton.GetComponentInChildren<Text>().text = "Resume";
        }
    }
    
    public void ExitButton()
    {
        Application.Quit();
    }

    void OnPostRender()
    {
        if (!maze.Gen)
        {
            lineMaterial.SetPass(0); // Must use the line material; otherwise un-predictable material is used.
            maze.ShowGrid();
        }
        else
        {
            lineMaterial.SetPass(0);
            maze.DrawMaze();

            if (showGraph)
                maze.DrawGraph();

            if (showGraphSolution)
                    maze.DrawSolutionGraphPath();
            else
                    maze.DrawSolutionPath();
        }
 
     /*
            // Must use the line material; otherwise un-predictable material is used.
            lineMaterial.SetPass(0); 
            DrawOurGraph();
    */

    }

	bool setRobotWaypoints (bool useGraphPath)
	{
        if (!useGraphPath) 
            return setRobotWaypoints();

        if (maze.path == null) return false;

        Vector3[] points;
        points = new Vector3[maze.path.Length+2];

        for (int i = 1; i <= maze.path.Length; ++i)
        {
            points[i] = maze.path[i-1].Position;
        }

        Vector3[,] cellPosition = maze.CellCenterPosition;
        float sx = cellPosition[0, 1].x - cellPosition[0, 0].x;
        // First point in the waypoints
        points[0] = new Vector3(points[1].x - 2 * sx, 0, points[1].z);
        // The last point in the waypoints
        points[points.Length - 1] = new Vector3(points[points.Length - 2].x + 2 * sx, 0, points[points.Length - 2].z);

        //robot.transform.position = new Vector3(points[0].x, robot.transform.position.y, points[0].z);
        Agent.GetComponent<WPNavigate>().SetWaypoints(points);
        Debug.Log("Waypoints set");
        return true;
    }

    bool setRobotWaypoints()
    {
		if (maze.SolutionPath == null) return false;

        Vector3[,] cellPosition = maze.CellCenterPosition;
        float sx = cellPosition[0, 1].x - cellPosition[0, 0].x;

		// The maze solution path is in the reversed order
		int i;
		Vector3[] points;
		points = new Vector3[maze.SolutionPath.Length+3];
        i = 1;
		for (int ri = maze.SolutionPath.Length - 1; ri >= 0; ri--)
		{
            points[i++] = cellPosition[maze.SolutionPath[ri].row, maze.SolutionPath[ri].col];
		}

        // First point in the waypoints
        points[0] = new Vector3(points[1].x - 2*sx, 0, points[1].z);

        // Next to the last point in the waypoints
        points[points.Length-2] = cellPosition[maze.End, maze.NC - 1];

        // The last point in the waypoints
        points[points.Length-1] = new Vector3(points[points.Length-2].x + 2*sx, 0, points[points.Length-2].z);

        //robot.transform.position = new Vector3(points[0].x, robot.transform.position.y, points[0].z);
        Agent.GetComponent<WPNavigate>().SetWaypoints(points);

		return true;
	}

    void OnApplicationQuit()
    {
        DestroyImmediate(lineMaterial);
    }

    public void CreateGraph()
    {
        ourGraph = new Graph();
        
        ourGraph.AddNode (1, new Vector3(-1.2f, 0, 1.2f));
        ourGraph.AddNode (2, new Vector3(0, 0, 1.2f));
        ourGraph.AddNode (3, new Vector3(1.2f, 0, 1.2f));
        ourGraph.AddNode (4, new Vector3(-0.6f, 0, 0));
        ourGraph.AddNode (5, new Vector3(0.6f, 0, 0));
        ourGraph.AddNode (6, new Vector3(-1.2f, 0, -1.2f));
        ourGraph.AddNode (7, new Vector3(0, 0, -1.2f));
        ourGraph.AddNode (8, new Vector3(1.2f, 0, -1.2f));

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

using System;
using System.Collections;
using UnityEngine;
using MazeGen;

public class ManagerMaze : MonoBehaviour 
{
    public GameObject Agent;
    public GUIStyle gs;
   
    Maze maze;

    string rows = "20";
    string columns = "20";
    bool showGraph = false;
    bool showGraphSolution = false;

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

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 40, 140, 30), "Generate maze"))
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
        showGraph = GUI.Toggle(new Rect(20, 70, 140, 30), showGraph, " show graph");

        GUI.Label(new Rect(10, 120, 100, 30), "Solve maze :");
        if (GUI.Button(new Rect(20, 150, 100, 30), "DFS"))
        {
            if (maze.Gen)
            {
                //maze.SolveMaze();
                maze.SolveMazeWithGraphDFS();
            }
        }
        if (GUI.Button(new Rect(20, 180, 100, 30), "Dijkstra"))
        {
            if (maze.Gen)
            {
                //maze.SolveMaze();
                maze.SolveMazeWithDijkstra();
            }
        }
        if (GUI.Button(new Rect(20, 210, 100, 30), "AStar"))
        {
            if (maze.Gen)
            {
                //maze.SolveMaze();
                maze.SolveMazeWithAStar();
            }
        }
        showGraphSolution = GUI.Toggle(new Rect(20, 240, 140, 30), showGraphSolution, 
            " show graph solution");

        GUI.Label(new Rect(10, 300, 100, 30), "rows :");
        rows = GUI.TextField(new Rect(120, 300, 100, 30), rows, 6);
        GUI.Label(new Rect(10, 330, 100, 30), "columns :");    
        columns = GUI.TextField(new Rect(120, 330, 100, 30), columns, 6);

        if (GUI.Button(new Rect(120, 360, 140, 30), "resize"))
        {
            int nr = 0;
            int nc = 0;
            try
            {
                nr = Convert.ToInt32(rows);
                nc = Convert.ToInt32(columns);
            }
            catch { }
            finally
            {
                if (nr >= 2 && nc >= 2)
                {
                    maze.Resize(nr, nc);
                }
            }
        }

        if (GUI.Button(new Rect(10, 400, 180, 30), "DFS Search Animation"))
        {
            Agent.GetComponent<SearchAnimation>().DFSAnimation(maze.graph,
                maze.StartNodeID, maze.EndNodeID);

            //if (setRobotWaypoints(showGraphSolution))
            //    robot.GetComponent<WPNavigate>().SetCurrentWP(0);

            //robot.animation["run"].wrapMode = WrapMode.Loop;
            //robot.animation.Play("run");
        }

        if (GUI.Button(new Rect(10, 440, 180, 30), "Dijkstra Search Animation"))
        {
            Agent.GetComponent<SearchAnimation>().DijkstraAnimation(maze.graph,
                maze.StartNodeID, maze.EndNodeID);     
        }

        if (GUI.Button(new Rect(10, 480, 180, 30), "A* Search Animation"))
        {
            Agent.GetComponent<SearchAnimation>().AStarAnimation(maze.graph,
                maze.StartNodeID, maze.EndNodeID);

        }

        if (GUI.Button(new Rect(10, 560, 180, 30), "Exit"))
        {
            Application.Quit();
        }
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

}

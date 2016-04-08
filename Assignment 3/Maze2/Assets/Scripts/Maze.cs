using System;
using System.Collections.Generic;
using UnityEngine;


namespace MazeGen
{

    //Define Cell strcuture of the Maze
    public struct Cell
    {
        public int row, col;    // row is numbered from bottom to top; 
                                // col is numbered from left to right. 
        public bool visited;
    };

    /// <summary>
    /// Summary description for Maze.
    /// </summary>
    /// 
    public class Maze
    {
        // Maze configuration
        int nR, nC;                     // size of the maze (number of Rows and Columns)
        int[,] cellID;                  // ID, 1-based, is assigned row by row  
                                        // (from bottom to top)
        bool[,] eastWall, northWall;    // for each cell:
                                        // true ==> wall exists, false ==> no wall exists
        Vector3[,] centerPos;           // position of the center of cell

        int startR, endR;               // (startR, 0) (endR, NC-1): 
                                        // openning of the maze at left and right ends

        // Used for maze generation and solving
        bool[,] visited;
        bool gen;                        // true if the maze has been generated
        Cell[] solutionPath = null;      // not null if the maze has been solved

        // Maze drawing parameters (or transformation matrix from cell space to 3D world space)
        float vxmax = 1.5f;
        float vxmin = -1.5f;    // -vxmax;
        float vzmax = 1.5f;
        float vzmin = -1.5f;     // -vzmax;
        float sx, sz;

        System.Random rand = new System.Random((int)DateTime.Now.Ticks);

        // Graph for path finding
        public Graph graph = new Graph();
        public Node[] path = null;
        //public bool graphSolutionOn = false;

        #region Constructors and properties

        public Maze()
        {
            nR = nC = 0;
            eastWall = northWall = visited = null;
            gen = false;
        }

        public Maze(int r, int c)
        {
            Resize(r, c);
        }

        public int NR
        {
            get { return nR; }
        }

        public int NC
        {
            get { return nC; }
        }

        public int[,] CellID
        {
            get { return cellID; }
        }

        public bool[,] EastWall
        {
            get { return eastWall; }
        }

        public bool[,] NorthWall
        {
            get { return northWall; }
        }

        public Vector3[,] CellCenterPosition
        {
            get { return centerPos; }
        }

        public int Start
        {
            get { return startR; }
        }

        public int End
        {
            get { return endR; }
        }

        public bool Gen
        {
            get { return gen; }
        }

        public Cell[] SolutionPath
        {
            get { return solutionPath; }
        }

        public int StartNodeID
        {
            get { return cellID[startR, 0]; }
        }

        public int EndNodeID
        {
            get { return cellID[endR, nC - 1]; }
        }

        #endregion Constructors and properties

        #region Methods to generate path graph (full or simplified) of the maze  

        public bool GenerateGraph()
        {
            if (!gen) return false;
            graph.Clear();

            // Add center points of open cells to the graph
            for (int r = 0; r < nR; r++)
            {
                for (int c = 0; c < nC; c++)
                {
                    graph.AddNode(cellID[r, c], centerPos[r, c]);
                }
            }

            // Add edges connecting open cells to the graph
            for (int r = 0; r < nR; r++)
            {
                for (int c = 0; c < nC; c++)
                {
                    // Add east edge from (r,c) cell if no eastWall[r,c]
                    if (!eastWall[r, c] && c < nC - 1)
                        graph.AddEdge(cellID[r, c], cellID[r, c + 1], 1.0f);

                    // Add north edge from (r,c) cell if no northtWall[r,c]
                    if (!northWall[r, c] && r < nR - 1)
                        graph.AddEdge(cellID[r, c], cellID[r+1, c], 1.0f);
                }
            }

            return true;
        }

        public bool GenerateSimplifiedGraph()
        {
            if (!gen) return false;
            graph.Clear();

            bool[,] isNode = new bool[nR, nC];
            bool[,] isAdded = new bool[nR, nC];
            for (int r = 0; r < nR; r++)
            {
                for (int c = 0; c < nC; c++)
                {
                    isNode[r, c] = true;
                    isAdded[r, c] = false;
                }
            }

            for (int r = 0; r < nR; r++)
            {
                for (int c = 0; c < nC; c++)
                {
                    if (isNode[r, c])
                    {
                        // Add this cell to the graph node list
                        if (!isAdded[r, c])
                        {
                            graph.AddNode(cellID[r, c], centerPos[r, c]);
                            isAdded[r, c] = true;
                        }

                        // Add horizotal (west-to-eat) graph edge list from this node if possible
                        if (!eastWall[r,c] && c < nC -1)
                        {
                            for (int nextC = c + 1; nextC < nC; nextC++)
                            {
                                if (eastWall[r, nextC] || !northWall[r, nextC] || (nextC == nC - 1 && r == endR)
                                    || (r > 0 && !northWall[r-1, nextC]))
                                {
                                    if (!isAdded[r, nextC])
                                    {
                                        graph.AddNode(cellID[r, nextC], centerPos[r, nextC]);
                                        isAdded[r, nextC] = true;
                                    }
                                    graph.AddEdge(cellID[r, c], cellID[r, nextC], 1.0f);
                                    break;
                                }
                                else isNode[r, nextC] = false;
                            }
                        }

                        // Add vertical (south-to-north) graph edge list from this node if possible
                        if (!northWall[r, c])
                        {
                            for (int nextR = r + 1; nextR < nR; nextR++)
                            {
                                if (northWall[nextR, c] || !eastWall[nextR, c] || (nextR == startR && c == 0)
                                    || (c > 0 && !eastWall[nextR, c-1]))
                                {
                                    if (!isAdded[nextR, c])
                                    {
                                        graph.AddNode(cellID[nextR, c], centerPos[nextR, c]);
                                        isAdded[nextR, c] = true;
                                    }
                                    graph.AddEdge(cellID[r, c], cellID[nextR, c], 1.0f);
                                    break;
                                }
                                else isNode[nextR,c] = false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public void PrintGraph()
        {
            Debug.Log("number of nodess " + graph.nodes.Count.ToString());
            Debug.Log("number of edges " + graph.edges.Count.ToString());
            int n = 0;
            foreach (Node node in graph.nodes)
                n += node.NextNodes.Count;
            Debug.Log("number of edges (from nodes) " + n.ToString());
            //foreach (Edge e in graph.edges)
            //{
            //    Debug.Log ("edge " + e.startNode.ID.ToString() + " " + e.endNode.ID.ToString());
            //}
            foreach (Node node in graph.nodes)
            {
                Debug.Log("Node " + node.ID.ToString());
                foreach (Node node2 in node.NextNodes)
                    Debug.Log("   Next node -> " + node2.ID.ToString());
            }
        }

        #endregion Methods to generate path graph (full or simplified) of the maze

        #region Resize and Init maze methods

        public void Resize(int nr, int nc)
        {
            cellID = new int[nr, nc];
            eastWall = new bool[nr, nc];
            northWall = new bool[nr, nc];
            centerPos = new Vector3[nr, nc];

            visited = new bool[nr, nc];
            nR = nr;
            nC = nc;
            Init();
            gen = false;
        }

        public void Init()
        {
            // Initialize maze drawing parameters
            sx = (vxmax - vxmin) / nC;
            sz = (vzmax - vzmin) / nR;

            float x, z;
            int i = 1;
            for (int r = 0; r < nR; r++)
            {
                for (int c = 0; c < nC; c++)
                {
                    cellID[r, c] = i++;
                    eastWall[r, c] = true;
                    northWall[r, c] = true;
                    visited[r, c] = false;

                    // Calculate positions of the centers of all cells
                    x = c * sx + vxmin + sx / 2;
                    z = r * sz + vzmin + sz / 2;
                    centerPos[r, c] = new Vector3(x, 0, z);
                }
            }
        }

        #endregion Resize and Init maze methods

        #region Maze generation methods


        // Generate a maze using a depth-first search algorithm
        public void GenerateMaze(bool modified = false)
        {
            //System.Random rand2 = new System.Random((int)DateTime.Now.Ticks);

            Stack<Cell> CellStack = new Stack<Cell>();
            int TotalCells = nR * nC;
            Cell CurrentCell, SelectedCell;
            int VisitedCells;

            // Randomly select one cell as current cell
            //setBarrierCell();
            int r = rand.Next() % nR;
            int c = rand.Next() % nC;

            CurrentCell.row = r;
            CurrentCell.col = c;
            CurrentCell.visited = false;
            visited[r, c] = true;
            VisitedCells = 1;

            int visitedIndex;
            Cell[] T = new Cell[4];

            while (VisitedCells < TotalCells)
            {
                visitedIndex = 0;

                for (int i = 0; i < 4; i++)
                {
                    T[i].visited = true;
                }
                // Check north cell
                if (r != (nR - 1))
                {
                    if (!visited[r + 1, c])
                    {
                        T[visitedIndex].row = r + 1;
                        T[visitedIndex].col = c;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }
                // Check west cell
                if (c != 0)
                {
                    if (!visited[r, c - 1])
                    {
                        T[visitedIndex].row = r;
                        T[visitedIndex].col = c - 1;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }
                // Check south cell
                if (r != 0)
                {
                    if (!visited[r - 1, c])
                    {
                        T[visitedIndex].row = r - 1;
                        T[visitedIndex].col = c;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }
                // Check east cell
                if (c != nC - 1)
                {
                    if (!visited[r, c + 1])
                    {
                        T[visitedIndex].row = r;
                        T[visitedIndex].col = c + 1;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }

                if (visitedIndex > 0)
                {
                    // If there is at least one cell unvisited, randomly select one.
                    SelectedCell = T[rand.Next() % visitedIndex];

                    // Knock down the wall between these two cells.
                    // Note: (r,c) is the CurrentCell.
                    if ((SelectedCell.row == (r + 1)) && (SelectedCell.col == c))
                        northWall[r, c] = false;
                    if ((SelectedCell.row == r) && (SelectedCell.col == (c - 1)))
                        eastWall[r, c - 1] = false;
                    if ((SelectedCell.row == (r - 1)) && (SelectedCell.col == c))
                        northWall[r - 1, c] = false;
                    if ((SelectedCell.row == r) && (SelectedCell.col == (c + 1)))
                        eastWall[r, c] = false;

                    CellStack.Push(CurrentCell);

                    // Choose the selected cell as the current cell
                    CurrentCell = SelectedCell;
                    r = CurrentCell.row;
                    c = CurrentCell.col;
                    visited[r, c] = true;
                    VisitedCells++;
                }
                else
                {
                    //If there is no cell unvisited, pop cell from the stack		
                    CurrentCell = CellStack.Pop();
                    r = CurrentCell.row;
                    c = CurrentCell.col;
                    visited[r, c] = true;
                }
            }
            CellStack.Clear();

            // Randomly select the start and end cell
            startR = rand.Next() % nR;
            endR = rand.Next() % (nR - 1) + 1;
            //endR = rand.Next() % nR;
            eastWall[endR, (nC - 1)] = false;

            if (modified)
                removeWalls(30);

            // Set the gen flag and clear the solution path
            gen = true;
            solutionPath = null;
            path = null;
        }

        // Generate a maze using a depth-first search algorithm with more horizontal influence
        public void GenerateMaze2()
        {
            //System.Random rand2 = new System.Random((int)DateTime.Now.Ticks);

            Stack<Cell> CellStack = new Stack<Cell>();
            int TotalCells = nR * nC;
            Cell CurrentCell, SelectedCell;
            int VisitedCells;

            // Randomly select one cell as current cell
            //setBarrierCell();
            int r = rand.Next() % nR;
            int c = rand.Next() % nC;

            CurrentCell.row = r;
            CurrentCell.col = c;
            CurrentCell.visited = false;
            visited[r, c] = true;
            VisitedCells = 1;

            int visitedIndex;
            Cell[] T = new Cell[4];

            bool isEast = false;
            int indexEast = 0;
            bool isWest = false;
            int indexWest = 0;
            while (VisitedCells < TotalCells)
            {
                visitedIndex = 0;

                isEast = false;
                isWest = false;

                for (int i = 0; i < 4; i++)
                {
                    T[i].visited = true;
                }
                // Check north cell
                if (r != (nR - 1))
                {
                    if (!visited[r + 1, c])
                    {
                        T[visitedIndex].row = r + 1;
                        T[visitedIndex].col = c;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }
                // Check west cell
                if (c != 0)
                {
                    if (!visited[r, c - 1])
                    {
                        isWest = true;
                        indexWest = visitedIndex;

                        T[visitedIndex].row = r;
                        T[visitedIndex].col = c - 1;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }
                // Check south cell
                if (r != 0)
                {
                    if (!visited[r - 1, c])
                    {
                        T[visitedIndex].row = r - 1;
                        T[visitedIndex].col = c;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }
                // Check east cell
                if (c != nC - 1)
                {
                    if (!visited[r, c + 1])
                    {
                        isEast = true;
                        indexEast = visitedIndex;

                        T[visitedIndex].row = r;
                        T[visitedIndex].col = c + 1;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }

                if (visitedIndex > 0)
                {
                    // Select East or West unvisited cell first if there is any;
                    // otherwiese randomly select one
                    if (isWest)
                        SelectedCell = T[indexWest];
                    //else if (isEast)
                    //    SelectedCell = T[indexEast];
                    else // If there is at least one cell unvisited, randomly select one.
                        SelectedCell = T[rand.Next() % visitedIndex];

                    // Knock down the wall between these two cells.
                    // Note: (r,c) is the CurrentCell.
                    if ((SelectedCell.row == (r + 1)) && (SelectedCell.col == c))
                        northWall[r, c] = false;
                    if ((SelectedCell.row == r) && (SelectedCell.col == (c - 1)))
                        eastWall[r, c - 1] = false;
                    if ((SelectedCell.row == (r - 1)) && (SelectedCell.col == c))
                        northWall[r - 1, c] = false;
                    if ((SelectedCell.row == r) && (SelectedCell.col == (c + 1)))
                        eastWall[r, c] = false;

                    CellStack.Push(CurrentCell);

                    // Choose the selected cell as the current cell
                    CurrentCell = SelectedCell;
                    r = CurrentCell.row;
                    c = CurrentCell.col;
                    visited[r, c] = true;
                    VisitedCells++;
                }
                else
                {
                    //If there is no cell unvisited, pop cell from the stack		
                    CurrentCell = CellStack.Pop();
                    r = CurrentCell.row;
                    c = CurrentCell.col;
                    visited[r, c] = true;
                }
            }
            CellStack.Clear();

            // Randomly select the start and end cell
            startR = rand.Next() % nR;
            endR = rand.Next() % (nR - 1) + 1;
            //endR = rand.Next() % nR;
            eastWall[endR, (nC - 1)] = false;

            //removeWalls(30);

            // Set the gen flag and clear the solution path
            gen = true;
            solutionPath = null;
            path = null;
        }

        // Remove n walls from the generated maze
        int removeWalls(int n)
        {
            //System.Random rand2 = new System.Random((int)DateTime.Now.Ticks);

            if (!gen) return 0;

            // Remove wall from randomly select one cell as current cell
            int nWalls = 0;
            int r, c;

            int test = 0;

            while (test < n)
            {
                r = rand.Next() % nR;
                c = rand.Next() % nC;

                if (nWalls % 2 == 0)
                {
                    if (c < nC - 1 && eastWall[r, c])
                    {
                        eastWall[r, c] = false;
                        nWalls++;
                    }
                }
                else
                {
                    if (r < nR - 1 && northWall[r, c])
                    {
                        northWall[r, c] = false;
                        nWalls++;
                    }
                }
                test++;
            }

            return nWalls;

        }

        void setBarrierCell()
        {
            visited[nR - 2, 4] = visited[nR - 2, 5] = visited[nR - 2, 6] = true;
            eastWall[nR - 2, 4] = eastWall[nR - 2, 5] = false;

        }

        #endregion Maze generation methods

        #region SolveMaze method

        // Solve the maze using a depth-first graph search algorithm 
        public void SolveMazeWithGraphDFS()
        {
            if (!gen) return;

            path = graph.DepthFirstSearch(cellID[startR,0], cellID[endR, nC-1]);
        }

        public void SolveMazeWithDijkstra()
        {
            if (!gen) return;

            path = graph.DijkstraSearch(cellID[startR, 0], cellID[endR, nC - 1]);
        }

        public void SolveMazeWithAStar()
        {
            if (!gen) return;

            path = graph.AStar(cellID[startR, 0], cellID[endR, nC - 1]);
        }

        // Solve the maze using a depth-first traversal algorithm
        public void SolveMaze()
        {
            //System.Random rand2 = new System.Random((int)DateTime.Now.Ticks);

            if (!gen) return;

            // Check if the maze has been solved
            if (solutionPath != null) solutionPath = null;

            Stack<Cell> PathStack = new Stack<Cell>();
            int r, c;
            Cell EndCell, CurrentCell, SelectedCell;

            EndCell.row = endR;
            EndCell.col = nC - 1;
            EndCell.visited = false;
            CurrentCell.row = startR;
            CurrentCell.col = 0;
            CurrentCell.visited = false;
            visited[startR, 0] = true;

            for (r = 0; r < nR; r++)
                for (c = 0; c < nC; c++)
                    visited[r, c] = false;
            visited[startR, 0] = true;

            int nvisited = 0;   // for search performance measure

            int visitedIndex;
            Cell[] T = new Cell[4];

            r = CurrentCell.row;
            c = CurrentCell.col;
            while ((CurrentCell.row != EndCell.row) || (CurrentCell.col != EndCell.col))
            {
                visitedIndex = 0;
                for (int i = 0; i < 4; i++)
                {
                    T[i].visited = true;
                }
                //check north cell
                if (r != (nR - 1))
                {
                    if ((!northWall[r, c]) && (!visited[r + 1, c]))
                    {
                        T[visitedIndex].row = r + 1;
                        T[visitedIndex].col = c;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }
                if (c != (nC - 1))
                {
                    //check east cell
                    if ((!eastWall[r, c]) && (!visited[r, c + 1]))
                    {
                        T[visitedIndex].row = r;
                        T[visitedIndex].col = c + 1;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }
                if (c != 0)
                {
                    //check west cell
                    if ((!eastWall[r, c - 1]) && (!visited[r, c - 1]))
                    {
                        T[visitedIndex].row = r;
                        T[visitedIndex].col = c - 1;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }
                if (r != 0)
                {
                    //check south cell
                    if ((!northWall[r - 1, c]) && (!visited[r - 1, c]))
                    {
                        T[visitedIndex].row = r - 1;
                        T[visitedIndex].col = c;
                        T[visitedIndex].visited = false;
                        visitedIndex++;
                    }
                }

                if (visitedIndex > 0)
                {
                    //If there is at least one cell unvisited, randomly select one 
                    SelectedCell = T[rand.Next() % visitedIndex];

                    //push the current cell onto the stack
                    PathStack.Push(CurrentCell);
                    //choose the selected cell as current cell
                    CurrentCell = SelectedCell;
                    r = CurrentCell.row;
                    c = CurrentCell.col;
                    CurrentCell.visited = true;
                    visited[r, c] = true;
                    nvisited++;
                }
                else
                {
                    // If there is no cell unvisited, pop cell from the stack;
                    // The poped cell is not part of the solution path.
                    CurrentCell = PathStack.Pop();
                    r = CurrentCell.row;
                    c = CurrentCell.col;
                    CurrentCell.visited = true;
                    visited[r, c] = true;
                    nvisited++;
                }
            }
            if (PathStack.Count > 0)
            {
                solutionPath = new Cell[PathStack.Count];
                PathStack.CopyTo(solutionPath, 0);  // Solution path is in reversed order
            }

            Debug.Log("SolotuonPath : Number of nodes visited = " + nvisited.ToString());
            Debug.Log("SolutionPath : Path length = " + solutionPath.Length.ToString());
        }

        #endregion SolveMaze method

        #region Drawing maze methods

        public void DrawSolutionGraphPath()
        {
            if (path == null) return;

            for(int i = 0; i < path.Length - 1; i++)
                GLDraw.DrawLine(path[i].Position, path[i+1].Position, Color.green);
        }

        public void DrawSolutionPath()
        {
            if (solutionPath == null) return;

            int vr1 = endR, vc1 = nC - 1;
            int vr2, vc2;

            // The maze solution path is drwan in the revesed order
            foreach (Cell cell in solutionPath)
            {
                vr2 = cell.row;
                vc2 = cell.col;
                GLDraw.DrawLine(centerPos[vr1, vc1], centerPos[vr2, vc2], Color.green);
                vr1 = vr2;
                vc1 = vc2;

            }
        }

        // Draw maze using GL
        public void DrawMaze()
        {
            int r, c;

            // Draw the left line		
            for (r = 0; r < nC; r++)
                DrawWall(r, 0, r + 1, 0);
            for (r = 0; r < startR; r++)
                DrawWall(0, r, 0, r + 1);

            // Draw the bottom line
            for (r = startR + 1; r < nR; r++)
                DrawWall(0, r, 0, r + 1);

            for (r = 1; r <= nR; r++)
                for (c = 1; c <= nC; c++)
                {
                    //draw north wall of one cell 
                    if (northWall[r - 1, c - 1])
                        DrawWall(c - 1, r, c, r);
                    //draw east wall of one cell
                    if (eastWall[r - 1, c - 1])
                        DrawWall(c, r, c, r - 1);
                }
        }

        // Draw graph of the maze.graph
        public void DrawGraph()
        {
            if (!gen) return;

            // Draw nodes of the graph
            foreach (Node node in graph.nodes)
                GLDraw.DrawDot(node.Position.x, node.Position.z, node.color, node.dotSize);

            // Draw edges of the graph
            foreach (Edge edge in graph.edges)
                GLDraw.DrawLine(edge.startNode.Position, edge.endNode.Position, edge.color);
        }

        // Draw graph of the cell connecting edges
        public void DrawCellGraph()
        {
            if (!gen) return;

            // Draw center dots of all cells
            for (int r = 0; r < nR; r++)
            {
                for (int c = 0; c < nC; c++)
                {
                    GLDraw.DrawDot(centerPos[r, c].x, centerPos[r, c].z, Color.yellow);

                    if (!eastWall[r, c] && c != nC - 1)
                        GLDraw.DrawLine(centerPos[r, c], centerPos[r, c + 1], Color.yellow);

                    if (!northWall[r, c] && r != nR - 1)
                        GLDraw.DrawLine(centerPos[r, c], centerPos[r + 1, c], Color.yellow);
                }
            }
        }
/*
        // Draw the solution line on the maze solution path
        private void DrawLine(float x1, float z1, float x2, float z2)
        {
            // tansform the coordinate
            float sx1, sz1, sx2, sz2;

            //the route will be in the middle of the cell
            sx1 = x1 * sx + vxmin + sx / 2;
            sz1 = z1 * sz + vzmin + sz / 2;
            sx2 = x2 * sx + vxmin + sx / 2;
            sz2 = z2 * sz + vzmin + sz / 2;

            //GL.LineWidth (4);

            GL.Begin(GL.LINES);
            GL.Color(Color.blue);	    // GL.Color must be in between GL.Begin and GL.End.
            GL.Vertex3(sx1, 0, sz1);
            GL.Vertex3(sx2, 0, sz2);
            GL.End();
        }
*/
        // Draw the wall of the maze
        private void DrawWall(float x1, float z1, float x2, float z2)
        {
            //transformation of the coodinate
            float sx1, sz1, sx2, sz2;		// screen coordinates of x1, z1, x2, z2

            sx1 = x1 * sx + vxmin;
            sz1 = z1 * sz + vzmin;
            sx2 = x2 * sx + vxmin;
            sz2 = z2 * sz + vzmin;

            GLDraw.DrawLine(new Vector3(sx1, 0, sz1), new Vector3(sx2, 0, sz2), Color.white);
        }

        //show grid
        public void ShowGrid()
        {
            int r, c;

            //GL.Enable (GL.LINE_STIPPLE);
            //GL.LineStipple (1, 0x0101);
            for (r = 0; r < nC; r++)
                DrawWall(r, 0, r + 1, 0);
            for (r = 0; r < nR; r++)
                DrawWall(0, r, 0, r + 1);
            for (r = 1; r <= nR; r++)
                for (c = 1; c <= nC; c++)
                {
                    DrawWall(c - 1, r, c, r);
                    DrawWall(c, r, c, r - 1);
                }
            //GL.Disable (GL.LINE_STIPPLE);

        }

        #endregion Drawing maze methods
    }

}

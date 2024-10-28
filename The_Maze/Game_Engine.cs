//#define MazeGenertorLoop // uncomment to run the generator in a loop
//#define DebugRandomMazeGeneration // uncomment me to watch the maze being built node-by-node
#define UsePrims // uncomment me to use an alternate algorithm for maze generation
using System.Text;
using Towel.DataStructures;



namespace The_Maze
{
    public class Game_Engine
    {


        static void Main()
        {
            if (OperatingSystem.IsWindows())
            {
                Console.WindowHeight = 32;
            }
            const int rows = 8;
            const int columns = 20;

            static Maze.Tile[,] GenerateMaze() =>
#if UsePrims
        Maze.GeneratePrims(rows, columns);
#else
                Maze.Generate(rows, columns);
#endif

#if MazeGenertorLoop
    while (true)
    {
        Maze.Tile[,] maze = GenerateMaze();
        Console.Clear();
        Console.WriteLine(Maze.Render(maze));
        Console.WriteLine("Press Enter To Continue...");
        Console.ReadLine();
    }
#else
            Console.CursorVisible = true;
            Maze.Tile[,] maze = GenerateMaze();

            Console.Clear();
            Console.WriteLine(Maze.Render(maze));
            Console.WriteLine();
            Console.WriteLine("Maze");
            Console.WriteLine("Solve the maze by using the arrow keys.");
            Console.WriteLine("Press escape to quit.");

            // Вызов BFS и получение количества шагов до конечной точки
            BFS bfs = new BFS(maze);
            var (path, steps) = bfs.FindShortestPath((0, 0), (rows - 1, columns - 1));

            if (steps != -1)
            {
                Console.WriteLine($"Кратчайшее количество ходов до конечной точки: {steps}");
            }
            else
            {
                Console.WriteLine("Нет доступного пути до конечной точки.");
            }

            // Выбор стратегии ИИ
            Console.WriteLine("Выберите стратегию для ИИ:");
            Console.WriteLine("1. Двигаться вдоль правой стенки");
            Console.WriteLine("2. Двигаться вдоль левой стенки");
            Console.WriteLine("3. Умный выбор на развилках");

            int choice;
            while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 3)
            {
                Console.WriteLine("Неверный выбор, попробуйте еще раз.");
            }

            AI ai = choice switch
            {
                1 => new AI(maze, 0, 0, AI.Strategy.FollowRightWall),
                2 => new AI(maze, 0, 0, AI.Strategy.FollowLeftWall),
                3 => new AI(maze, 0, 0, AI.Strategy.RandomSituation),
                _ => throw new InvalidOperationException("Неправильный выбор стратегии")
            };

            ai.Move(); // Запуск поведения ИИ
            //int row = 0;
            //int column = 0;
            //while (row != rows - 1 || column != columns - 1)
            //{
            //    Console.SetCursorPosition(column * 3 + 1, row * 3 + 1);
            //    switch (Console.ReadKey().Key)
            //    {
            //        // Управление доступными клавишами
            //        case ConsoleKey.UpArrow:
            //            if (maze[row, column].HasFlag(Maze.Tile.Up))
            //                row--;
            //            break;
            //        case ConsoleKey.DownArrow:
            //            if (maze[row, column].HasFlag(Maze.Tile.Down))
            //                row++;
            //            break;
            //        case ConsoleKey.LeftArrow:
            //            if (maze[row, column].HasFlag(Maze.Tile.Left))
            //                column--;
            //            break;
            //        case ConsoleKey.RightArrow:
            //            if (maze[row, column].HasFlag(Maze.Tile.Right))
            //                column++;
            //            break;
            //        case ConsoleKey.Escape:
            //            Console.Clear();
            //            Console.Write("Maze was closed.");
            //            return;
            //    }
            //}
            //Console.Clear();
            //Console.Write("You Win.");
#endif
        }
    }

    public static class Maze
    {
        [Flags]
        public enum Tile
        {
            Null = 0,
            Up = 1,
            Down = 2,
            Left = 4,
            Right = 8,
            Start = 16,
            End = 32,
        }


        #region Algorithm 2 (Prims)

        public class Graph
        {
            public class Node
            {
                public int OwnIndex { get; }
                public List<int> Connections { get; }
                public List<double> Costs { get; }

                public void Add(int other, double cost)
                {
                    Connections.Add(other);
                    Costs.Add(cost);
                }

                public Node(int ownIndex)
                {
                    OwnIndex = ownIndex;
                    Connections = new List<int>();
                    Costs = new List<double>();
                }
            }

            public Node[] Nodes { get; }

            public Graph(Node[] nodes)
            {
                Nodes = nodes ?? throw new ArgumentNullException(nameof(nodes));
            }

            public static Maze.Tile[,] ConvertToGrid(Graph graph, int rows, int columns, Func<int, int, int> index, int start_row, int start_column, int end_row, int end_column)
            {
                var tiles = new Maze.Tile[rows, columns];

                foreach (var node in graph.Nodes)
                {
                    if (node == null)
                        continue;

                    (int, int) Unpack(int i) => (i % rows, i / rows);

                    var (row, col) = Unpack(node.OwnIndex);

                    // directional
                    if (node.Connections.Contains(index(row - 1, col)))
                    {
                        tiles[row, col] |= Maze.Tile.Up;
                        tiles[row - 1, col] |= Maze.Tile.Down;
                    }
                    if (node.Connections.Contains(index(row + 1, col)))
                    {
                        tiles[row, col] |= Maze.Tile.Down;
                        tiles[row + 1, col] |= Maze.Tile.Up;
                    }
                    if (node.Connections.Contains(index(row, col - 1)))
                    {
                        tiles[row, col] |= Maze.Tile.Left;
                        tiles[row, col - 1] |= Maze.Tile.Right;
                    }
                    if (node.Connections.Contains(index(row, col + 1)))
                    {
                        tiles[row, col] |= Maze.Tile.Right;
                        tiles[row, col + 1] |= Maze.Tile.Left;
                    }

                    // start/end
                    if (row == start_row && col == start_column)
                    {
                        tiles[row, col] |= Maze.Tile.Start;
                    }
                    if (row == end_row && col == end_column)
                    {
                        tiles[row, col] |= Maze.Tile.End;
                    }
                }
                return tiles;
            }
        }

        public static Tile[,] GeneratePrims(
            int rows, int columns,
            int? start_row = null, int? start_column = null,
            int? end_row = null, int? end_column = null)
        {
            start_row ??= 0;
            start_column ??= 0;
            end_row ??= rows - 1;
            end_column ??= columns - 1;

            var grid = new Graph.Node[rows * columns];

            int Index(int row, int col) => row + rows * col;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    var n = new Graph.Node(Index(row, col));
                    if (row + 1 < rows)
                    {
                        n.Add(Index(row + 1, col), Random.Shared.NextDouble());
                    }
                    if (row - 1 >= 0)
                    {
                        n.Add(Index(row - 1, col), Random.Shared.NextDouble());
                    }
                    if (col + 1 < columns)
                    {
                        n.Add(Index(row, col + 1), Random.Shared.NextDouble());
                    }
                    if (col - 1 >= 0)
                    {
                        n.Add(Index(row, col - 1), Random.Shared.NextDouble());
                    }
                    grid[Index(row, col)] = n;
                }
            }

            var graph = new Graph(grid);

#if DebugRandomMazeGeneration
		Console.Clear();
		Console.WriteLine(Maze.Render(Graph.ConvertToGrid(graph, rows, columns, Index, start_row.Value, start_column.Value, end_row.Value, end_column.Value)));
		Console.WriteLine("Press Enter To Continue...");
		Console.ReadLine();
		var res = SimplePrims(graph, rows, columns, Index, start_row.Value, start_column.Value, end_row.Value, end_column.Value);
#else
            var res = SimplePrims(graph);
#endif

            return Graph.ConvertToGrid(res, rows, columns, Index, start_row.Value, start_column.Value, end_row.Value, end_column.Value);
        }

        private readonly struct TwoWayConnection : IComparable<TwoWayConnection>
        {
            public readonly int IndexA;
            public readonly int IndexB;
            public readonly double Cost;

            public TwoWayConnection(int indexA, int indexB, double cost)
            {
                IndexA = indexA;
                IndexB = indexB;
                Cost = cost;
            }

            public int CompareTo(TwoWayConnection other) => other.Cost.CompareTo(Cost); // inversed because of how the heap works
        }

        public static Graph SimplePrims(Graph graph
#if DebugRandomMazeGeneration
		, int rows, int columns, Func<int, int, int> index, int start_row, int start_column, int end_row, int end_column
#endif
            )
        {
            var newGraph = new Graph(new Graph.Node[graph.Nodes.Length]);
            var nodes = graph.Nodes;
            var current = nodes[0];
            newGraph.Nodes[0] = new Graph.Node(0);

            var heap = HeapArray.New<TwoWayConnection>();

            while (true)
            {
                for (int i = 0; i < current.Connections.Count; i++)
                {
                    heap.Enqueue(new TwoWayConnection(current.OwnIndex, current.Connections[i], current.Costs[i]));
                }

                TwoWayConnection c;
                do
                {
                    if (heap.Count is 0)
                    {
                        return newGraph;
                    }
                    c = heap.Dequeue();
                }
                while (newGraph.Nodes[c.IndexB] != null);

                newGraph.Nodes[c.IndexA].Add(c.IndexB, c.Cost);

                newGraph.Nodes[c.IndexB] = new Graph.Node(c.IndexB);
                current = graph.Nodes[c.IndexB];
                newGraph.Nodes[c.IndexB].Add(c.IndexA, c.Cost);

#if DebugRandomMazeGeneration
			Console.Clear();
			Console.WriteLine(Maze.Render(Graph.ConvertToGrid(newGraph, rows, columns, index, start_row, start_column, end_row, end_column)));
			Console.WriteLine("Press Enter To Continue...");
			Console.ReadLine();
#endif
            }
        }

        #endregion

        public static string Render(Tile[,] maze)
        {
            static char Center(Tile tile) =>
                tile.HasFlag(Tile.Start) ? 'S' :
                tile.HasFlag(Tile.End) ? 'E' :
                /* default */ ' ';

            static char Side(Tile tile, Tile flag) =>
                tile.HasFlag(flag) ? ' ' : '█';

            static char[,] RenderTile(Tile tile) => new char[,]
            {
            { '█', Side(tile, Tile.Up), '█' },
            { Side(tile, Tile.Left), Center(tile), Side(tile, Tile.Right) },
            { '█', Side(tile, Tile.Down), '█' },
            };
            int rows = maze.GetLength(0);
            int columns = maze.GetLength(1);
            char[,][,] rendered = new char[rows, columns][,];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    rendered[row, column] = RenderTile(maze[row, column]);
                }
            }
            int rowsX3 = rows * 3;
            int columnsX3 = columns * 3;
            StringBuilder stringBuilder = new();
            for (int row = 0; row < rowsX3; row++)
            {
                for (int column = 0; column < columnsX3; column++)
                {
                    int tileRow = row / 3;
                    int tileColumn = column / 3;
                    int renderRow = row % 3;
                    int renderColumn = column % 3;
                    stringBuilder.Append(rendered[tileRow, tileColumn][renderRow, renderColumn]);
                }
                stringBuilder.AppendLine();
            }
            string render = stringBuilder.ToString();
            return render;
        }
    }
}


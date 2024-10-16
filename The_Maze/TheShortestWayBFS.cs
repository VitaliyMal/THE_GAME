//namespace The_Maze

// НЕ РАБОТАЕТ - РАЗОБРАТЬСЯ
{
    public class BFS
    {
        private readonly Maze.Tile[,] _maze;
        private readonly int _rows;
        private readonly int _columns;

        public BFS(Maze.Tile[,] maze)
        {
            _maze = maze;
            _rows = maze.GetLength(0);
            _columns = maze.GetLength(1);
        }

        public (List<(int, int)> path, int steps) FindShortestPath((int startRow, int startCol) start, (int endRow, int endCol) end)
        {
            if (_maze[start.startRow, start.startCol].HasFlag(Maze.Tile.Null) ||
                _maze[end.endRow, end.endCol].HasFlag(Maze.Tile.Null))
            {
                return (new List<(int, int)>(), -1); // Start or End point is blocked
            }

            var queue = new Queue<Node>();
            var visited = new bool[_rows, _columns];
            var parent = new (int, int)[_rows, _columns];
            var directions = new (int dRow, int dCol)[]
            {
                (1, 0), // Down
                (-1, 0), // Up
                (0, 1), // Right
                (0, -1) // Left
            };

            queue.Enqueue(new Node(start.startRow, start.startCol));
            visited[start.startRow, start.startCol] = true;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                // Check if reached the end
                if (current.Row == end.endRow && current.Column == end.endCol)
                {
                    var path = BuildPath(parent, start, end);
                    return (path, path.Count - 1); // Return path and step count (steps = edges = path.Count - 1)
                }

                foreach (var (dRow, dCol) in directions)
                {
                    int newRow = current.Row + dRow;
                    int newCol = current.Column + dCol;

                    if (IsValidMove(newRow, newCol, visited))
                    {
                        visited[newRow, newCol] = true;
                        parent[newRow, newCol] = (current.Row, current.Column);
                        queue.Enqueue(new Node(newRow, newCol));
                    }
                }
            }

            return (new List<(int, int)>(), -1); // No path found, return empty path and -1 as steps
        }

        private bool IsValidMove(int row, int col, bool[,] visited)
        {
            // Check if within bounds, not visited, and not a wall
            return row >= 0 && row < _rows && col >= 0 && col < _columns &&
                   !_maze[row, col].HasFlag(Maze.Tile.Null) && !visited[row, col];
        }

        private List<(int, int)> BuildPath((int, int)[,] parent, (int startRow, int startCol) start, (int endRow, int endCol) end)
        {
            var path = new List<(int, int)>();
            var (row, col) = end;

            // Build path by backtracking from end to start
            while (row != start.startRow || col != start.startCol)
            {
                path.Add((row, col));
                (row, col) = parent[row, col];
            }
            path.Add((start.startRow, start.startCol));
            path.Reverse(); // Reverse to get path from start to end

            return path;
        }

        private class Node
        {
            public int Row { get; }
            public int Column { get; }

            public Node(int row, int column)
            {
                Row = row;
                Column = column;
            }
        }
    }
}




//Возврат кортежа: Метод FindShortestPath возвращает кортеж, состоящий из списка координат пути и целочисленного значения, представляющего количество шагов.

//Количество шагов: После построения пути с помощью метода BuildPath, мы возвращаем количество шагов, используя path.Count - 1, так как количество шагов равно количеству рёбер на пути.

//Возврат значения при отсутствии пути: Если путь не найден, метод возвращает пустой список и -1 в качестве числа шагов, чтобы указать, что путь отсутствует.

//Вызывая метод FindShortestPath, получаем путь и количество шагов, что будет полезно для дальнейшего анализа. 
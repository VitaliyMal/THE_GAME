using System.Collections.Generic;

namespace The_Maze
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

        public (List<(int, int)> path, int steps) FindShortestPath((int, int) start, (int, int) end)
        {
            var queue = new Queue<((int, int) position, List<(int, int)> path)>();
            var visited = new HashSet<(int, int)>();

            queue.Enqueue((start, new List<(int, int)> { start }));
            visited.Add(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                var currentPosition = current.position;
                var currentPath = current.path;

                if (currentPosition == end)
                {
                    return (currentPath, currentPath.Count - 1);
                }

                foreach (var neighbor in GetNeighbors(currentPosition))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        var newPath = new List<(int, int)>(currentPath) { neighbor };
                        queue.Enqueue((neighbor, newPath));
                    }
                }
            }

            return (null, -1); // No path found
        }

        private IEnumerable<(int, int)> GetNeighbors((int row, int column) position)
        {
            var (row, column) = position;

            if (row > 0 && _maze[row, column].HasFlag(Maze.Tile.Up)) yield return (row - 1, column); // Up
            if (row < _rows - 1 && _maze[row, column].HasFlag(Maze.Tile.Down)) yield return (row + 1, column); // Down
            if (column > 0 && _maze[row, column].HasFlag(Maze.Tile.Left)) yield return (row, column - 1); // Left
            if (column < _columns - 1 && _maze[row, column].HasFlag(Maze.Tile.Right)) yield return (row, column + 1); // Right
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading;

namespace The_Maze
{
    public class AI
    {
        private readonly Maze.Tile[,] maze;
        private int row;
        private int column;
        private readonly HashSet<(int, int)> visitedLocations; // Хранит все посещённые клетки
        private readonly Stack<(int row, int column, List<Direction> availableDirections)> pathStack;

        public enum Strategy
        {
            FollowRightWall,
            FollowLeftWall,
            RandomSituation
        }

        private Strategy strategy;

        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }

        public AI(Maze.Tile[,] maze, int startRow, int startColumn, Strategy strategy)
        {
            this.maze = maze;
            this.row = startRow;
            this.column = startColumn;
            this.strategy = strategy;
            visitedLocations = new HashSet<(int, int)>();
            pathStack = new Stack<(int, int, List<Direction>)>();
            visitedLocations.Add((row, column)); // Добавляем начальную позицию в посещённые
            pathStack.Push((row, column, GetAvailableDirections()));
        }

        public void Move()
        {
            switch (strategy)
            {
                case Strategy.FollowRightWall:
                    FollowRightWall();
                    break;
                case Strategy.FollowLeftWall:
                    FollowLeftWall();
                    break;
                case Strategy.RandomSituation:
                    MoveRandomly();
                    break;
            }
        }

        private void FollowRightWall()
        {
            while (true)
            {
                CheckVictoryCondition(); // Проверка на победу
                MoveBasedOnStrategy();
                Thread.Sleep(50);
            }
        }

        private void FollowLeftWall()
        {
            while (true)
            {
                CheckVictoryCondition(); // Проверка на победу
                MoveBasedOnStrategy();
                Thread.Sleep(50);
            }
        }

        private void MoveRandomly()
        {
            while (true)
            {
                CheckVictoryCondition(); // Проверка на победу
                MoveBasedOnStrategy();
                Thread.Sleep(50);
            }
        }

        private void MoveBasedOnStrategy()
        {
            List<Direction> availableDirections = GetAvailableDirections();
            List<Direction> unvisitedDirections = new List<Direction>();

            foreach (var direction in availableDirections)
            {
                var (nextRow, nextColumn) = GetNextPosition(direction);
                if (!visitedLocations.Contains((nextRow, nextColumn)))
                {
                    unvisitedDirections.Add(direction);
                }
            }

            if (unvisitedDirections.Count > 0)
            {
                // Выбор случайного непосещённого направления
                Random rnd = new Random();
                Direction direction = unvisitedDirections[rnd.Next(unvisitedDirections.Count)];
                Move(direction);
                visitedLocations.Add((row, column)); // Добавляем текущую позицию в посещённые
                pathStack.Push((row, column, GetAvailableDirections())); // Запоминаем путь
            }
            else if (pathStack.Count > 0)
            {
                // Возврат к последней развилке
                (row, column, List<Direction> lastAvailableDirections) = pathStack.Pop();
                // Если есть не посещенные направления, выбираем одно из них
                List<Direction> lastUnvisitedDirections = lastAvailableDirections.FindAll(d =>
                {
                    var (nextRow, nextColumn) = GetNextPosition(d);
                    return !visitedLocations.Contains((nextRow, nextColumn));
                });

                if (lastUnvisitedDirections.Count > 0)
                {
                    Random rnd = new Random();
                    Direction direction = lastUnvisitedDirections[rnd.Next(lastUnvisitedDirections.Count)];
                    Move(direction);
                }
            }
        }

        private List<Direction> GetAvailableDirections()
        {
            List<Direction> directions = new List<Direction>();
            if (CanMove(Direction.Up)) directions.Add(Direction.Up);
            if (CanMove(Direction.Down)) directions.Add(Direction.Down);
            if (CanMove(Direction.Left)) directions.Add(Direction.Left);
            if (CanMove(Direction.Right)) directions.Add(Direction.Right);
            return directions;
        }

        private void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    row--;
                    break;
                case Direction.Down:
                    row++;
                    break;
                case Direction.Left:
                    column--;
                    break;
                case Direction.Right:
                    column++;
                    break;
            }
            Console.SetCursorPosition(column * 3 + 1, row * 3 + 1);
            Console.Write("*");
        }

        private bool CanMove(Direction direction)
        {
            return direction switch
            {
                Direction.Right => maze[row, column].HasFlag(Maze.Tile.Right),
                Direction.Down => maze[row, column].HasFlag(Maze.Tile.Down),
                Direction.Left => maze[row, column].HasFlag(Maze.Tile.Left),
                Direction.Up => maze[row, column].HasFlag(Maze.Tile.Up),
                _ => false
            };
        }

        private (int, int) GetNextPosition(Direction direction)
        {
            return direction switch
            {
                Direction.Up => (row - 1, column),
                Direction.Down => (row + 1, column),
                Direction.Left => (row, column - 1),
                Direction.Right => (row, column + 1),
                _ => (row, column)
            };
        }

        private void CheckVictoryCondition()
        {
            // Проверка достижения конца (предполагая, что конец находится в правом нижнем углу)
            if (row == maze.GetLength(0) - 1 && column == maze.GetLength(1) - 1)
            {
                Console.SetCursorPosition(0, maze.GetLength(0) * 3);
                Console.WriteLine("ИИ достиг точки конца. Игра окончена.");
                Environment.Exit(0);
            }
        }
    }
}

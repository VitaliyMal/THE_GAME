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
            row = startRow;
            column = startColumn;
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
            else
            {
                // Если нет доступных непосещённых направлений, возвращаемся к предыдущему шагу
                if (pathStack.Count > 0)
                {
                    (row, column, List<Direction> lastAvailableDirections) = pathStack.Pop();

                    // Получаем непосещенные направления снова
                    List<Direction> lastUnvisitedDirections = new List<Direction>();

                    foreach (var direction in lastAvailableDirections)
                    {
                        var (nextRow, nextColumn) = GetNextPosition(direction);
                        if (!visitedLocations.Contains((nextRow, nextColumn)))
                        {
                            lastUnvisitedDirections.Add(direction);
                        }
                    }

                    // Если есть непосещённые направления, выбираем одно из них
                    if (lastUnvisitedDirections.Count > 0)
                    {
                        Random rnd = new Random();
                        Direction direction = lastUnvisitedDirections[rnd.Next(lastUnvisitedDirections.Count)];
                        Move(direction);
                    }
                    else
                    {
                        // Если всё равно не нашли, продолжаем искать среди всех доступных направлений
                        availableDirections.AddRange(GetAvailableDirections());
                        if (availableDirections.Count > 0)
                        {
                            Random rnd = new Random();
                            Direction direction = availableDirections[rnd.Next(availableDirections.Count)];
                            Move(direction);
                        }
                        else
                        {
                            Console.WriteLine("Стек пуст - не знаю куда идти!"); // Такого не должно быть, но на всякий случай
                            Thread.Sleep(50);
                        }
                    }
                }
                else
                {
                    // Если стек пуст, ищем любые доступные направления
                    List<Direction> directions = GetAvailableDirections();
                    if (directions.Count > 0)
                    {
                        Random rnd = new Random();
                        Direction direction = directions[rnd.Next(directions.Count)];
                        Move(direction);
                    }
                    else
                    {
                        Console.WriteLine("Стек пуст - не знаю куда идти!");
                        Thread.Sleep(50);
                    }
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

            // Проверка, чтобы оставаться в пределах консоли
            if (row >= 0 && row < maze.GetLength(0) && column >= 0 && column < maze.GetLength(1))
            {
                Console.SetCursorPosition(column * 3 + 1, row * 3 + 1);
                Console.Write("*");
            }
            else
            {
                // Если выход за границы, можно добавить логику для возврата назад или игнорирования
                // Например: row и column нельзя менять при выходе за границы
                switch (direction)
                {
                    case Direction.Up: row++; break;
                    case Direction.Down: row--; break;
                    case Direction.Left: column++; break;
                    case Direction.Right: column--; break;
                }
            }
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

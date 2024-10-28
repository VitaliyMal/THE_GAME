namespace The_Maze
{
    public class AI
    {
        private readonly Maze.Tile[,] maze;
        private int row;
        private int column;
        private readonly HashSet<(int, int)> visitedLocations; // Хранит все посещённые клетки
        private readonly Stack<(int row, int column, List<Direction> availableDirections)> pathStack;
        public int moveCount; // Счетчик количества ходов
        public int steps; // кратчайший путь к выходу
        private bool reachedEnd; // Флаг достижения конца лабиринта
        public bool end;

        private int currentDirection;

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

        public AI(Maze.Tile[,] maze, int startRow, int startColumn, Strategy strategy, int steps, bool end)
        {
            this.maze = maze;
            row = startRow;
            column = startColumn;
            this.strategy = strategy;
            visitedLocations = new HashSet<(int, int)>();
            pathStack = new Stack<(int, int, List<Direction>)>();
            visitedLocations.Add((row, column)); // Добавляем начальную позицию в посещённые
            pathStack.Push((row, column, GetAvailableDirections()));
            moveCount = 0; // Инициализация счетчика
            this.steps = steps;
            this.end = end;
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
            while (true && !reachedEnd&& end) // Цикл продолжается, пока не достигли конца
            {
                Navigate(Strategy.FollowRightWall);
            }
            if (end)
            {
                //Console.Clear();
                //Console.WriteLine("Игра прервана.");
                Environment.Exit(0);
            }
        }

        private void FollowLeftWall()
        {
            while (true && !reachedEnd && end) // Цикл продолжается, пока не достигли конца
            {
                Navigate(Strategy.FollowLeftWall);
            }
            if (end)
            {
                //Console.Clear();
                //Console.WriteLine("Игра прервана.");
                Environment.Exit(0);
            }
        }

        private void MoveRandomly()
        {
            while (true && !reachedEnd && end) // Цикл продолжается, пока не достигли конца
            {
                CheckVictoryCondition(); // Проверка на победу
                MoveBasedOnStrategy();
                Thread.Sleep(50);
                // Проверка прерывания по Escape
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    reachedEnd = true;
                    end = true;
                    Console.Clear();
                    Console.WriteLine("Игра прервана.");
                    Environment.Exit(0);
                    //return; // Выход из метода
                }
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

                    // Получаем непосещённые направления снова
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
                        // Если ничего не нашли, продолжаем искать среди всех доступных направлений
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

            // Передвигаем маркер на позиции
            if (row >= 0 && row < maze.GetLength(0) && column >= 0 && column < maze.GetLength(1))
            {
                Console.SetCursorPosition(column * 3 + 1, row * 3 + 1);
                Console.Write("*");
                moveCount++; // Увеличиваем счетчик движений
            }
            else
            {
                // Если выход за границы, игнорируем движение
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
                reachedEnd = true; // Устанавливаем флаг достижения конца
                Console.SetCursorPosition(0, (maze.GetLength(0) * 3)+12);
                //Console.Clear();
                Console.WriteLine($"Кратчайшее количество ходов до конечной точки: {steps}");
                Console.WriteLine("ИИ достиг точки конца. Игра окончена.");
                Console.WriteLine($"Количество совершённых ходов: {moveCount}"); // Выводим количество ходов
                //Environment.Exit(0);
            }
        }


        /// <summary>
        /// Простой обход вдоль выбранной стены
        /// </summary>

        // Directions represented as (row change, column change)
        private static readonly (int, int)[] Directions =
        {
            (0, 1),   // Right
            (1, 0),   // Down
            (0, -1),  // Left
            (-1, 0)   // Up
        };

        public void Navigate(Strategy strategy)
        {
            while (row != maze.GetLength(0) - 1 || column != maze.GetLength(1) - 1)
            {
                // Проверка, чтобы избежать выхода за пределы консоли
                int cursorLeft = column * 3 + 1;
                int cursorTop = row * 3 + 1;

                if (cursorLeft >= 0 && cursorLeft < Console.WindowWidth && cursorTop >= 0 && cursorTop < Console.WindowHeight)
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    Console.Write("*");
                    moveCount++; // Увеличиваем счетчик движений
                    MoveAlongWall(strategy);
                    Thread.Sleep(50);
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        reachedEnd = true;
                        end = true;
                        Console.Clear();
                        Console.WriteLine("Игра прервана.");
                        return;  // Выход из метода
                    }
                }
            }
            CheckVictoryCondition();
        }

        private void MoveAlongWall(Strategy strategy)
        {
            int x = 0;
            int y = 0;
            switch (strategy)
            {
                case Strategy.FollowRightWall:
                    x = 1;
                    y = 3;
                    break;
                case Strategy.FollowLeftWall:
                    x = 3;
                    y = 1;
                    break;

            }
            // Поворачиваем сначала в выбранную сторону
            int nextDirection = (currentDirection + x) % 4;
            //int currentDirection = (currentDirection + 1) % 4; // Поворот направо
            if (CanMoveTo(nextDirection))
            {
                currentDirection = nextDirection; // Обновляем направление
            }
            else if (CanMoveTo(currentDirection)) // Если можем двигаться в текущем направлении
            {
                // Ничего не делаем, просто движемся
            }
            else // Если не можем двигаться ни в одном направлении
            {
                // Если упираемся в стену, поворачиваем
                currentDirection = (currentDirection + y) % 4;
                //currentDirection = (currentDirection + 3) % 4; // Поворот налево
                // Проверяем, можем ли мы двигаться в новом направлении
                if (!CanMoveTo(currentDirection))
                {
                    // Если мы все равно не можем двигаться, нужно попытаться вернуться обратно
                    Backtrack();
                }
            }
            // Двинуться в текущее направление
            MoveInCurrentDirection();
        }

        private bool CanMoveTo(int direction)
        {
            int newRow = row + Directions[direction].Item1;
            int newColumn = column + Directions[direction].Item2;
            return IsInBounds(newRow, newColumn) && maze[row, column].HasFlag(GetTileMovement(direction));
        }

        private bool IsInBounds(int newRow, int newColumn)
        {
            return newRow >= 0 && newRow < maze.GetLength(0) && newColumn >= 0 && newColumn < maze.GetLength(1);
        }

        private Maze.Tile GetTileMovement(int direction)
        {
            return direction switch
            {
                0 => Maze.Tile.Right,
                1 => Maze.Tile.Down,
                2 => Maze.Tile.Left,
                3 => Maze.Tile.Up,
                _ => 0
            };
        }

        private void MoveInCurrentDirection()
        {
            row += Directions[currentDirection].Item1;
            column += Directions[currentDirection].Item2;
        }

        private void Backtrack()
        {
            // Двигаемся назад в противоположном направлении
            currentDirection = (currentDirection + 2) % 4; // Поворачиваем на 180 градусов
            row += Directions[currentDirection].Item1;
            column += Directions[currentDirection].Item2;
            // Попробуем вернуться в исходное направление
            currentDirection = (currentDirection + 2) % 4; // Поворачиваем обратно к оригиналу
        }
    }
}

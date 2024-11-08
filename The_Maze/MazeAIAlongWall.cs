//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using The_Maze;


////добавить в движок
////MazeAIRightWall ai = new MazeAIRightWall(maze);
////ai.Navigate();

//namespace The_Maze
//{
//    public class MazeAIAlongWall
//    {
//        private int row;
//        private int column;
//        private readonly Maze.Tile[,] maze;
//        private int currentDirection;
//        private int moveCount; // Счетчик количества ходов

//        // Directions represented as (row change, column change)
//        private static readonly (int, int)[] Directions =
//        {
//            (0, 1),   // Right
//            (1, 0),   // Down
//            (0, -1),  // Left
//            (-1, 0)   // Up
//        };

//        public MazeAIAlongWall(Maze.Tile[,] maze)
//        {
//            this.maze = maze;
//            this.row = 0; // Начало в верхнем левом углу
//            this.column = 0; // Начало в верхнем левом углу
//            this.currentDirection = 0; // Начало направлено вправо
//            moveCount = 0; // Инициализация счетчика
//        }

//        public void Navigate()
//        {
//            while (row != maze.GetLength(0) - 1 || column != maze.GetLength(1) - 1)
//            {
//                // Проверка, чтобы избежать выхода за пределы консоли
//                int cursorLeft = column * 3 + 1;
//                int cursorTop = row * 3 + 1;

//                if (cursorLeft >= 0 && cursorLeft < Console.WindowWidth && cursorTop >= 0 && cursorTop < Console.WindowHeight)
//                {
//                    Console.SetCursorPosition(cursorLeft, cursorTop);
//                    Console.Write("*");
//                    moveCount++; // Увеличиваем счетчик движений
//                    MoveAlongWall();
//                    Thread.Sleep(50);
//                }
//            }
//            Console.Clear();
//            Console.WriteLine("ИИ достиг точки конца. Игра окончена.");
//            Console.WriteLine($"Количество совершённых ходов: {moveCount}"); // Выводим количество ходов
//        }

//        private void MoveAlongWall()
//        {
//            // Поворачиваем налево сначала
//            int nextDirection = (currentDirection + 3) % 4; // Поворот налево
//            //int currentDirection = (currentDirection + 1) % 4; // Поворот направо
//            if (CanMoveTo(nextDirection))
//            {
//                currentDirection = nextDirection; // Обновляем направление
//            }
//            else if (CanMoveTo(currentDirection)) // Если можем двигаться в текущем направлении
//            {
//                // Ничего не делаем, просто движемся
//            }
//            else // Если не можем двигаться ни в одном направлении
//            {
//                // Если упираемся в стену, поворачиваем направо
//                currentDirection = (currentDirection + 1) % 4; // Поворот направо
//                //currentDirection = (currentDirection + 3) % 4; // Поворот налево
//                // Проверяем, можем ли мы двигаться в новом направлении
//                if (!CanMoveTo(currentDirection))
//                {
//                    // Если мы все равно не можем двигаться, нужно попытаться вернуться обратно
//                    Backtrack();
//                }
//            }
//            // Двинуться в текущее направление
//            MoveInCurrentDirection();
//        }

//        private bool CanMoveTo(int direction)
//        {
//            int newRow = row + Directions[direction].Item1;
//            int newColumn = column + Directions[direction].Item2;
//            return IsInBounds(newRow, newColumn) && maze[row, column].HasFlag(GetTileMovement(direction));
//        }

//        private bool IsInBounds(int newRow, int newColumn)
//        {
//            return newRow >= 0 && newRow < maze.GetLength(0) && newColumn >= 0 && newColumn < maze.GetLength(1);
//        }

//        private Maze.Tile GetTileMovement(int direction)
//        {
//            return direction switch
//            {
//                0 => Maze.Tile.Right,
//                1 => Maze.Tile.Down,
//                2 => Maze.Tile.Left,
//                3 => Maze.Tile.Up,
//                _ => 0
//            };
//        }

//        private void MoveInCurrentDirection()
//        {
//            row += Directions[currentDirection].Item1;
//            column += Directions[currentDirection].Item2;
//        }

//        private void Backtrack()
//        {
//            // Двигаемся назад в противоположном направлении
//            currentDirection = (currentDirection + 2) % 4; // Поворачиваем на 180 градусов
//            row += Directions[currentDirection].Item1;
//            column += Directions[currentDirection].Item2;
//            // Попробуем вернуться в исходное направление
//            currentDirection = (currentDirection + 2) % 4; // Поворачиваем обратно к оригиналу
//        }
//    }
//}



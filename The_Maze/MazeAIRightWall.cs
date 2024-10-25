using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Maze;


//добавить в движок
//AtWall ai = new AtWall(maze);
//ai.Navigate();

namespace The_Maze
{
    public class MazeAIRightWall
    {
        private int row;
        private int column;
        private readonly Maze.Tile[,] maze;
        private int currentDirection;

        // Directions represented as (row change, column change)
        private static readonly (int, int)[] Directions =
        {
            (0, 1),   // Right
            (1, 0),   // Down
            (0, -1),  // Left
            (-1, 0)   // Up
        };

        public MazeAIRightWall(Maze.Tile[,] maze)
        {
            this.maze = maze;
            this.row = 0; // Starting at top-left corner
            this.column = 0; // Starting at top-left corner
            this.currentDirection = 0; // Start facing right
        }

        public void Navigate()
        {
            while (row != maze.GetLength(0) - 1 || column != maze.GetLength(1) - 1)
            {
                // Проверка, чтобы избежать выхода за пределы консоли
                int cursorLeft = column * 3 + 1;
                int cursorTop = row * 3 + 1;

                if (cursorLeft >= 0 && cursorLeft < Console.WindowWidth && cursorTop >= 0 && cursorTop < Console.WindowHeight)
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    MoveAlongWall();
                    Thread.Sleep(150);
                }
            }
            Console.Clear();
            Console.WriteLine("AI reached the end!");
        }

        private void MoveAlongWall()
        {
            // Attempt to turn right first
            int nextDirection = (currentDirection + 1) % 4; // Right turn
            if (CanMoveTo(nextDirection))
            {
                currentDirection = nextDirection; // Update direction to the one turned to
            }
            else if (CanMoveTo(currentDirection)) // If can move in the current direction
            {
                // Do nothing, just move
            }
            else // If can't move in both directions
            {
                // If hitting a wall, backtrack by turning left
                currentDirection = (currentDirection + 3) % 4; // Turn left (or backtrack)
                // As we cannot move, we need to attempt to move in the new direction
                if (CanMoveTo(currentDirection))
                {
                    // Move in the new direction
                }
                else
                {
                    // If we still can't move, we need to continue backtracking until we find a new path
                    Backtrack();
                }
            }
            // Move in the current direction
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
            // Move backward in the opposite direction
            currentDirection = (currentDirection + 2) % 4; // Turn around
            row += Directions[currentDirection].Item1;
            column += Directions[currentDirection].Item2;
            // Attempt the new direction after turning around
            currentDirection = (currentDirection + 2) % 4; // Turn back to original
        }
    }

//по левой стене ниже:

    public class MazeAILeftWall
    {
        private int row;
        private int column;
        private readonly Maze.Tile[,] maze;
        private int currentDirection;

        // Directions represented as (row change, column change)
        private static readonly (int, int)[] Directions =
        {
            (0, -1),  // Left
            (1, 0),   // Down
            (0, 1),   // Right
            (-1, 0)   // Up
        };

        public MazeAILeftWall(Maze.Tile[,] maze)
        {
            this.maze = maze;
            this.row = 0; // Starting at top-left corner
            this.column = 0; // Starting at top-left corner
            this.currentDirection = 0; // Start facing left
        }

        public void Navigate()
        {
            while (row != maze.GetLength(0) - 1 || column != maze.GetLength(1) - 1)
            {
                // Проверка, чтобы избежать выхода за пределы консоли
                int cursorLeft = column * 3 + 1;
                int cursorTop = row * 3 + 1;

                if (cursorLeft >= 0 && cursorLeft < Console.WindowWidth && cursorTop >= 0 && cursorTop < Console.WindowHeight)
                {
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                    MoveAlongWall();
                    Thread.Sleep(150);
                }
            }
            Console.Clear();
            Console.WriteLine("AI reached the end!");
        }

        private void MoveAlongWall()
        {
            // Attempt to turn left first
            int nextDirection = (currentDirection + 1) % 4; // Left turn
            if (CanMoveTo(nextDirection))
            {
                currentDirection = nextDirection; // Update direction to the one turned to
            }
            else if (CanMoveTo(currentDirection)) // If can move in the current direction
            {
                // Do nothing, just move
            }
            else // If can't move in both directions
            {
                // If hitting a wall, backtrack by turning right
                currentDirection = (currentDirection + 3) % 4; // Turn right (or backtrack)
                // As we cannot move, we need to attempt to move in the new direction
                if (CanMoveTo(currentDirection))
                {
                    // Move in the new direction
                }
                else
                {
                    // If we still can't move, we need to continue backtracking until we find a new path
                    Backtrack();
                }
            }
            // Move in the current direction
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
                0 => Maze.Tile.Left,
                1 => Maze.Tile.Down,
                2 => Maze.Tile.Right,
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
            // Move backward in the opposite direction
            currentDirection = (currentDirection + 2) % 4; // Turn around
            row += Directions[currentDirection].Item1;
            column += Directions[currentDirection].Item2;
            // Attempt the new direction after turning around
            currentDirection = (currentDirection + 2) % 4; // Turn back to original
        }
    }
}


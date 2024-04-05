using System;
using System.Collections.Generic;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        // Constants for game settings
        const int Width = 20;
        const int Height = 10;
        const char SnakeSymbol = 'O';
        const char FoodSymbol = '@';

        // Enum for direction
        enum Direction { Up, Down, Left, Right };

        // Game variables
        static int snakeX, snakeY;
        static Direction direction;
        static bool gameover;
        static Random random = new Random();
        static List<Tuple<int, int>> snakeBody = new List<Tuple<int, int>>();
        static Tuple<int, int> foodPosition;
        static int gameSpeed;

        // Difficulty levels
        enum Difficulty { Easy, Normal, Hard };

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.Title = "Snake Game";

            Difficulty difficulty = ChooseDifficulty();

            SetGameSpeed(difficulty);

            InitializeGame();

            while (!gameover)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    HandleInput(key);
                }

                MoveSnake();
                CheckCollision();
                Draw();
                Thread.Sleep(gameSpeed);
            }

            Console.SetCursorPosition(0, Height);
            Console.WriteLine("Game Over! Press any key to exit.");
            Console.ReadKey();
        }

        static Difficulty ChooseDifficulty()
        {
            Console.WriteLine("Choose difficulty: (E)asy, (N)ormal, (H)ard");
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
            } while (key.Key != ConsoleKey.E && key.Key != ConsoleKey.N && key.Key != ConsoleKey.H);

            switch (key.Key)
            {
                case ConsoleKey.E:
                    return Difficulty.Easy;
                case ConsoleKey.N:
                    return Difficulty.Normal;
                case ConsoleKey.H:
                    return Difficulty.Hard;
                default:
                    return Difficulty.Normal; // Default to normal if invalid input
            }
        }


        static void SetGameSpeed(Difficulty difficulty)
        {
            switch (difficulty)
            {
                case Difficulty.Easy:
                    gameSpeed = 200; // Adjust according to your preference
                    break;
                case Difficulty.Normal:
                    gameSpeed = 100; // Adjust according to your preference
                    break;
                case Difficulty.Hard:
                    gameSpeed = 50; // Adjust according to your preference
                    break;
                default:
                    gameSpeed = 100;
                    break;
            }
        }

        static void InitializeGame()
        {
            snakeX = Width / 2;
            snakeY = Height / 2;
            direction = Direction.Right;
            gameover = false;
            snakeBody.Clear();
            snakeBody.Add(Tuple.Create(snakeX, snakeY));
            GenerateFood();
        }

        static void GenerateFood()
        {
            int foodX = random.Next(2, Width - 2); // Adjusted range to ensure space from left and right walls
            int foodY = random.Next(1, Height - 1); // Adjusted range to ensure space from top and bottom walls

            foodPosition = Tuple.Create(foodX, foodY);

            if (snakeBody.Contains(foodPosition))
            {
                GenerateFood();
            }
        }


        static void MoveSnake()
        {
            Tuple<int, int> newHead = null;

            switch (direction)
            {
                case Direction.Up:
                    newHead = Tuple.Create(snakeBody[0].Item1, snakeBody[0].Item2 - 1);
                    break;
                case Direction.Down:
                    newHead = Tuple.Create(snakeBody[0].Item1, snakeBody[0].Item2 + 1);
                    break;
                case Direction.Left:
                    newHead = Tuple.Create(snakeBody[0].Item1 - 1, snakeBody[0].Item2);
                    break;
                case Direction.Right:
                    newHead = Tuple.Create(snakeBody[0].Item1 + 1, snakeBody[0].Item2);
                    break;
            }

            if (newHead.Item1 == foodPosition.Item1 && newHead.Item2 == foodPosition.Item2)
            {
                GenerateFood();
            }
            else
            {
                snakeBody.RemoveAt(snakeBody.Count - 1);
            }

            snakeBody.Insert(0, newHead);
        }

        static void CheckCollision()
        {
            Tuple<int, int> head = snakeBody[0];

            if (head.Item1 < 0 || head.Item1 >= Width || head.Item2 < 0 || head.Item2 >= Height)
                gameover = true;

            for (int i = 1; i < snakeBody.Count; i++)
            {
                if (head.Item1 == snakeBody[i].Item1 && head.Item2 == snakeBody[i].Item2)
                {
                    gameover = true;
                    break;
                }
            }
        }

        static void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    if (direction != Direction.Down)
                        direction = Direction.Up;
                    break;
                case ConsoleKey.DownArrow:
                    if (direction != Direction.Up)
                        direction = Direction.Down;
                    break;
                case ConsoleKey.LeftArrow:
                    if (direction != Direction.Right)
                        direction = Direction.Left;
                    break;
                case ConsoleKey.RightArrow:
                    if (direction != Direction.Left)
                        direction = Direction.Right;
                    break;
            }
        }

        static void Draw()
        {
            // Clear previous snake segments and redraw collision grid
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("#"); // Wall
                    }
                    else
                    {
                        Console.SetCursorPosition(x, y);
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(".");
                    }
                }
            }

            // Draw food (clamped to console bounds)
            int foodX = Math.Clamp(foodPosition.Item1, 1, Width - 2);
            int foodY = Math.Clamp(foodPosition.Item2, 1, Height - 2);
            Console.SetCursorPosition(foodX, foodY);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(FoodSymbol);

            // Draw snake (clamped to console bounds)
            foreach (var segment in snakeBody)
            {
                int x = Math.Clamp(segment.Item1, 1, Width - 2);
                int y = Math.Clamp(segment.Item2, 1, Height - 2);
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(SnakeSymbol);
            }
        }
    }
}

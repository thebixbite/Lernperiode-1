using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MathQuiz
{
    class Program
    {
        enum Difficulty
        {
            Easy,
            Medium,
            Hard
        }

        static int highScore = 0;
        static readonly object consoleLock = new();
        static int lastHighScoreTextLength = 0;

        static async Task ShowCountdownAsync(CancellationToken cancellationToken)
        {
            for (int remainingSeconds = 10; remainingSeconds > 0; remainingSeconds--)
            {
                Console.Write($"\rOnly {remainingSeconds} seconds left... ");

                try
                {
                    await Task.Delay(1000, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }

            Console.WriteLine();
        }

        static void DrawHighScore()
        {
            lock (consoleLock)
            {
                string text = $"Highest Score: {highScore}";
                int top = 0;
                int left = Math.Max(Console.WindowWidth - text.Length, 0);

                int prevLeft = Console.CursorLeft;
                int prevTop = Console.CursorTop;
                bool prevVisible = Console.CursorVisible;

                string padded = text;
                if (lastHighScoreTextLength > text.Length)
                {
                    padded = text + new string(' ', lastHighScoreTextLength - text.Length);
                }

                try
                {
                    Console.CursorVisible = false;
                    Console.SetCursorPosition(left, top);
                    Console.Write(padded);
                    lastHighScoreTextLength = text.Length;
                }
                catch
                {
                }
                finally
                {
                    try
                    {
                        Console.SetCursorPosition(Math.Min(prevLeft, Console.WindowWidth - 1), Math.Min(prevTop, Console.BufferHeight - 1));
                    }
                    catch { }
                    Console.CursorVisible = prevVisible;
                }
            }
        }

        static async Task Main()
        {
            Random random = new Random();

            Console.WriteLine("Welcome");

            while (true)
            {
                Difficulty difficulty;
                while (true)
                {
                    Console.WriteLine("\nSelect difficulty");
                    Console.WriteLine("1) Easy ");
                    Console.WriteLine("2) Medium ");
                    Console.WriteLine("3) Hard ");
                    Console.Write("Choice (1/2/3): ");

                    string? choice = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(choice))
                    {
                        Console.WriteLine("Please enter a valid number.");
                        continue;
                    }

                    choice = choice.Trim().ToLowerInvariant();
                    if (choice == "1" || choice == "Easy")
                    {
                        difficulty = Difficulty.Easy;
                        break;
                    }
                    else if (choice == "2" || choice == "Medium")
                    {
                        difficulty = Difficulty.Medium;
                        break;
                    }
                    else if (choice == "3" || choice == "Hard")
                    {
                        difficulty = Difficulty.Hard;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid number.");
                    }
                }

                int totalScore = 0;

                while (true)
                {
                    DrawHighScore();

                    
                    int minNumber = 1;
                    int maxNumber = 99;
                    List<int> allowedOps = new() { 1, 2 };
                    if (difficulty == Difficulty.Easy)
                    {
                        maxNumber = 9;
                        allowedOps = new() { 1, 2 };
                    }
                    else if (difficulty == Difficulty.Medium)
                    {
                        maxNumber = 99;
                        allowedOps = new() { 1, 2 };
                    }
                    else if (difficulty == Difficulty.Hard)
                    {
                        maxNumber = 99;
                        allowedOps = new() { 1, 2, 3 };
                    }

                    int operationType = allowedOps[random.Next(allowedOps.Count)];
                    int number1 = random.Next(minNumber, maxNumber + 1);
                    int number2 = random.Next(minNumber, maxNumber + 1);

                    int correctAnswer = 0;

                    if (operationType == 1)
                    {
                        Console.WriteLine($"Please calculate: {number1} + {number2} = ");
                        correctAnswer = number1 + number2;
                    }
                    else if (operationType == 2)
                    {
                        Console.WriteLine($"Please calculate: {number1} - {number2} = ");
                        correctAnswer = number1 - number2;
                    }
                    else if (operationType == 3)
                    {
                        Console.WriteLine($"Please calculate: {number1} * {number2} = ");
                        correctAnswer = number1 * number2;
                    }

                    string? input = null;

                    using CancellationTokenSource countdownCancellation = new CancellationTokenSource();
                    Task countdownTask = ShowCountdownAsync(countdownCancellation.Token);
                    Task<string?> readTask = Task.Run(() => Console.ReadLine());
                    Task timeoutTask = Task.Delay(10000);

                    Task completedTask = await Task.WhenAny(readTask, timeoutTask);

                    countdownCancellation.Cancel();
                    await countdownTask;

                    if (completedTask == timeoutTask)
                    {
                        Console.WriteLine("\nTime is up! Game Over");
                        Console.WriteLine($"Final score: = {totalScore}");
                        UpdateAndShowHighScore(totalScore, false);
                        break;
                    }

                    input = await readTask;

                    if (!int.TryParse(input,out int enteredNumber))
                    {
                        Console.WriteLine("Please enter a valid number.");
                        continue;
                    }

                    if (enteredNumber == correctAnswer)
                    {
                        totalScore++;
                        Console.WriteLine($"Correct! Score: = {totalScore}");
                        UpdateAndShowHighScore(totalScore, true);
                    }
                    else
                    {
                        Console.WriteLine("Wrong! Game Over");
                        Console.WriteLine($"Final score: {totalScore}");
                        UpdateAndShowHighScore(totalScore, false);
                        break;
                    }
                }

                Console.Write("\nPlay again? (Yes/No): ");
                string? again = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(again) && (again.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase)))
                {
                    Console.Clear();
                    continue;
                }

                break;
            }
        }

        static void UpdateAndShowHighScore(int currentScore, bool announceAtBottom)
        {
            if (currentScore > highScore)
            {
                highScore = currentScore;
                if (announceAtBottom)
                {
                    Console.WriteLine($"New record! Highest score: {highScore}");
                }
            }

            DrawHighScore();
        }
    }
}
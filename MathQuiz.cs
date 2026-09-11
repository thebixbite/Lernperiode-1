using System;
using System.Threading;
using System.Threading.Tasks;

namespace MathQuiz
{
    class Program
    {
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
                int totalScore = 0;
                DrawHighScore();

                while (true)
                {
                    DrawHighScore();

                    int operationType = random.Next(1, 4);
                    int number1 = random.Next(1, 100);
                    int number2 = random.Next(1, 100);

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

                    if (!int.TryParse(input, out int enteredNumber))
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
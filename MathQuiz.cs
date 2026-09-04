using System;
using System.Threading;
using System.Threading.Tasks;


    namespace MathQuiz
{
    class Program
    {
    static async Task ShowCountdownAsync(CancellationToken cancellationToken)
    {
        for (int remainingSeconds = 10; remainingSeconds > 0; remainingSeconds--)
        {
            Console.Write($"\rNoch {remainingSeconds} Sekunden übrig... ");

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

    static async Task Main()

    {
    int totalScore = 0;
Random random = new Random();

Console.WriteLine("Herzlich Willkommen");

while (true)
{
    


int operationType = random.Next(1, 4); // 1: Addition, 2: Subtraction, 3: Multiplication

int number1 = random.Next(1, 100);
int number2 = random.Next(1, 100);

int correctAnswer = 0;

if (operationType == 1)
{

    Console.WriteLine($"Bitte berechnen Sie: {number1} + {number2} = ");
    correctAnswer = number1 + number2;
}
else if (operationType == 2)
{
    Console.WriteLine($"Bitte berechnen Sie: {number1} - {number2} = ");
    correctAnswer = number1 - number2;
}
else if (operationType == 3)
{
    Console.WriteLine($"Bitte berechnen Sie: {number1} * {number2} = ");
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
    Console.WriteLine("\nZeit abgelaufen! Game Over");
    Console.WriteLine($"Ende score: = {totalScore}");
    break;
}

input = await readTask;

if (!int.TryParse(input, out int enteredNumber))
{
    Console.WriteLine("Bitte geben Sie eine gültige Zahl ein.");
    continue;
}


    if (enteredNumber == correctAnswer)
   {

totalScore++;
    Console.WriteLine($"Richtig! Score: = {totalScore}");
   }
   else
   {
       Console.WriteLine("Falsch! Game Over");
    Console.WriteLine($"Ende score: = {totalScore}");
       break;
   }
   }
   }
   }
   }
   
   

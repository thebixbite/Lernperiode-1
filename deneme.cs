using System;
namespace MathQuiz
{
class Program
{
    static void Main(string[] args)
    {


int totalScore = 0;
Random random = new Random();

while (true)
{
int operationType = random.Next(1, 4); // 1: Addition, 2: Subtraction, 3: Multiplication

int number1 = random.Next(1, 100);
int number2 = random.Next(1, 100);

int correctAnswer = 0;

if (operationType == 1)
{

    Console.WriteLine("Herzlich Willkommen");
    Console.Write($"Bitte berechnen Sie: {number1} + {number2} = ");
    correctAnswer = number1 + number2;
}
else if (operationType == 2)
{
    Console.WriteLine("Herzlich Willkommen");
    Console.Write($"Bitte berechnen Sie: {number1} - {number2} = ");
    correctAnswer = number1 - number2;
}
else if (operationType == 3)
{
    Console.WriteLine("Herzlich Willkommen");
    Console.Write($"Bitte berechnen Sie: {number1} * {number2} = ");
    correctAnswer = number1 * number2;
}

    if (!int.TryParse(Console.ReadLine(), out int enteredNumber))
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

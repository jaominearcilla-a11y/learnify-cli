using System;
using System.Collections.Generic;
using System.Linq;

namespace Learnify_prtp;

class Question
{
    public string Text { get; set; }
   
    public string[] Choices { get; set; }
   
    public string CorrectAnswer { get; set; }
   
    public string Difficulty { get; set; }
}

class Program
{
    static Dictionary<string, List<Question>> allQuizzes = new Dictionary<string, List<Question>>();

    static void Main()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.CursorVisible = false;
        Console.Clear();

        Typewriter(">> LEARNIFY CORP (TM) TERMLINK 2026");
        Typewriter(">> INITIALIZING LEARNIFY PROTOCOLS...");

        bool running = true;
        while (running)
        {
            string[] menuOptions = {"[CREATE] NEW QUIZ DATA", "[ACCESS]EXISTING QUIZ", "[EXIT] TERMINAL" };

          if (selected == 0) CreateQuiz();
            else if (selected == 1) SelectAndTakeQuiz();
            else if (selected == 2) running = false;
        }
    }

    static void Typewriter(string text)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            System.Threading.Thread.Sleep(20);
        }
        Console.WriteLine();
    }
 }    
     static int MenuSelector(string title, string[] options) 
    {
        int currentIndex = 0;
        while (true)
        {
            Console.Clear();
            Console.WriteLine(title);
            Console.WriteLine("-------------------------------------");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == currentIndex)
                {
                    
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine($"> {options[i]} ");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.WriteLine($"  {options[i]} ");
                }
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("\n(Use UP/DOWN Arrows & Press ENTER)");

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                currentIndex = (currentIndex == 0) ? options.Length - 1 : currentIndex - 1;
                Console.Beep(1200, 20); 
            }
            else if (key == ConsoleKey.DownArrow)
            {
                currentIndex = (currentIndex == options.Length - 1) ? 0 : currentIndex + 1;
                Console.Beep(1200, 20);
            }
            else if (key == ConsoleKey.Enter)
            {
                Console.Beep(800, 100); 
                return currentIndex;
            }
        }
    }
   static void Typewriter(string text)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            System.Threading.Thread.Sleep(15);
        }
        Console.WriteLine();
    }
   
    static void CreateQuiz()   
    {     
        Console.Clear()
        Console.CursorVisible
        Typewriter(">> ENTER NAME FOR NEW QUIZ:");
        string quizName = Console.ReadLine() ?? "";
        
        if (!allQuizzes.ContainsKey(quizName)) allQuizzes.Add(quizName, new List<Question>());
        // Pick the mode for all the questions
        Console.WriteLine("\nSELECT MODE FOR ALL QUESTIONS IN THIS QUIZ:");
        Console.WriteLine("1: Easy (T/F) | 2: Medium (MCQ) | 3: Hard (ID)");
        Console.Write("SELECTION: ");
        string typeChoice = Console.ReadLine();

        Console.Write(">> HOW MANY QUESTIONS?: ");
        if (!int.TryParse(Console.ReadLine(), out int count)) return;

        // Loop this only ask for answer
        for (int i = 0; i < count; i++)
        {
            Question q = new Question();
            Console.WriteLine($"\n-- ENTRY {i + 1} --");

            Console.Write("INPUT QUESTION: ");
            q.Text = Console.ReadLine();

            if (typeChoice == "1") // EASY
            {
                q.Difficulty = "Easy";
                q.Choices = new string[] { "True", "False" };
                Console.Write("CORRECT ANSWER (T/F): ");
                q.CorrectAnswer = Console.ReadLine().ToUpper();
            }
            else if (typeChoice == "2") // MEDIUM
            {
                q.Difficulty = "Medium";
                q.Choices = new string[4];
                for (int j = 0; j < 4; j++)
                {
                    Console.Write($"CHOICE {(char)('A' + j)}: ");
                    q.Choices[j] = Console.ReadLine();
                }
                Console.Write("CORRECT KEY (A-D): ");
                q.CorrectAnswer = Console.ReadLine().ToUpper();
            }
            else // HARD
            {
                q.Difficulty = "Hard";
                Console.Write("INPUT EXACT CORRECT ANSWER: ");
                q.CorrectAnswer = Console.ReadLine();
            }

            allQuizzes[quizName].Add(q);        
        Typewriter("\n>> DATA COMPILED SUCCESSFULLY.");
        Console.ReadKey();
   }

    static void SelectAndTakeQuiz()
    {
        Console.Clear();
        if (allQuizzes.Count == 0) return;

        var quizNames = allQuizzes.Keys.ToList();
        for (int i = 0; i < quizNames.Count; i++) Console.WriteLine($"{i + 1}. {quizNames[i]}");

        Console.Write("\nSELECTION: ");
        if (int.TryParse(Console.ReadLine(), out int choice) && choice > 0 && choice <= quizNames.Count)
        {
            RunQuiz(quizNames[choice - 1]);
        }
    }

    static void RunQuiz(string name)
    {
        List<Question> questions = allQuizzes[name];
        int score = 0;

        foreach (var q in questions)
        {
            Console.Clear();
            Console.WriteLine($">> ACCESSING: {name} | MODE: {q.Difficulty.ToUpper()} <<");
            Console.WriteLine("-------------------------------------");
            Typewriter(q.Text);

            string userAnswer = "";
            if (q.Difficulty == "Easy")
            {
                Console.WriteLine("A. True\nB. False");
                Console.Write("\nINPUT (A/B): ");
                userAnswer = Console.ReadLine().ToUpper();
                if (userAnswer == "A") userAnswer = "T";
                else if (userAnswer == "B") userAnswer = "F";
            }
            else if (q.Difficulty == "Medium")
            {
                for (int i = 0; i < 4; i++) Console.WriteLine($"{(char)('A' + i)}. {q.Choices[i]}");
                Console.Write("\nSELECTION (A-D): ");
                userAnswer = Console.ReadLine().ToUpper();
            }
            else
            {
                Console.Write("\nTYPE YOUR ANSWER: ");
                userAnswer = Console.ReadLine();
            }

            if (userAnswer.Trim().Equals(q.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\n>> CORRECT.");
                score++;
            }
            else
            {
                Console.WriteLine($"\n>> INCORRECT. ANSWER: {q.CorrectAnswer}");
            }
            Console.WriteLine("PRESS ENTER TO CONTINUE.");
            Console.ReadLine();
        }

        Console.Clear();
        Typewriter("--- FINAL EVALUATION ---");
        Typewriter($"RESULT: {score} OUT OF {questions.Count} CORRECT.");
        Console.ReadKey();
    }
}
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
{}
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
            string[] menuOptions = { "[CREATE] NEW QUIZ DATA", "[ACCESS] EXISTING QUIZ", "[EXIT] TERMINAL" };
            int selected = MenuSelector("=== LEARNIFY TERMINAL - MAIN MENU ===", menuOptions);

            if (selected == 0) CreateQuiz();
            else if (selected == 1) SelectAndTakeQuiz();
            else if (selected == 2) running = false;
        }
    }

    // --- THE ARROW KEY NAVIGATION SYSTEM ---
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
                Console.Beep(); 
            
            else if (key == ConsoleKey.DownArrow)
            {
                currentIndex = (currentIndex == options.Length - 1) ? 0 : currentIndex + 1;
                Console.Beep();
            }
            else if (key == ConsoleKey.Enter)
            {
                Console.Beep(); 
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
        Console.Clear();
        Console.CursorVisible = true;
        Typewriter(">> ENTER NAME FOR NEW QUIZ:");     
        string? quizName = Console.ReadLine();
        
        if (string.IsNullOrEmpty(quizName)) return;
        if (!allQuizzes.ContainsKey(quizName)) allQuizzes.Add(quizName, new List<Question>());

        string[] modes = { "Easy (T/F)", "Medium (MCQ)", "Hard (ID)" };
        int typeChoice = MenuSelector("SELECT MODE FOR ALL QUESTIONS:", modes);

        Console.Write(">> HOW MANY QUESTIONS?: ");
        if (!int.TryParse(Console.ReadLine(), out int count)) return;

        for (int i = 0; i < count; i++)
        {
            Question q = new Question();
                Console.WriteLine($"\n-- ENTRY {i + 1} --");
            Console.Write("INPUT QUESTION: ");
            q.Text = Console.ReadLine();

             if (typeChoice == 0) // Easy
            {
                q.Difficulty = "Easy";
                q.Choices = new string[] { "True", "False" };
                Console.Write("CORRECT ANSWER (T/F): ");
                q.CorrectAnswer = Console.ReadLine().ToUpper();
            }
            else if (typeChoice == 1) // Medium
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
            else // Hard
            {
                q.Difficulty = "Hard";
                Console.Write("INPUT EXACT CORRECT ANSWER: ");
                q.CorrectAnswer = Console.ReadLine();
            }
            allQuizzes[quizName].Add(q);
        }
        Console.CursorVisible = false;
        Typewriter("\n>> DATA COMPILED SUCCESSFULLY.");
        Console.ReadKey();
    }

    static void SelectAndTakeQuiz()
    {
        if (allQuizzes.Count == 0)
        {
            Console.Clear();
            Typewriter(">> ERROR: NO QUIZ DATA FOUND.");
            Console.ReadKey();
            return;
        }

        var quizNames = allQuizzes.Keys.ToArray();
        int choice = MenuSelector("SELECT A QUIZ TO BEGIN:", quizNames);
        RunQuiz(quizNames[choice]);
    }

    static void RunQuiz(string name)
    {
        List<Question> questions = allQuizzes[name];
        int score = 0;

        foreach (var q in questions)
            {
            string userAnswer = "";
            if (q.Difficulty == "Easy" || q.Difficulty == "Medium")
            {
                // For quiz choices same arrow key selector
                int choiceIndex = MenuSelector($">> {name} | {q.Difficulty.ToUpper()} <<\n\n{q.Text}", q.Choices);
                
                if (q.Difficulty == "Easy")
                    userAnswer = (choiceIndex == 0) ? "T" : "F";
                else
                    userAnswer = ((char)('A' + choiceIndex)).ToString();
            }
        else
        {
            Console.Clear();
            Console.CursorVisible = true;
            Console.WriteLine($">> {name} | HARD <<\n------------------");
            Typewriter(q.Text);
            
            Console.Write("\nTYPE YOUR ANSWER: ");
            userAnswer = Console.ReadLine();
            Console.CursorVisible = false;
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
            Console.WriteLine("PRESS ANY KEY TO CONTINUE.");
            Console.ReadKey(true);
        }

        Console.Clear();
        Typewriter("--- FINAL EVALUATION ---");
        Typewriter($"RESULT: {score} OUT OF {questions.Count} CORRECT.");
        Console.ReadKey();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Learnify_prtp;

// --- DATA STRUCTURES ---
class Question
{
    public string Text { get; set; } = string.Empty;
    public string[] Choices { get; set; } = Array.Empty<string>();
    public string CorrectAnswer { get; set; } = string.Empty;
    public string Difficulty { get; set; } = "Easy";
}

class Program
{
    static Dictionary<string, List<Question>> allQuizzes = new Dictionary<string, List<Question>>();

    static void Main()
    {
        Console.Title = "LEARNIFY TERMINAL";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.CursorVisible = false;
        Console.Clear();

        Typewriter(">> LEARNIFY CORP (TM) TERMLINK 2026");
        Typewriter(">> INITIALIZING LEARNIFY PROTOCOLS...");
        Thread.Sleep(500);

        bool running = true;
        while (running)
        {
            while (Console.KeyAvailable) Console.ReadKey(true);

            string[] menuOptions = { 
                "[CREATE] NEW QUIZ DATA", 
                "[ACCESS] EXISTING QUIZ", 
                "[DELETE] QUIZ DATA", 
                "[EXIT] TERMINAL" 
            };
            
            int selected = MenuSelector("=== LEARNIFY TERMINAL - MAIN MENU ===", menuOptions);

            switch (selected)
            {
                case 0: CreateQuiz(); break;
                case 1: SelectAndTakeQuiz(); break;
                case 2: DeleteQuiz(); break;
                case 3: running = false; break;
            }
        }
        
        Console.Clear();
        Typewriter(">> SHUTTING DOWN...");
        Thread.Sleep(1000);
    }

    // --- NAVIGATION & VISUALS ---

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
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else
                {
                    Console.WriteLine($"  {options[i]} ");
                }
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("(Use UP/DOWN Arrows & Press ENTER)");

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                currentIndex = (currentIndex == 0) ? options.Length - 1 : currentIndex - 1;
            }
            else if (key == ConsoleKey.DownArrow)
            {
                currentIndex = (currentIndex == options.Length - 1) ? 0 : currentIndex + 1;
            }
            else if (key == ConsoleKey.Enter)
            {
                return currentIndex;
            }
        }
    }

    static void Typewriter(string text)
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(10); 
        }
        Console.WriteLine();
    }

    // --- QUIZ CREATION  ---

    static void CreateQuiz()
{
    Console.Clear();
    Console.CursorVisible = true;
    
    Typewriter(">> ENTER NAME FOR NEW QUIZ:");
    string? quizName = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(quizName))
    {
         Typewriter(">> ERROR: INVALID NAME.");
         Thread.Sleep(1000);
         return;
    }

    if (!allQuizzes.ContainsKey(quizName)) 
        allQuizzes.Add(quizName, new List<Question>());
    else 
    {
        Typewriter(">> NOTICE: APPENDING TO EXISTING QUIZ.");
        Thread.Sleep(1000);
    }

    string[] modes = { "Easy (True/False)", "Medium (Multiple Choice)", "Hard (Direct Input)" };
    int typeChoice = MenuSelector($"CONFIGURING: {quizName}", modes);

    int count = 0;
    while(true) 
    {
        Console.Clear();
        Console.WriteLine($">> QUIZ: {quizName}");
        Console.Write(">> HOW MANY QUESTIONS TO ADD?: ");
        if (int.TryParse(Console.ReadLine(), out count) && count > 0) break;
        
        Console.WriteLine(">> INVALID NUMBER. TRY AGAIN.");
        Thread.Sleep(1000);
    }

    for (int i = 0; i < count; i++)
    {
        Question q = new Question();
        Console.WriteLine($"\n-- QUESTION ENTRY {i + 1} / {count} --");
        Console.Write("INPUT TEXT: ");
        q.Text = Console.ReadLine() ?? "Placeholder Question";

        if (typeChoice == 0) // Easy
        {
            q.Difficulty = "Easy";
            q.Choices = new string[] { "True", "False" };
            
            // VALIDATION 1
            string input = "";
            do {
                Console.Write("CORRECT ANSWER (T/F): ");
                input = Console.ReadLine()?.ToUpper() ?? "";
                if (input != "T" && input != "F") 
                    Console.WriteLine(">> INVALID. Please enter 'T' or 'F'.");
            } while (input != "T" && input != "F");
            q.CorrectAnswer = input;
        
        }
        else if (typeChoice == 1) // Medium
        {
            q.Difficulty = "Medium";
            q.Choices = new string[4];
            for (int j = 0; j < 4; j++)
            {
                Console.Write($"CHOICE {(char)('A' + j)}: ");
                q.Choices[j] = Console.ReadLine() ?? "";
            }

            // VALIDATION 2
            string input = "";
            string[] valid = { "A", "B", "C", "D" };
            do {
                Console.Write("CORRECT KEY (A/B/C/D): ");
                input = Console.ReadLine()?.ToUpper() ?? "";
                if (!valid.Contains(input)) 
                    Console.WriteLine(">> INVALID. Please enter A, B, C, or D.");
            } while (!valid.Contains(input));
            q.CorrectAnswer = input;
        
        }
        else // Hard
        {
            q.Difficulty = "Hard";
            Console.Write("INPUT EXACT CORRECT ANSWER: ");
            q.CorrectAnswer = Console.ReadLine() ?? "";
        }
        
        allQuizzes[quizName].Add(q);
    }

    Console.CursorVisible = false;
    Typewriter("\n>> DATA COMPILED SUCCESSFULLY.");
    
    Console.WriteLine("[PRESS ANY KEY TO RETURN TO MENU]");
    Console.ReadKey(true);
}

    // --- QUIZ TAKING ---

    static void SelectAndTakeQuiz()
    {
        if (allQuizzes.Count == 0)
        {
            Console.Clear();
            Typewriter(">> ERROR: NO QUIZ DATA FOUND.");
            Console.WriteLine("\n[PRESS KEY TO RETURN]");
            Console.ReadKey();
            return;
        }

        string[] quizNames = allQuizzes.Keys.ToArray();
        List<string> options = quizNames.ToList();
        options.Add("[RETURN TO MENU]");

        int choice = MenuSelector("SELECT A QUIZ TO BEGIN:", options.ToArray());

        if (choice == options.Count - 1) return;

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
                Console.WriteLine($">> {name} | HARD <<\n-------------------------------------");
                Console.WriteLine(q.Text);
                Console.WriteLine("-------------------------------------");
                
                Console.Write("TYPE YOUR ANSWER: ");
                userAnswer = Console.ReadLine() ?? "";
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
        Console.WriteLine("[PRESS ANY KEY TO RETURN TO MENU]");
        Console.ReadKey();
    }

    // --- QUIZ DELETION ---

    static void DeleteQuiz()
    {
        if (allQuizzes.Count == 0)
        {
            Console.Clear();
            Typewriter(">> NO QUIZZES AVAILABLE TO DELETE.");
            Typewriter(">> RETURNING TO MENU...");
            Thread.Sleep(1500);
            return;
        }

        List<string> options = allQuizzes.Keys.ToList();
        options.Add("[RETURN TO MENU]");

        int selectedIndex = MenuSelector(">> SELECT QUIZ TO DELETE PERMANENTLY:", options.ToArray());

        if (selectedIndex == options.Count - 1) return;

        string quizToDelete = options[selectedIndex];
        string[] confirmOptions = { "NO, CANCEL", "YES, DELETE DATA" };
        int confirm = MenuSelector($"WARNING: DELETE \"{quizToDelete}\"?", confirmOptions);

        if (confirm == 1) 
        {
            allQuizzes.Remove(quizToDelete);
            Console.Clear();
            Typewriter($">> QUIZ \"{quizToDelete}\" ERASED.");
            Console.WriteLine("\n[PRESS KEY TO RETURN]");
            Console.ReadKey(true);
        }
        else
        {
            Console.Clear();
            Typewriter(">> DELETION ABORTED.");
            Thread.Sleep(800);
        }
    }
}
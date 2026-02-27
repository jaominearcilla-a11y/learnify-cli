using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;
using System.Text.Json;

namespace Learnify_prtp;

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
    const string FilePath = "quizzes.json";

    static void Save()
    {
        string json = JsonSerializer.Serialize(allQuizzes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    static void Load()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            var data = JsonSerializer.Deserialize<Dictionary<string, List<Question>>>(json);
            if (data != null) allQuizzes = data;
        }
    }

    static void Intro()
    {
        Console.Title = "LEARNIFY TERMINAL";
        Console.ForegroundColor = ConsoleColor.Green;
        Console.BackgroundColor = ConsoleColor.Black;
        Console.CursorVisible = false;
        Console.Clear();

        Typewriter(">> LEARNIFY CORP (TM) TERMLINK 2026");
        Typewriter(">> INITIALIZING LEARNIFY PROTOCOLS...");
        Thread.Sleep(500);
    }

    static void Typewriter(string text)
    {
        foreach (char c in text) { Console.Write(c); Thread.Sleep(10); }
        Console.WriteLine();
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {options[i]} ");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.BackgroundColor = ConsoleColor.Black;
                }
                else { Console.WriteLine($"  {options[i]} "); }
            }
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("(Use UP/DOWN Arrows & Press ENTER)"); 
            Console.WriteLine("-------------------------------------");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow) currentIndex = (currentIndex == 0) ? options.Length - 1 : currentIndex - 1;
            else if (key == ConsoleKey.DownArrow) currentIndex = (currentIndex == options.Length - 1) ? 0 : currentIndex + 1;
            else if (key == ConsoleKey.Enter) return currentIndex;
        }
    }

    static void Create()
    {
        Console.Clear();
        Console.CursorVisible = true;
        Typewriter(">> ENTER NAME FOR NEW QUIZ:");
        string? quizName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(quizName)) {
             Typewriter(">> ERROR: INVALID NAME.");
             Thread.Sleep(1000);
             return;
        }

        if (!allQuizzes.ContainsKey(quizName)) allQuizzes.Add(quizName, new List<Question>());
        else { Typewriter(">> NOTICE: APPENDING TO EXISTING QUIZ."); Thread.Sleep(1000); }

        string[] modes = { "Easy (TRUE/FALSE)", "Medium (MULTIPLE CHOICE)", "Hard (IDENTIFICATION)" };
        int typeChoice = MenuSelector($"CONFIGURING: {quizName}", modes);

        int count = 0;
        while(true) {
            Console.Clear();
            Console.CursorVisible = true;
            Console.WriteLine($">> QUIZ: {quizName}");
            Console.Write(">> HOW MANY QUESTIONS TO ADD?: ");
            if (int.TryParse(Console.ReadLine(), out count) && count > 0) break;
            Console.WriteLine(">> INVALID NUMBER. TRY AGAIN.");
            Thread.Sleep(1000);
        }

        for (int i = 0; i < count; i++) {
            Console.Clear();
            Console.CursorVisible = true;
            
            Question q = new Question();
            Console.WriteLine($"\n-- QUESTION ENTRY {i + 1} / {count} --");
            Console.Write("INPUT TEXT: ");
            q.Text = Console.ReadLine() ?? "Placeholder Question";

            if (typeChoice == 0) { //  EASY VALID
                q.Difficulty = "Easy";
                q.Choices = new string[] { "True", "False" };
                
                int correctIndex = MenuSelector($">> SET CORRECT ANSWER FOR:\n{q.Text}", q.Choices);
                q.CorrectAnswer = correctIndex == 0 ? "T" : "F";
            }
            else if (typeChoice == 1) { // MEDIUM VALID
                q.Difficulty = "Medium";
                q.Choices = new string[4];
                for (int j = 0; j < 4; j++) {
                    Console.Write($"CHOICE {(char)('A' + j)}: ");
                    q.Choices[j] = Console.ReadLine() ?? "";
                }
                
                int correctIndex = MenuSelector($">> SET CORRECT ANSWER FOR:\n{q.Text}", q.Choices);
                q.CorrectAnswer = ((char)('A' + correctIndex)).ToString();
            }
            else { // HARD VALID
                q.Difficulty = "Hard";
                Console.Write("INPUT EXACT CORRECT ANSWER: ");
                q.CorrectAnswer = Console.ReadLine() ?? "";
            }
            allQuizzes[quizName].Add(q);
        }
        Save();
        Console.Clear();
        Console.CursorVisible = false;
        Typewriter("\n>> DATA COMPILED SUCCESSFULLY.");
        Console.WriteLine("[PRESS ANY KEY TO RETURN TO MENU]");
        Console.ReadKey(true);
    }

    static void Access()
    {
        if (allQuizzes.Count == 0) {
            Console.Clear();
            Typewriter(">> ERROR: NO QUIZ DATA FOUND.");
            Console.WriteLine("\n[PRESS KEY TO RETURN]");
            Console.ReadKey(true);
            return;
        }
        string[] quizNames = allQuizzes.Keys.ToArray();
        List<string> options = quizNames.ToList();
        options.Add("[RETURN TO MENU]");
        int choice = MenuSelector("SELECT A QUIZ TO BEGIN:", options.ToArray());
        if (choice != options.Count - 1) Run(quizNames[choice]);
    }

    static void Run(string name)
    {
        List<Question> questions = allQuizzes[name];
        int score = 0;
        foreach (var q in questions) {
            string userAnswer = "";
            if (q.Difficulty == "Easy" || q.Difficulty == "Medium") {
                int choiceIndex = MenuSelector($">> {name} | {q.Difficulty.ToUpper()} <<\n\n{q.Text}", q.Choices);
                userAnswer = (q.Difficulty == "Easy") ? (choiceIndex == 0 ? "T" : "F") : ((char)('A' + choiceIndex)).ToString();
            }
            else {
                Console.Clear(); Console.CursorVisible = true;
                Console.WriteLine($">> {name} | HARD <<\n-------------------------------------\n{q.Text}\n-------------------------------------");
                Console.Write("TYPE YOUR ANSWER: ");
                userAnswer = Console.ReadLine() ?? "";
                Console.CursorVisible = false;
            }
            if (userAnswer.Trim().Equals(q.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase)) {
                Console.WriteLine("\n>> CORRECT.");
                score++;
            }
            else { Console.WriteLine($"\n>> INCORRECT. ANSWER: {q.CorrectAnswer}"); }
            Console.WriteLine("PRESS ANY KEY TO CONTINUE.");
            Console.ReadKey(true);
        }
        Console.Clear();
        Typewriter("--- FINAL EVALUATION ---");
        Typewriter($"RESULT: {score} OUT OF {questions.Count} CORRECT.");
        Console.WriteLine("[PRESS ANY KEY TO RETURN TO MENU]");
        Console.ReadKey(true);
    }

    static void Delete()
    {
        if (allQuizzes.Count == 0) {
            Console.Clear();
            Typewriter(">> NO QUIZZES AVAILABLE TO DELETE.");
            Thread.Sleep(1500);
            return;
        }
        List<string> options = allQuizzes.Keys.ToList();
        options.Add("[RETURN TO MENU]");
        int selectedIndex = MenuSelector(">> SELECT QUIZ TO DELETE:", options.ToArray());
        if (selectedIndex == options.Count - 1) return;

        string quizToDelete = options[selectedIndex];
        int confirm = MenuSelector($"WARNING: DELETE \"{quizToDelete}\"?", new[] { "NO, CANCEL", "YES, DELETE DATA" });
        if (confirm == 1) {
            allQuizzes.Remove(quizToDelete);
            Save();
            Console.Clear();
            Typewriter($">> QUIZ \"{quizToDelete}\" ERASED.");
            Console.WriteLine("[PRESS ANY KEY TO RETURN TO MENU]");
            Console.ReadKey(true);
        }
    }

    static void Main()
    {
        Load();
        Intro();
        bool running = true;
        while (running)
        {
            while (Console.KeyAvailable) Console.ReadKey(true);
            int selected = MenuSelector("=== LEARNIFY TERMINAL - MAIN MENU ===", 
                new[] { "[CREATE] NEW QUIZ", "[ACCESS] QUIZ", "[DELETE] QUIZ", "[EXIT] TERMINAL" });

            switch (selected)
            {
                case 0: Create(); break;
                case 1: Access(); break;
                case 2: Delete(); break;
                case 3: running = false; break;
            }
        }
        Console.Clear();
        Typewriter(">> SHUTTING DOWN...");
        Thread.Sleep(1000);
    }
}

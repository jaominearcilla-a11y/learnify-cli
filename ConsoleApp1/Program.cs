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

    const ConsoleColor TextColor = ConsoleColor.Green;
    const ConsoleColor BorderColor = ConsoleColor.DarkGray;
    const ConsoleColor HeaderColor = ConsoleColor.Cyan;
    const ConsoleColor HighlightBg = ConsoleColor.Yellow;
    const ConsoleColor HighlightFg = ConsoleColor.Black;
    const ConsoleColor ErrorColor = ConsoleColor.Red;
    static void Save()
    {
        string json = JsonSerializer.Serialize(allQuizzes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }
    static void PressEnterIntro()
{
    Console.Clear();
    Console.CursorVisible = false;


    while (!Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Enter)
    {
        Console.SetCursorPosition(0, 0);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("[ ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("PRESS ENTER TO START SYSTEM");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(" ]");
        
        Thread.Sleep(500);
        
        
        Console.SetCursorPosition(0, 0);
        Console.Write(new string(' ', 35));
        
        Thread.Sleep(500);
    }

    Console.SetCursorPosition(0, 0);
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("[ SYSTEM ONLINE ]");
    Thread.Sleep(900);
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

    

    static void Typewriter(string text, int delay = 10)
    {
        foreach (char c in text) { Console.Write(c); Thread.Sleep(delay); }
        Console.WriteLine();
    }

    static void DrawHeader(string title)
    {
        Console.Clear();
        Console.ForegroundColor = BorderColor;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.Write("║ ");
        Console.ForegroundColor = HeaderColor;
        Console.Write(title.PadRight(62).Substring(0, 62));
        Console.ForegroundColor = BorderColor;
        Console.WriteLine(" ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.ForegroundColor = TextColor;
        Console.WriteLine();
    }

    static int MenuSelector(string title, string[] options, string subtitle = "(Use UP/DOWN Arrows & Press ENTER)")
    {
        int currentIndex = 0;
        while (true)
        {
            DrawHeader(title);
            Console.WriteLine($"  {subtitle}\n");

            for (int i = 0; i < options.Length; i++)
            {
                if (i == currentIndex)
                {
                    Console.Write("  ");
                    Console.BackgroundColor = HighlightBg;
                    Console.ForegroundColor = HighlightFg;
                    Console.WriteLine($" ► {options[i].PadRight(40)} ");
                    Console.ResetColor();
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = TextColor;
                }
                else
                {
                    Console.ForegroundColor = BorderColor;
                    Console.Write("  ");
                    Console.ForegroundColor = TextColor;
                    Console.WriteLine($"   {options[i]} ");
                }
            }
            
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow) currentIndex = (currentIndex == 0) ? options.Length - 1 : currentIndex - 1;
            else if (key == ConsoleKey.DownArrow) currentIndex = (currentIndex == options.Length - 1) ? 0 : currentIndex + 1;
            else if (key == ConsoleKey.Enter) return currentIndex;
        }
    }

    static void PrintError(string message)
    {
        Console.ForegroundColor = ErrorColor;
        Console.WriteLine($"\n[!] {message}");
        Console.ForegroundColor = TextColor;
        Thread.Sleep(1200);
    }

    static void Create()
    {
        DrawHeader("SYSTEM // CREATE NEW QUIZ");
        Console.CursorVisible = true;
        Console.Write(">> ENTER NAME FOR NEW QUIZ: ");
        string? quizName = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(quizName)) {
             PrintError("INVALID NAME. ABORTING.");
             return;
        }

        if (!allQuizzes.ContainsKey(quizName)) allQuizzes.Add(quizName, new List<Question>());
        else { 
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n>> NOTICE: APPENDING TO EXISTING QUIZ."); 
            Console.ForegroundColor = TextColor;
            Thread.Sleep(1000); 
        }

        string[] modes = { "Easy   (TRUE/FALSE)", "Medium (MULTIPLE CHOICE)", "Hard   (IDENTIFICATION)" };
        int typeChoice = MenuSelector($"CONFIGURING QUIZ: {quizName.ToUpper()}", modes, "SELECT QUESTION DIFFICULTY:");

        int count = 0;
        while(true) {
            DrawHeader($"QUIZ: {quizName.ToUpper()} // SETUP");
            Console.CursorVisible = true;
            Console.Write(">> HOW MANY QUESTIONS TO ADD?: ");
            if (int.TryParse(Console.ReadLine(), out count) && count > 0) break;
            PrintError("INVALID NUMBER. PLEASE ENTER A NUMBER GREATER THAN 0.");
        }

        for (int i = 0; i < count; i++) {
            DrawHeader($"QUIZ: {quizName.ToUpper()} // QUESTION ENTRY {i + 1} OF {count}");
            Console.CursorVisible = true;
            
            Question q = new Question();
            
            while (true) {
                Console.Write(">> INPUT QUESTION TEXT:\n   > ");
                q.Text = Console.ReadLine() ?? "";
                if (!string.IsNullOrWhiteSpace(q.Text)) break;
                PrintError("QUESTION CANNOT BE BLANK.");
                Console.WriteLine();
            }

            if (typeChoice == 0) { // EASY
                q.Difficulty = "Easy";
                q.Choices = new string[] { "True", "False" };
                
                int correctIndex = MenuSelector($"QUESTION {i + 1}:\n  {q.Text}", q.Choices, "SET CORRECT ANSWER:");
                q.CorrectAnswer = correctIndex == 0 ? "T" : "F";
            }
            else if (typeChoice == 1) { // MEDIUM
                q.Difficulty = "Medium";
                q.Choices = new string[4];
                Console.WriteLine("\n>> ENTER MULTIPLE CHOICE OPTIONS:");
                for (int j = 0; j < 4; j++) {
                    while (true) {
                        Console.Write($"   [{(char)('A' + j)}] ");
                        q.Choices[j] = Console.ReadLine() ?? "";
                        if (!string.IsNullOrWhiteSpace(q.Choices[j])) break;
                        PrintError("CHOICE CANNOT BE BLANK.");
                        Console.Write("   ");
                    }
                }
                
                int correctIndex = MenuSelector($"QUESTION {i + 1}:\n  {q.Text}", q.Choices, "SET CORRECT ANSWER:");
                q.CorrectAnswer = ((char)('A' + correctIndex)).ToString();
            }
            else { // HARD
                q.Difficulty = "Hard";
                Console.WriteLine();
                while (true) {
                    Console.Write(">> INPUT EXACT CORRECT ANSWER:\n   > ");
                    q.CorrectAnswer = Console.ReadLine() ?? "";
                    if (!string.IsNullOrWhiteSpace(q.CorrectAnswer)) break;
                    PrintError("ANSWER CANNOT BE BLANK.");
                }
            }
            allQuizzes[quizName].Add(q);
        }
        Save();
        Console.CursorVisible = false;
        DrawHeader("SYSTEM // SUCCESS");
        Console.WriteLine(">> DATA COMPILED AND SAVED SUCCESSFULLY.");
        Console.WriteLine("\n[PRESS ANY KEY TO RETURN TO MENU]");
        Console.ReadKey(true);
    }

    static void Access()
    {
        if (allQuizzes.Count == 0) {
            DrawHeader("SYSTEM // ERROR");
            PrintError("NO QUIZ DATA FOUND IN DATABANKS.");
            Console.WriteLine("\n[PRESS ANY KEY TO RETURN]");
            Console.ReadKey(true);
            return;
        }
        string[] quizNames = allQuizzes.Keys.ToArray();
        List<string> options = quizNames.ToList();
        options.Add("[ RETURN TO MAIN MENU ]");
        
        int choice = MenuSelector("SYSTEM // SELECT DATABANK", options.ToArray(), "AVAILABLE QUIZZES:");
        if (choice != options.Count - 1) Run(quizNames[choice]);
    }

    static void Run(string name)
    {
        List<Question> questions = allQuizzes[name];
        int score = 0;
        int qNum = 1;

        foreach (var q in questions) {
            string userAnswer = "";
            
            if (q.Difficulty == "Easy" || q.Difficulty == "Medium") {
                string title = $"ACTIVE QUIZ: {name.ToUpper()} // {q.Difficulty.ToUpper()}";
                string subtitle = $"QUESTION {qNum} OF {questions.Count}:\n\n  {q.Text}\n";
                int choiceIndex = MenuSelector(title, q.Choices, subtitle);
                userAnswer = (q.Difficulty == "Easy") ? (choiceIndex == 0 ? "T" : "F") : ((char)('A' + choiceIndex)).ToString();
                
                DrawHeader($"ACTIVE QUIZ: {name.ToUpper()} // EVALUATION");
                Console.WriteLine($"QUESTION:\n  {q.Text}\n");
            }
            else {
                DrawHeader($"ACTIVE QUIZ: {name.ToUpper()} // HARD");
                Console.CursorVisible = true;
                Console.WriteLine($"QUESTION {qNum} OF {questions.Count}:\n");
                Console.WriteLine($"  {q.Text}\n");
                Console.ForegroundColor = BorderColor;
                Console.WriteLine("─────────────────────────────────────────────────────────────────");
                Console.ForegroundColor = TextColor;
                Console.Write(">> TYPE YOUR ANSWER: ");
                userAnswer = Console.ReadLine() ?? "";
                Console.CursorVisible = false;
            }
            
            Console.ForegroundColor = BorderColor;
            Console.WriteLine("─────────────────────────────────────────────────────────────────");
            Console.ForegroundColor = TextColor;

            if (userAnswer.Trim().Equals(q.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase)) {
                Console.ForegroundColor = HeaderColor;
                Console.WriteLine("\n  [+] CORRECT.");
                Console.ForegroundColor = TextColor;
                score++;
            }
            else { 
                Console.ForegroundColor = ErrorColor;
                Console.Write("\n  [-] INCORRECT.");
                Console.ForegroundColor = TextColor;
                Console.Write(" CORRECT ANSWER: ");
                
                Console.ForegroundColor = ErrorColor;
                Console.WriteLine(q.CorrectAnswer);
                Console.ForegroundColor = TextColor; 
            }
            
            Console.WriteLine("\n>> PRESS ANY KEY TO CONTINUE.");
            Console.ReadKey(true);
            qNum++;
        }
        
        DrawHeader("SYSTEM // FINAL EVALUATION");
        Console.ForegroundColor = HeaderColor;
        Console.WriteLine($"  FINAL SCORE: {score} OUT OF {questions.Count} CORRECT.");
        
        // Calculate percentage for feedback
        double percentage = (double)score / questions.Count;
        if (percentage == 1.0) Console.WriteLine("  RATING: FLAWLESS.");
        else if (percentage >= 0.7) Console.WriteLine("  RATING: ACCEPTABLE.");
        else Console.WriteLine("  RATING: SUBOPTIMAL. RETRAINING ADVISED.");
        
        Console.ForegroundColor = TextColor;
        Console.WriteLine("\n[PRESS ANY KEY TO RETURN TO MENU]");
        Console.ReadKey(true);
    }

    static void Delete()
    {
        if (allQuizzes.Count == 0) {
            DrawHeader("SYSTEM // DELETE MODULE");
            PrintError("NO QUIZZES AVAILABLE TO DELETE.");
            return;
        }
        List<string> options = allQuizzes.Keys.ToList();
        options.Add("[ CANCEL ]");
        
        int selectedIndex = MenuSelector("SYSTEM // DELETE MODULE", options.ToArray(), "SELECT QUIZ TO PURGE:");
        if (selectedIndex == options.Count - 1) return;

        string quizToDelete = options[selectedIndex];
        int confirm = MenuSelector($"WARNING: PURGE DATABANK \"{quizToDelete}\"?", new[] { "NO, CANCEL", "YES, DELETE DATA" }, "CONFIRM DELETION:");
        
        if (confirm == 1) {
            allQuizzes.Remove(quizToDelete);
            Save();
            DrawHeader("SYSTEM // DELETE MODULE");
            Console.ForegroundColor = ErrorColor;
            Console.WriteLine($"\n>> DATABANK \"{quizToDelete}\" ERASED.");
            Console.ForegroundColor = TextColor;
            Console.WriteLine("\n[PRESS ANY KEY TO RETURN TO MENU]");
            Console.ReadKey(true);
        }
    }

    static void Main()
    {
        Load();
        PressEnterIntro();
        bool running = true;
        while (running)
        {
            while (Console.KeyAvailable) Console.ReadKey(true);
            int selected = MenuSelector("LEARNIFY TERMINAL OS // MAIN MENU", 
                new[] { 
                    "CREATE NEW QUIZ", 
                    "ACCESS DATABANK (TAKE QUIZ)", 
                    "PURGE DATABANK (DELETE QUIZ)", 
                    "EXIT TERMINAL" 
                }, 
                "AWAITING COMMAND:USE UP/DOWN ARROWS & PRESS ENTER");

            switch (selected)
            {
                case 0: Create(); break;
                case 1: Access(); break;
                case 2: Delete(); break;
                case 3: running = false; break;
            }
        }
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Typewriter(">> TERMINATING SESSION...", 20);
        Typewriter(">> GOODBYE.", 30);
        Thread.Sleep(800);
        Console.ResetColor();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    // Spelkonfiguration
    static int width = 40; // Bredden på spelplanen
    static int height = 20; // Höjden på spelplanen
    static int score = 0; // Poängräkning
    static int level = 1; // Nivå
    static int speed = 300; // Hastighet (lägre värde är snabbare)
    static bool gameOver = false; // Flagga för att kontrollera om spelet är över
    static bool gamePaused = false; // Flagga för att kontrollera om spelet är pausat

    // Ormens data
    static List<(int x, int y)> snake = new List<(int x, int y)>(); // Ormens kropp
    static (int x, int y) direction = (0, 1); // Ormens riktning 
    static (int x, int y) food; // Matens position

    // Mappning av piltangenter till riktningar
    private static readonly Dictionary<ConsoleKey, (int x, int y)> directionMap = new()
    {
        { ConsoleKey.LeftArrow, (-1, 0) },
        { ConsoleKey.RightArrow, (1, 0) },
        { ConsoleKey.UpArrow, (0, -1) },
        { ConsoleKey.DownArrow, (0, 1) }
    };

    static void Main()
    {
        InitGame();

        // Grundläggande inställningar för konsol
        Console.CursorVisible = false;
        Console.Clear();
        Console.SetWindowSize(Math.Max(Console.WindowWidth, width + 2), Math.Max(Console.WindowHeight, height + 4));

        // Visa startskärmen
        StartScreen();

        // Huvudspel-loop
        while (!gameOver)
        {
            if (!gamePaused)
            {
                Draw(); // Rita spelplanen
                Input(); // Hantera användarinput
                Logic(); // Spellogik
                Thread.Sleep(speed); // Kontrollera spelets hastighet
            }
            else
            {
                PauseScreen(); // Visa paus-skärmen
                Input(); // Hantera användarinput under paus
            }
        }

        // Visa Game Over-skärmen när spelet är slut
        GameOverScreen();
    }

    static void InitGame()
    {
        score = 0;
        level = 1;
        speed = 200;
        gameOver = false;
        gamePaused = false;
        snake = new List<(int x, int y)> { (10, 10), (10, 9), (10, 8) };
        direction = (0, 1);
        var rnd = new Random();
        food = (rnd.Next(0, width), rnd.Next(0, height));
    }

    static void StartScreen()
    {
        // Rensa konsolen och visa startmeddelandet
        Console.Clear();
        Console.SetCursorPosition(width / 2 - 5, height / 2 - 2);
        Console.Write("Snake Game");
        Console.SetCursorPosition(width / 2 - 12, height / 2);
        Console.Write("Press ENTER to Start");
        Console.SetCursorPosition(width / 2 - 12, height / 2 + 1);
        Console.Write("Press P to Pause/Resume");
        Console.SetCursorPosition(width / 2 - 12, height / 2 + 2);
        Console.Write("Press ESC to Quit");

        // Vänta på att användaren trycker på Enter för att starta spelet
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
    }

    static void Draw()
    {
        // Rensa konsolen och rita spelplanen med ormen och maten
        Console.Clear();
        for (int i = 0; i < width + 2; i++) Console.Write("#");
        Console.WriteLine();
        for (int i = 0; i < height; i++)
        {
            Console.Write("#");
            for (int j = 0; j < width; j++)
            {
                if (snake.Contains((j, i)))
                    Console.Write("O"); // Rita ormen
                else if (food == (j, i))
                {
                    Console.ForegroundColor = ConsoleColor.Red; // Sätt färgen till röd
                    Console.Write("X"); // Rita maten
                    Console.ResetColor(); // Återställ färgen
                }
                else
                    Console.Write(" ");
            }
            Console.WriteLine("#");
        }
        for (int i = 0; i < width + 2; i++) Console.Write("#");
        Console.WriteLine();
        Console.WriteLine($"Score: {score}  Level: {level}");
    }

    static void Input()
    {
        // Hantera användarinput för att styra ormen, pausa spelet eller avsluta spelet
        if (!Console.KeyAvailable) return;
        var key = Console.ReadKey(true).Key;

        if (directionMap.ContainsKey(key) && directionMap[key] != (-direction.x, -direction.y))
        {
            direction = directionMap[key];
        }

        switch (key)
        {
            case ConsoleKey.P:
                gamePaused = !gamePaused; // Pausa/återuppta spelet
                break;
            case ConsoleKey.Escape:
                gameOver = true; // Avsluta spelet
                break;
        }
    }

    static void Logic()
    {
        // Uppdatera ormens position och hantera spelreglerna
        var head = (x: snake[0].x + direction.x, y: snake[0].y + direction.y);

        // Kontrollera om ormen krockar med väggarna eller sig själv
        if (head.x < 0 || head.x >= width || head.y < 0 || head.y >= height || snake.Contains(head))
        {
            gameOver = true;
            return;
        }

        snake.Insert(0, head); // Lägg till nytt huvud på ormen

        if (head == food)
        {
            score += 10; // Öka poängen
            var rnd = new Random();
            food = (rnd.Next(0, width), rnd.Next(0, height)); // Generera ny mat

            // Öka nivån och snabba upp spelet var 50:e poäng
            if (score % 50 == 0)
            {
                level++;
                speed = Math.Max(50, speed - 10); // Minska sleep-tiden för att öka hastigheten långsammare
            }
        }
        else
        {
            snake.RemoveAt(snake.Count - 1); // Ta bort sista segmentet om inte mat äts
        }
    }

    static void PauseScreen()
    {
        // Visa pausmeddelandet
        Console.SetCursorPosition(width / 2 - 5, height / 2);
        Console.Write("Paused");
    }

    static void GameOverScreen()
    {
        // Visa Game Over-skärmen och möjligheten att starta om
        Console.Clear();
        Console.SetCursorPosition(width / 2 - 5, height / 2);
        Console.Write("Game Over!");
        Console.SetCursorPosition(width / 2 - 5, height / 2 + 1);
        Console.Write($"Score: {score}");
        Console.SetCursorPosition(width / 2 - 5, height / 2 + 2);
        Console.Write($"Level: {level}");
        Console.SetCursorPosition(width / 2 - 12, height / 2 + 4);
        Console.Write("Press ENTER to Restart");

        // Vänta på att användaren trycker på Enter för att starta om spelet
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }

        // Återställ spelets tillstånd
        InitGame();

        // Starta om spelet
        Main();
    }
}



using System;
using System.Threading;

namespace PayrollManagementSystem
{
    public class Application
    {
        private int consoleWidth, consoleHeight;
        private int index = 0;


        public string title = "Payroll Mangement System";
        public string[] commands = { "(1) New Payroll", "(2) View Payroll", "(3) Print Payroll" };

        public Application(int width, int height)
        {
            consoleWidth = width;
            consoleHeight = height;

            InitialiseWindow();
        }

        // Initialise the console window
        public void InitialiseWindow()
        {
            // Set console and buffer size
            Console.SetWindowSize(consoleWidth, consoleHeight);
            Console.SetBufferSize(consoleWidth, consoleHeight);
        }

        // Print strings onto the console
        public void Print(object message, int position = 0)
        {
            // Set cursor position and print message
            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2 + position, Console.CursorTop);
            Console.Write( message);
        }

        // Print title
        public void PrintTitle()
        {
            // Print top title border
            for (int i = 0; i < title.Length; ++i)
                Print((i < title.Length - 1) ? "*" : "*\n", i);

            // Print appliation title
            Print(title + "\n");

            // Print bottom title border
            for (int i = 0; i < title.Length; ++i)
                Print((i < title.Length - 1) ? "*" : "*\n", i);
        }

        public string GetCommand()
        {
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.UpArrow:
                    if (index == commands.Length - 1)
                        return commands[index = 0];

                    return commands[++index];

                case ConsoleKey.DownArrow:
                    if (index == 0)
                        return commands[index = commands.Length - 1];

                    return commands[--index];

                default:
                    return commands[index];
            }
        }
    }

    public enum PositionHours
    {
        CHEF = 56,
        HEADCHEF = 42,
        MANAGER = 56,
        SUPERVISOR = 56,
        CATERING = 49,
        WAITERS = 35,
        CLEANERS = 14
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Initialise application
            Application application = new Application(50, 20);

            string currentCommand = application.commands[0];
            while (true)
            {
                Console.Clear();
                application.PrintTitle();
                application.Print("Command: ");
                Console.Write(currentCommand);

                // Get user command
                currentCommand = application.GetCommand();

                // TODO: If user enters then complete task
                Thread.Sleep(1);
            }
        }
    }
}

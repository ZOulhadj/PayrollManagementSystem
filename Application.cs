using System;
using System.Collections.Specialized;
using System.Threading;
using System.Collections.Generic;

namespace PayrollManagementSystem
{
    // Command types enum
    public enum CommandTypes
    {
        NEWPAYROLL,
        VIEWPAYROLL,
        NEWEMPLOYEE,
        REMOVEEMPLOYEE,
        HELP
    };

    // Application class
    public class Application
    {
        // Application member variables
        private string m_ApplicationTitle;
        private int m_ConsoleWidth, m_ConsoleHeight;
        private int m_CommandIndex = 0;
        private bool m_EnterPressed;

        // An ordered array of commands (Key, Value)
        private OrderedDictionary commands = new OrderedDictionary
        {
            { CommandTypes.NEWPAYROLL,      "(1) New Payroll" },
            { CommandTypes.VIEWPAYROLL,     "(2) View Payroll" },
            { CommandTypes.NEWEMPLOYEE,     "(3) New Employee" },
            { CommandTypes.REMOVEEMPLOYEE,  "(4) Remove Employee"},
            { CommandTypes.HELP,            "(5) Help"}
        };

        public Application(string title, int width, int height)
        {
            m_ApplicationTitle = title;
            m_ConsoleWidth = width;
            m_ConsoleHeight = height;

            InitialiseWindow(ConsoleColor.DarkBlue, false);
        }

        ~Application() { }

        // Initialise the console window
        public void InitialiseWindow(ConsoleColor backgroundColor, bool cursorVisible)
        {
            // Initialise console
            Console.Title = m_ApplicationTitle + " - ©Zakariya Oulhadj";
            Console.SetWindowSize(m_ConsoleWidth, m_ConsoleHeight);
            Console.SetBufferSize(m_ConsoleWidth, m_ConsoleHeight);
            Console.BackgroundColor = backgroundColor;
            Console.CursorVisible = cursorVisible;
        }

        // This is the main program loop
        public void Loop()
        {
            // Initialise example employee data
            List<Person> employees = new List<Person>();
            employees.Add(new Person
            {
                name = "Zakariya",
                age = 18,
                jobTitle = Enum.GetName(typeof(PositionHours), PositionHours.CHEF),
                apprentice = false,
                weeklyHours = (int)Enum.Parse(typeof(PositionHours), "CHEF")
            });

            // Display initial command on startup
            object currentCommand = commands[CommandTypes.NEWPAYROLL];
            
            // Main loop
            while (true)
            {
                // Clear the window when new text is printed
                Console.Clear();

                // Print title and accept command selection
                PrintTitle();
                Print("Command: " + currentCommand);
                currentCommand = CommandList();

                // Once the user hits the enter button, complete a certain task
                if (IsEnterPressed())
                {
                    // Create a new employee payroll
                    if (currentCommand == commands[CommandTypes.NEWPAYROLL])
                    {
                        Console.Clear();
                        PrintTitle();
                        Print("New Payroll\n\n");

                        // Display payslip date range (1 Month)
                        string payrollDateRange = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy") + " - " + DateTime.Today.ToString("dd/MM/yyyy");

                        for (int i = 0; i < employees.Count; ++i)
                        {
                            // Ensure that overtime input is valid
                            while (true)
                            {
                                Console.Clear();
                                PrintTitle();
                                Print("New Payroll\n\n");
                                Print(payrollDateRange + "\n\n");

                                employees[i].PrintEmployeeInformation(this);

                                Print("\n");
                                Print("Hours: " + employees[i].weeklyHours * 4 + "\n");
                                try
                                {
                                    employees[i].SetOvertimeHours(int.Parse(Input("Overtime: ")));

                                    break;
                                }
                                catch (Exception e)
                                {
                                    Print("\n");
                                    Print("Invalid input. Try again.", ConsoleColor.Red);
                                    Wait();
                                }

                            }
                            Print("\n");
                            Print("\n");

                            // Display 'menu' buttons
                            string choice = ButtonInput("Exit", "Next");

                            if (choice == "Exit")
                            {
                                Print("\n");
                                Print("Going Back To Main Menu", ConsoleColor.Yellow);
                                Wait();
                                break;
                            }
                            else if (choice == "Next")
                            {
                                Print("\n");

                                // Last means there are no more employees
                                if (i < employees.Count - 1)
                                {
                                    Print("Going To The Next Employee", ConsoleColor.Yellow);
                                    Wait();
                                    continue;
                                }
                                else
                                {
                                    Print("No More Employees... Exiting", ConsoleColor.Red);
                                    Wait();
                                    break;
                                }

                            }
                        }
                    }

                    // An overview of an employees payroll
                    if (currentCommand == commands[CommandTypes.VIEWPAYROLL])
                    {
                        Console.Clear();
                        PrintTitle();
                        Print("View Payroll\n\n");

                        // Display payslip date range (1 Month)
                        string payrollDateRange = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy") + " - " + DateTime.Today.ToString("dd/MM/yyyy");

                        for (int i = 0; i < employees.Count; ++i)
                        {
                            Console.Clear();
                            PrintTitle();
                            Print("View Payroll\n\n");
                            Print(payrollDateRange + "\n\n");

                            employees[i].PrintEmployeeInformation(this);
                            employees[i].CalculateEmployeeEarnings();
                            employees[i].PrintPayrollInformation(this);
                            Print("\n");

                            // Display 'menu' buttons
                            string choice = ButtonInput("Exit", "Next");

                            if (choice == "Exit")
                            {
                                Print("\n");
                                Print("Going Back To Main Menu", ConsoleColor.Yellow);
                                Wait();
                                break;
                            }
                            else if (choice == "Next")
                            {
                                Print("\n");

                                // Last means there are no more employees
                                if (i < employees.Count - 1)
                                {
                                    Print("Going To The Next Employee", ConsoleColor.Yellow);
                                    Wait();
                                    continue;
                                }
                                else
                                {
                                    Print("No More Employees... Exiting", ConsoleColor.Red);
                                    Wait();
                                    break;
                                }
                            }
                        }
                    }

                    // Add a new customer to the system
                    if (currentCommand == commands[CommandTypes.NEWEMPLOYEE])
                    {
                        Console.Clear();
                        PrintTitle();
                        Print("New Employee\n\n");

                        // Add new employee to the list
                        try
                        {
                            Person employee = new Person();
                            employee.name = Input("Name: ");
                            employee.age = uint.Parse(Input("Age: "));
                            employee.jobTitle = Input("Job Title: ");
                            employee.apprentice = bool.Parse(Input("Apprentice: "));
                            employee.weeklyHours = (int)Enum.Parse(typeof(PositionHours), employee.jobTitle);

                            employees.Add(employee);

                            Print("A new employee has been added!\n", ConsoleColor.Green);
                            Wait(3000);
                        }
                        catch (Exception e)
                        {
                            Print("\n");
                            Print("Error adding new employee!\n", ConsoleColor.Red);
                            Print(e.Message + "\n", ConsoleColor.Red);
                            Print("Exiting back to main menu...\n", ConsoleColor.Yellow);
                            Wait(4000);
                        }
                    }

                    // Remove an employee from the employees list
                    if (currentCommand == commands[CommandTypes.REMOVEEMPLOYEE])
                    {
                        Console.Clear();
                        PrintTitle();
                        Print("Remove Employee\n\n");

                        string name = Input("Name: ");
                        if (employees.Remove(employees.Find(employee => employee.name == name)))
                        {
                            Print("\n");
                            Print("Employee " + name + " has been removed for the list", ConsoleColor.Green);
                        }
                        else
                        {
                            Print("Could not find employee " + "'" + name + "'\n", ConsoleColor.Red);
                            Print("\n");
                        }

                        Wait();
                    }

                    // Open a help file
                    if (currentCommand == commands[CommandTypes.HELP])
                    {
                        OpenFile(@"Help.txt");
                    }
                }

                // Note: Stops the window from flashing in some instances
                Wait(100);
            }
        }

        // Standard application wait time
        public void Wait(int sleep = 2000)
        {
            Thread.Sleep(sleep);
        }

        // Print strings onto the console
        public void Print(object message,
                          ConsoleColor textColor = ConsoleColor.White,
                          ConsoleColor backgroundColor = ConsoleColor.DarkBlue,
                          int position = 0,
                          bool add = false)
        {
            // Set cursor position and print message
            if (add)
                Console.SetCursorPosition((Console.WindowWidth + m_ApplicationTitle.Length) / 2 + position, Console.CursorTop);
            else
                Console.SetCursorPosition((Console.WindowWidth - m_ApplicationTitle.Length) / 2 + position, Console.CursorTop);

            Console.ForegroundColor = textColor;
            Console.BackgroundColor = backgroundColor;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.DarkBlue;
        }

        // Prints message and accepts user input 
        public string Input(object message)
        {
            Print(message);
            return Console.ReadLine();
        }

        // Print title
        public void PrintTitle()
        {
            // Print top title border
            for (int i = 0; i < m_ApplicationTitle.Length; ++i)
                Print((i < m_ApplicationTitle.Length - 1) ? "*" : "*\n", ConsoleColor.White, ConsoleColor.DarkBlue, i);

            // Print application title
            Print(m_ApplicationTitle + "\n");

            // Print bottom title border
            for (int i = 0; i < m_ApplicationTitle.Length; ++i)
                Print((i < m_ApplicationTitle.Length - 1) ? "*" : "*\n", ConsoleColor.White, ConsoleColor.DarkBlue, i);
        }

        // Display buttons and accept user input
        public string ButtonInput(string first, string second)
        {
            // Display initial buttons
            Print(second, ConsoleColor.White, ConsoleColor.DarkBlue, 0, true);
            Print(first, ConsoleColor.Black, ConsoleColor.White);

            // Button selection
            string choice = "";
            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.LeftArrow:
                        Print(second, ConsoleColor.White, ConsoleColor.DarkBlue, 0, true);
                        Print(first, ConsoleColor.Black, ConsoleColor.White);

                        choice = first;
                        break;
                    case ConsoleKey.RightArrow:
                        Print(first, ConsoleColor.White, ConsoleColor.DarkBlue);
                        Print(second, ConsoleColor.Black, ConsoleColor.White, 0, true);

                        choice = second;
                        break;
                    case ConsoleKey.Enter:
                        return choice;
                    default:
                        break;
                }
            }
        }

        // Filter through commands
        public object CommandList()
        {
            m_EnterPressed = false;
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.UpArrow:
                    // Bounds checking
                    if (m_CommandIndex == commands.Count - 1)
                        return commands[m_CommandIndex = 0];

                    return commands[++m_CommandIndex];
                case ConsoleKey.DownArrow:
                    // Bounds checking
                    if (m_CommandIndex == 0)
                        return commands[m_CommandIndex = commands.Count - 1];

                    return commands[--m_CommandIndex];
                case ConsoleKey.Enter:
                    m_EnterPressed = true;
                    return commands[m_CommandIndex];
                default:
                    return commands[m_CommandIndex];
            }
        }

        // Check if the enter key has been pressed
        public bool IsEnterPressed()
        {
            return m_EnterPressed;
        }

        // Load and open a file
        public void OpenFile(string filePath)
        {
            // Load and display a help text file using notepad
            System.Diagnostics.Process.Start(@"notepad.exe", filePath);
        }
    }
}

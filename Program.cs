using System;
using System.Threading;
using System.Collections.Specialized;

namespace PayrollManagementSystem
{
    // Command types interface
    public enum CommandTypes
    {
        NEWPAYROLL, 
        VIEWPAYROLL, 
        PRINTPAYROLL, 
        NEWCUSTOMER 
    };

    public class Application
    {
        private int consoleWidth, consoleHeight;
        private int commandIndex = 0;
        public string title = "Payroll Mangement System";

        // An ordered array of commands
        public OrderedDictionary commands = new OrderedDictionary
        {
            { CommandTypes.NEWPAYROLL,      "(1) New Payroll" },
            { CommandTypes.VIEWPAYROLL,     "(2) View Payroll" },
            { CommandTypes.PRINTPAYROLL,    "(3) Print Payroll" },
            { CommandTypes.NEWCUSTOMER,     "(4) New Customer" }
        };

        public Application(int width, int height)
        {
            consoleWidth = width;
            consoleHeight = height;

            InitialiseWindow();
        }

        ~Application() {}

        // Initialise the console window
        public void InitialiseWindow()
        {
            // Set console and buffer size
            Console.SetWindowSize(consoleWidth, consoleHeight);
            //Console.SetBufferSize(consoleWidth, consoleHeight);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.CursorVisible = false;
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
                Console.SetCursorPosition((Console.WindowWidth + title.Length) / 2 + position, Console.CursorTop);
            else
                Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2 + position, Console.CursorTop);

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
            for (int i = 0; i < title.Length; ++i)
                Print((i < title.Length - 1) ? "*" : "*\n", ConsoleColor.White, ConsoleColor.DarkBlue, i);

            // Print appliation title
            Print(title + "\n");

            // Print bottom title border
            for (int i = 0; i < title.Length; ++i)
                Print((i < title.Length - 1) ? "*" : "*\n", ConsoleColor.White, ConsoleColor.DarkBlue, i);
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

        // Get string command based on the command type
        public string GetCommand(CommandTypes commandType)
        {
            return commands[commandType].ToString();
        }

        // Filter through commands
        public string CommandList()
        {
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.UpArrow:
                    if (commandIndex == commands.Count - 1)
                        return commands[commandIndex = 0].ToString();

                    return commands[++commandIndex].ToString();
                case ConsoleKey.DownArrow:
                    if (commandIndex == 0)
                        return commands[commandIndex = commands.Count - 1].ToString();

                    return commands[--commandIndex].ToString();
                default:
                    return commands[commandIndex].ToString();
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
        WAITER = 35,
        CLEANER = 14
    }

    public struct Employee
    {
        public uint id;
        public string name;
        public uint age;
        public string jobTitle;
        public uint nationalInsurance;
        public string taxCode;

        public bool apprentice;
        public double hourlyPay;
        public int weeklyHours;
        public int overtimeHours;

        public double normalPay, overtimePay, grossPay, totalDeductions, netPay;

        public void CalculateEmployeeEarnings()
        {
            // Calculate hourly pay
            if (age < 17)
                hourlyPay = 3.72;
            else if (age < 19 && apprentice == true)
                hourlyPay = 2.68;
            else if (age >= 19 && apprentice == true)
                hourlyPay = 2.68;
            else if (age <= 20)
                hourlyPay = 5.03;
            else if (age >= 21)
                hourlyPay = 6.31;

            normalPay = Math.Round(hourlyPay * (weeklyHours * 4), 2);
            overtimePay = Math.Round(hourlyPay * overtimeHours, 2);
            grossPay = Math.Round(normalPay + overtimePay, 2);
        }
    }


    class Program
    {

        static void Main(string[] args)
        {
            // Initialise application
            Application application = new Application(80, 30);

            // Initialise employee data
            Employee[] employees = new Employee[33];
            employees[0].id = 22;
            employees[0].name = "Zakariya";
            employees[0].age = 18;
            employees[0].jobTitle = Enum.GetName(typeof(PositionHours), PositionHours.HEADCHEF);
            employees[0].nationalInsurance = 4520;
            employees[0].taxCode = "9LUQ";
            employees[0].apprentice = true;
            // Calculate weekly hours based on the job title
            employees[0].weeklyHours = (int)Enum.Parse(typeof(PositionHours), employees[0].jobTitle);

            string currentCommand = application.GetCommand(CommandTypes.NEWPAYROLL);
            while (true)
            {
                Console.Clear();
                application.PrintTitle();

                application.Print("Command: ");
                Console.Write(currentCommand);

                // Get command using input (arrow keys)
                currentCommand = application.CommandList();

                // Depending on the command do certain actions
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    if (currentCommand == application.commands[CommandTypes.NEWPAYROLL].ToString())
                    {
                        Console.Clear();
                        application.PrintTitle();
                        application.Print("New Payroll\n\n");
                        
                        // Display payslip date range (1 Month)
                        string payrollDateRange = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy") + " - " + DateTime.Today.ToString("dd/MM/yyyy");
                        application.Print(payrollDateRange + "\n\n");

                        foreach (Employee employee in employees)
                        {
                            Console.Clear();
                            application.PrintTitle();
                            application.Print("New Payroll\n\n");

                            application.Print("Name: " + employee.name + "\n");
                            application.Print("Age: " + employee.age + "\n");
                            application.Print("Job Title: " + employee.jobTitle + "\n");
                            application.Print("NI Code: " + employee.nationalInsurance + "\n");
                            application.Print("Tax Code: " + employee.taxCode + "\n");

                            application.Print("\n");
                            application.Print("Hours: " + employee.weeklyHours * 4 + "\n");
                            //employee.overtimeHours = int.Parse(application.Input("Overtime: "));

                            application.Print("\n");
                            application.Print("\n");

                            // Display 'menu' buttons
                            string choice = application.ButtonInput("Exit", "Next");

                            if (choice == "Exit")
                            {

                                application.Print("\n");
                                application.Print("Going Back To Main Menu", ConsoleColor.Yellow);
                                Thread.Sleep(1500);
                                break;

                            }
                            else if (choice == "Next")
                            {
                                application.Print("\n");
                                application.Print("Going To Next Customer", ConsoleColor.Yellow);
                                Thread.Sleep(1500);
                                continue;
                            }
                        }
                        

                        // Switch to the next command for convenience
                        //currentCommand = application.commands[CommandTypes.VIEWPAYROLL].ToString();
                    }
                    else if (currentCommand == application.commands[CommandTypes.VIEWPAYROLL].ToString())
                    {
                        Console.Clear();
                        application.PrintTitle();
                        application.Print("Viwe Payroll\n\n");

                        string id2 = application.Input("Employee ID: ");
                        application.Print("\n");

                        // If an employee is found, print their information
                        if (id2 == employees[0].id.ToString())
                        {
                            application.Print("Employee Name: " + employees[0].name + "\n");
                            application.Print("Employee Age: " + employees[0].age + "\n");
                            application.Print("Employee Job Title: " + employees[0].jobTitle + "\n");
                            application.Print("Employee NI: " + employees[0].nationalInsurance + "\n");
                            application.Print("Employee Tax Code: " + employees[0].taxCode + "\n");
                        }
                        else
                        {
                            application.Print("Employee Not Found.", ConsoleColor.Red);
                            Console.ReadLine();

                            break;
                        }

                        employees[0].CalculateEmployeeEarnings();
                        application.Print("\n");
                        application.Print("Normal Pay: " + "£" + employees[0].normalPay.ToString() + "\n");
                        application.Print("Overtime Pay: " + "£" + employees[0].overtimePay.ToString() + "\n");
                        application.Input("Gross Pay: " + "£" + employees[0].grossPay.ToString() + "\n");
                    }
                }

                // TODO: If user enters then complete task
                Thread.Sleep(100);
            }

        }
    }
}

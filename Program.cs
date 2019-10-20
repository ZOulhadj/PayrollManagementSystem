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
        public string applicationTitle;
        private int consoleWidth, consoleHeight;
        private int commandIndex = 0;

        // An ordered array of commands (Key, Value)
        public OrderedDictionary commands = new OrderedDictionary
        {
            { CommandTypes.NEWPAYROLL,      "(1) New Payroll" },
            { CommandTypes.VIEWPAYROLL,     "(2) View Payroll" },
            { CommandTypes.PRINTPAYROLL,    "(3) Print Payroll" },
            { CommandTypes.NEWCUSTOMER,     "(4) New Customer" }
        };

        public Application(string title, int width, int height)
        {
            applicationTitle = title;
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
            Console.SetBufferSize(consoleWidth, consoleHeight);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.CursorVisible = false;
        }

        // Standard application wait time
        public void Wait()
        {
            Thread.Sleep(2000);
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
                Console.SetCursorPosition((Console.WindowWidth + applicationTitle.Length) / 2 + position, Console.CursorTop);
            else
                Console.SetCursorPosition((Console.WindowWidth - applicationTitle.Length) / 2 + position, Console.CursorTop);

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
            for (int i = 0; i < applicationTitle.Length; ++i)
                Print((i < applicationTitle.Length - 1) ? "*" : "*\n", ConsoleColor.White, ConsoleColor.DarkBlue, i);

            // Print appliation title
            Print(applicationTitle + "\n");

            // Print bottom title border
            for (int i = 0; i < applicationTitle.Length; ++i)
                Print((i < applicationTitle.Length - 1) ? "*" : "*\n", ConsoleColor.White, ConsoleColor.DarkBlue, i);
        }

        // Display buttons and accept user input
        public string ButtonInput(string first, string second)
        {
            // Display initial buttons
            //Print(second, ConsoleColor.White, ConsoleColor.DarkBlue, 0, true);
            //Print(first, ConsoleColor.Black, ConsoleColor.White);

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
                    // Bounds checking
                    if (commandIndex == commands.Count - 1)
                        return commands[commandIndex = 0].ToString();

                    return commands[++commandIndex].ToString();
                case ConsoleKey.DownArrow:
                    // Bounds checking
                    if (commandIndex == 0)
                        return commands[commandIndex = commands.Count - 1].ToString();

                    return commands[--commandIndex].ToString();
                case ConsoleKey.Enter:
                    return commands[commandIndex].ToString();
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
        public string name;
        public uint age;
        public string jobTitle;
        public bool apprentice;
        public int weeklyHours;

        public int overtimeHours;
        
        public double hourlyPay;
        public double normalPay;
        public double overtimePay;
        public double grossPay;
        public double yearlyPay;

        public string taxCode;
        public double tax;
        public double nationalInsurance;

        public double totalDeductions;
        public double netPay;

        public void PrintEmployeeInformation(Application application)
        {
            application.Print("Name: " + name + "\n");
            application.Print("Age: " + age + "\n");
            application.Print("Job Title: " + jobTitle + "\n");
        }
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

            // Calculate pay
            normalPay = Math.Round(hourlyPay * (weeklyHours * 4), 2);
            overtimePay = Math.Round(hourlyPay * overtimeHours, 2);
            grossPay = Math.Round(normalPay + overtimePay, 2);
            yearlyPay = Math.Round(grossPay * 12);

            // Calculate monthly income tax code and tax
            if (yearlyPay <= 12500)
            {
                taxCode = "944L";
                tax = 0;
            }
            else if (yearlyPay <= 50000)
            {
                taxCode = "1250L";
                tax = Math.Round((yearlyPay / 20) / 12, 2);
            }
            else if (yearlyPay <= 150000)
            {
                taxCode = "5000L";
                tax = Math.Round((yearlyPay / 2) / 12, 2);
            }

            // Calculate montly national insurance
            uint nationalInsurancePercentage = 0;
            if (yearlyPay > 8632 && yearlyPay <= 50000)
            {
                nationalInsurancePercentage = 12;
                nationalInsurance = Math.Round((yearlyPay / nationalInsurancePercentage) / 12, 2);
            }
            else if (yearlyPay > 50000)
            {
                nationalInsurancePercentage = 2;
                nationalInsurance = Math.Round((yearlyPay / nationalInsurancePercentage) / 12, 2);
            }

            // Calculate montly total deduction
            totalDeductions = Math.Round((tax + nationalInsurance), 2);

            // Calculate montly net pay
            netPay = Math.Round(grossPay - totalDeductions, 2);
        }

        public void PrintInformation(Application application)
        {
            application.Print("\n");
            application.Print("Normal Pay: " + "£" + normalPay.ToString() + "\n");
            application.Print("Overtime Pay: " + "£" + overtimePay.ToString() + "\n");
            application.Print("Gross Pay: " + "£" + grossPay.ToString() + "\n");
            application.Print("Tax Code: " + taxCode + "\n");
            application.Print("Tax: " + "£" + tax + "\n");
            application.Print("National Insurance: " + "£" + nationalInsurance + "\n");
            application.Print("Total Deductions: " + "£" + totalDeductions + "\n");
            application.Print("\n");
            application.Print("\n");
            application.Print("Net Pay: " + "£" + netPay + "\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Initialise application
            Application application = new Application("Payroll Mangement System", 80, 30);

            // Initialise employee data
            Employee[] employees = new Employee[2];
            employees[0].name = "Zakariya";
            employees[0].age = 18;
            employees[0].jobTitle = Enum.GetName(typeof(PositionHours), PositionHours.CHEF);
            employees[0].apprentice = false;
            employees[0].weeklyHours = (int)Enum.Parse(typeof(PositionHours), employees[0].jobTitle); // Calculate weekly hours based on the job title

            employees[1].name = "Tom";
            employees[1].age = 45;
            employees[1].jobTitle = Enum.GetName(typeof(PositionHours), PositionHours.WAITER);
            employees[1].apprentice = true;
            employees[1].weeklyHours = (int)Enum.Parse(typeof(PositionHours), employees[1].jobTitle); // Calculate weekly hours based on the job title

            string currentCommand = application.GetCommand(CommandTypes.NEWPAYROLL);
            while (true)
            {
                Console.Clear();
                application.PrintTitle();

                application.Print("Command: ");
                Console.Write(currentCommand);

                // Get command using input (arrow keys)
                currentCommand = application.CommandList();

                // TODO: Fix double key repeat
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

                        for (uint i = 0; i < employees.Length; ++i)
                        {
                            Console.Clear();
                            application.PrintTitle();
                            application.Print("New Payroll\n\n");
                            application.Print(payrollDateRange + "\n\n");

                            employees[i].PrintEmployeeInformation(application);

                            application.Print("\n");
                            application.Print("Hours: " + employees[i].weeklyHours * 4 + "\n");
                            employees[i].overtimeHours = int.Parse(application.Input("Overtime: "));

                            application.Print("\n");
                            application.Print("\n");

                            // Display 'menu' buttons
                            string choice = application.ButtonInput("Exit", "Next");

                            if (choice == "Exit")
                            {

                                application.Print("\n");
                                application.Print("Going Back To Main Menu", ConsoleColor.Yellow);
                                application.Wait();
                                break;

                            }
                            else if (choice == "Next")
                            {
                                application.Print("\n");

                                // Last means there are no more customers
                                if (i == employees.Length - 1)
                                {
                                    application.Print("No More Customers... Exiting", ConsoleColor.Red);
                                    application.Wait();
                                    break;
                                }
                                application.Print("Going To The Next Customer", ConsoleColor.Yellow);
                                application.Wait();
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
                        application.Print("View Payroll\n\n");

                        // Display payslip date range (1 Month)
                        string payrollDateRange = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy") + " - " + DateTime.Today.ToString("dd/MM/yyyy");

                        for (uint i = 0; i < employees.Length; ++i)
                        {
                            Console.Clear();
                            application.PrintTitle();
                            application.Print("View Payroll\n\n");
                            application.Print(payrollDateRange + "\n\n");

                            employees[i].PrintEmployeeInformation(application);
                            employees[i].CalculateEmployeeEarnings();
                            employees[i].PrintInformation(application);

                            // Display 'menu' buttons
                            string choice = application.ButtonInput("Exit", "Next");

                            if (choice == "Exit")
                            {
                                application.Print("\n");
                                application.Print("Going Back To Main Menu", ConsoleColor.Yellow);
                                application.Wait();
                                break;
                            }
                            else if (choice == "Next")
                            {
                                application.Print("\n");

                                // Last means there are no more customers
                                if (i == employees.Length - 1)
                                {
                                    application.Print("No More Customers... Exiting", ConsoleColor.Red);
                                    application.Wait();
                                    break;
                                }
                                application.Print("Going To The Next Customer", ConsoleColor.Yellow);
                                application.Wait();
                                continue;
                            }
                        }
                    }
                    // TODO: If user enters then complete task
                    Thread.Sleep(100);
                }   
            }
        }
    }
}

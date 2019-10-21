using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace PayrollManagementSystem
{
    // Command types interface
    public enum CommandTypes
    {
        NEWPAYROLL, 
        VIEWPAYROLL, 
        NEWEMPLOYEE,
        REMOVEEMPLOYEE
    };

    public class Application
    {
        // Applicaiton member variables
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
            { CommandTypes.REMOVEEMPLOYEE,  "(4) Remove Employee"}
        };

        public Application(string title, int width, int height)
        {
            m_ApplicationTitle = title;
            m_ConsoleWidth = width;
            m_ConsoleHeight = height;

            InitialiseWindow(ConsoleColor.DarkBlue, false);
        }

        ~Application() {}

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

            // Print appliation title
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

        // Get command object based on the command type
        public object GetCommand(CommandTypes commandType)
        {
            return commands[commandType];
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

    public class Employee
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

        public void SetOvertimeHours(int overtime)
        {
            overtimeHours = overtime;
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

            // Initialise example employee data
            List<Employee> employees = new List<Employee>();
            employees.Add(new Employee
            {
                name = "Zakariya",
                age = 18,
                jobTitle = Enum.GetName(typeof(PositionHours), PositionHours.CHEF),
                apprentice = false,
                weeklyHours = (int)Enum.Parse(typeof(PositionHours), "CHEF")
            });

            object currentCommand = application.GetCommand(CommandTypes.NEWPAYROLL);
            while (true)
            {
                // Clear the window when new text is printed
                Console.Clear();
                
                // Print title and accept command selection
                application.PrintTitle();
                application.Print("Command: " + currentCommand);
                currentCommand = application.CommandList();

                // Once the user hits the enter button, complete a certain task
                if (application.IsEnterPressed())
                {
                    // Create a new employee payroll
                    if (currentCommand == application.GetCommand(CommandTypes.NEWPAYROLL))
                    {
                        Console.Clear();
                        application.PrintTitle();
                        application.Print("New Payroll\n\n");

                        // Display payslip date range (1 Month)
                        string payrollDateRange = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy") + " - " + DateTime.Today.ToString("dd/MM/yyyy");

                        for (int i = 0; i < employees.Count; ++i)
                        {
                            // Ensure that overtime input is valid
                            while (true)
                            {
                                Console.Clear();
                                application.PrintTitle();
                                application.Print("New Payroll\n\n");
                                application.Print(payrollDateRange + "\n\n");

                                employees[i].PrintEmployeeInformation(application);

                                application.Print("\n");
                                application.Print("Hours: " + employees[i].weeklyHours * 4 + "\n");
                                try
                                {
                                    employees[i].SetOvertimeHours(int.Parse(application.Input("Overtime: ")));

                                    break;
                                } catch (Exception e)
                                {
                                    application.Print("\n");
                                    application.Print("Invalid input. Try again.", ConsoleColor.Red);
                                    application.Wait();
                                }

                            }
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

                                // Last means there are no more employees
                                if (i < employees.Count)
                                {
                                    application.Print("No More Employees... Exiting", ConsoleColor.Red);
                                    application.Wait();
                                    break;
                                }
                                application.Print("Going To The Next Employee", ConsoleColor.Yellow);
                                application.Wait();
                                continue;
                            }
                        }
                    }
                    
                    // An overview of an employees payroll
                    if (currentCommand == application.GetCommand(CommandTypes.VIEWPAYROLL))
                    {
                        Console.Clear();
                        application.PrintTitle();
                        application.Print("View Payroll\n\n");

                        // Display payslip date range (1 Month)
                        string payrollDateRange = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy") + " - " + DateTime.Today.ToString("dd/MM/yyyy");

                        for (int i = 0; i < employees.Count; ++i)
                        {
                            Console.Clear();
                            application.PrintTitle();
                            application.Print("View Payroll\n\n");
                            application.Print(payrollDateRange + "\n\n");

                            employees[i].PrintEmployeeInformation(application);
                            employees[i].CalculateEmployeeEarnings();
                            employees[i].PrintInformation(application);
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
                                if (i < employees.Count)
                                {
                                    application.Print("No More Employees... Exiting", ConsoleColor.Red);
                                    application.Wait();
                                    break;
                                }
                                application.Print("Going To The Next Employees", ConsoleColor.Yellow);
                                application.Wait();
                                continue;
                            }
                        }
                    }
                    
                    // Add a new customer to the system
                    if (currentCommand == application.GetCommand(CommandTypes.NEWEMPLOYEE))
                    {
                        Console.Clear();
                        application.PrintTitle();
                        application.Print("New Employee\n\n");

                        // Add new employee to the list
                        try
                        {
                            Employee employee = new Employee();
                            employee.name = application.Input("Name: ");
                            employee.age = uint.Parse(application.Input("Age: "));
                            employee.jobTitle = application.Input("Job Title: ");
                            employee.apprentice = bool.Parse(application.Input("Apprentice: "));
                            employee.weeklyHours = (int)Enum.Parse(typeof(PositionHours), employee.jobTitle);

                            employees.Add(employee);

                            application.Print("A new employee has been added!\n", ConsoleColor.Green);
                            application.Wait(3000);
                        } catch (Exception e)
                        {
                            application.Print("\n");
                            application.Print("Error adding new employee!\n", ConsoleColor.Red);
                            application.Print(e.Message + "\n", ConsoleColor.Red);
                            application.Print("Exiting back to main menu...\n", ConsoleColor.Yellow);
                            application.Wait(4000);
                        }
                    }

                    // Remove an employee from the employees list
                    if (currentCommand == application.GetCommand(CommandTypes.REMOVEEMPLOYEE))
                    {
                        Console.Clear();
                        application.PrintTitle();
                        application.Print("Remove Employee\n\n");

                        string name = application.Input("Name: ");
                        if (employees.Remove(employees.Find(employee => employee.name == name)))
                        {
                            application.Print("\n");
                            application.Print("Employee " + name + " has been removed for the list", ConsoleColor.Green);
                        }
                        else
                        {
                            application.Print("Could not find employee " + "'" + name + "'\n", ConsoleColor.Red);
                            application.Print("\n");
                        }

                        application.Wait();
                    }
                }

                // Note: Stops the window from flashing in some instances
                application.Wait(100);
            }
        }
    }
}

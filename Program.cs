using System;
using System.Threading;

namespace PayrollManagementSystem
{
    public class Application
    {
        private int consoleWidth, consoleHeight;
        private int index = 0;

        public string title = "Payroll Mangement System";
        public string[] commands = { "(1) New Payroll", "(2) View Payroll", "(3) Print Payroll", "(4) New Customer" };

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
            Console.SetBufferSize(consoleWidth, consoleHeight);
            Console.BackgroundColor = ConsoleColor.DarkBlue;
        }

        // Print strings onto the console
        public void Print(object message, ConsoleColor textColor = ConsoleColor.White, int position = 0)
        {
            // Set cursor position and print message
            Console.SetCursorPosition((Console.WindowWidth - title.Length) / 2 + position, Console.CursorTop);
            Console.ForegroundColor = textColor;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Print message and accept user input 
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
                Print((i < title.Length - 1) ? "*" : "*\n", ConsoleColor.White, i);

            // Print appliation title
            Print(title + "\n");

            // Print bottom title border
            for (int i = 0; i < title.Length; ++i)
                Print((i < title.Length - 1) ? "*" : "*\n", ConsoleColor.White, i);
        }

        // Filter through commands
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

    }


    class Program
    {

        public static void CalculateEmployeeEarnings(ref Employee employee)
        {
            // Calculate hourly pay
            if (employee.age < 17)
                employee.hourlyPay = 3.72;
            else if (employee.age < 19 && employee.apprentice == true)
                employee.hourlyPay = 2.68;
            else if (employee.age >= 19 && employee.apprentice == true)
                employee.hourlyPay = 2.68;
            else if (employee.age <= 20)
                employee.hourlyPay = 5.03;
            else if (employee.age >= 21)
                employee.hourlyPay = 6.31;

            employee.normalPay = employee.hourlyPay * (employee.weeklyHours * 4);
            employee.overtimePay = employee.hourlyPay * employee.overtimeHours;
            employee.grossPay = employee.normalPay + employee.overtimePay;
        }

        static void Main(string[] args)
        {
            // Initialise application
            Application application = new Application(50, 20);

            // Initialise employee data
            Employee[] employees = new Employee[33];
            employees[0].id = 22;
            employees[0].name = "Zakariya";
            employees[0].age = 18;
            employees[0].jobTitle = Enum.GetName(typeof(PositionHours), PositionHours.WAITERS);
            employees[0].nationalInsurance = 4520;
            employees[0].taxCode = "9LUQ";

            // Calculate weekly hours based on the job title
            employees[0].weeklyHours = (int)Enum.Parse(typeof(PositionHours), employees[0].jobTitle);

            string currentCommand = application.commands[0];
            while (true)
            {
                Console.Clear();
                application.PrintTitle();
                application.Print("Command: ");
                Console.Write(currentCommand);

                // Get user command
                currentCommand = application.GetCommand();

                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    switch (currentCommand)
                    {
                        case "(1) New Payroll":
                            Console.Clear();
                            application.PrintTitle();
                            string payrollDateRange = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy") + " - " + DateTime.Today.ToString("dd/MM/yyyy");
                            application.Print("New Payroll (" + payrollDateRange + ")\n\n");
                            
                            string id = application.Input("Employee ID: ");
                            application.Print("\n");

                            // If an employee is found, print their information
                            if (id == employees[0].id.ToString())
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

                            application.Print("\n");
                            application.Print("Hours: " + employees[0].weeklyHours * 4 + "\n");
                            employees[0].overtimeHours = int.Parse(application.Input("Overtime: "));

                            break;
                        
                        case "(2) View Payroll":
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

                            CalculateEmployeeEarnings(ref employees[0]);
                            application.Print("\n");
                            application.Print("Normal Pay: " + "£" + employees[0].normalPay.ToString() + "\n");
                            application.Print("Overtime Pay: " + "£" + employees[0].overtimePay.ToString() + "\n");
                            application.Input("Gross Pay: " + "£" + employees[0].grossPay.ToString() + "\n");

                            break;
                    }
                }

                // TODO: If user enters then complete task
                Thread.Sleep(100);
            }

        }
    }
}

using System;
using System.IO;
using System.Collections.Specialized;
using System.Threading;
using System.Xml;

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
        public string m_FilePath = "Employees.xml";

        // Used for storage and retrival of information using a file (XML)
        public XmlDocument employeeData = new XmlDocument();
        public XmlElement employeeRoot;
        
        // Instance/Interface to employee class
        public Employee employee = new Employee();

        // An ordered array of commands (Key, Value)
        private OrderedDictionary commands = new OrderedDictionary
        {
            { CommandTypes.NEWPAYROLL,      "(1) New Payroll"     },
            { CommandTypes.VIEWPAYROLL,     "(2) View Payroll"    },
            { CommandTypes.NEWEMPLOYEE,     "(3) New Employee"    },
            { CommandTypes.REMOVEEMPLOYEE,  "(4) Remove Employee" },
            { CommandTypes.HELP,            "(5) Help"            }
        };

        // Class constructor
        public Application(string title, int width, int height)
        {
            // Set basic console properties
            m_ApplicationTitle = title;
            m_ConsoleWidth = width;
            m_ConsoleHeight = height;

            // Initialise console window using set properties
            InitialiseWindow(ConsoleColor.DarkBlue, false);
        }

        // Class destructor
        ~Application() { }

        // Initialise the console window
        private void InitialiseWindow(ConsoleColor backgroundColor, bool cursorVisible)
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

        // Return string object based on enum key
        public object GetCommand(CommandTypes command)
        {
            return commands[command];
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

        // Create or load an employee data xml file
        public void InitialiseEmployeeData()
        {
            // If the employee data file does not exist then create a new XML document and write to disk
            if (!File.Exists(m_FilePath))
            {
                // Initialise XML file with declartion information and file encoding
                XmlDeclaration xmlDeclaration = employeeData.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = employeeData.DocumentElement;
                employeeData.InsertBefore(xmlDeclaration, root);

                // Create root XML node and add it to the document
                employeeRoot = employeeData.CreateElement(string.Empty, "Employees", string.Empty);
                employeeData.AppendChild(employeeRoot);

                // Save all changes made to the document to the file
                employeeData.Save(m_FilePath);
            }
            else
            {
                // If the xml file exists, simply load it into memory and set the current root
                employeeData.Load(m_FilePath);
                employeeRoot = employeeData.DocumentElement;
            }
        }

        // Add elements to an XML file
        public void StoreXMLElement(ref XmlElement rootElement, string elementName, string variable)
        {
            XmlElement element = employeeData.CreateElement(string.Empty, elementName, string.Empty);
            XmlText text = employeeData.CreateTextNode(variable);
            element.AppendChild(text);
            rootElement.AppendChild(element);
        }

        // Retrive a XML element value using the node and parent element key
        public string GetValueFromFile(XmlNode node, string key)
        {
            return node.SelectSingleNode(key).InnerText;
        }

        // Main application loop
        public void Loop()
        {
            // Main application loop
            InitialiseEmployeeData();

            // Display initial command on startup
            object currentCommand = GetCommand(CommandTypes.NEWPAYROLL);

            // Main application loop
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
                    if (currentCommand == GetCommand(CommandTypes.NEWPAYROLL))
                    {
                        Console.Clear();
                        PrintTitle();
                        Print("New Payroll\n\n");

                        // Display payslip date range (1 Month)
                        string payrollDateRange = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy") + " - " + DateTime.Today.ToString("dd/MM/yyyy");

                        XmlNodeList nodeList = employeeData.GetElementsByTagName("Employee");
                        for (int i = 0; i < nodeList.Count; ++i)
                        {
                            // Ensure that overtime input is valid
                            while (true)
                            {
                                Console.Clear();
                                PrintTitle();
                                Print("New Payroll\n\n");
                                Print(payrollDateRange + "\n\n");

                                // Print employee information
                                Print("Name: " + GetValueFromFile(nodeList[i], "Name") + "\n");
                                Print("Age: " + GetValueFromFile(nodeList[i], "Age") + "\n");
                                Print("Job Title: " + GetValueFromFile(nodeList[i], "JobTitle") + "\n");
                                Print("\n");
                                Print("Hours: " + GetValueFromFile(nodeList[i], "WeeklyHours") + "\n");

                                //Print("Hours: " + int.Parse(nodeList[i].SelectSingleNode("WeeklyHours").InnerText) * 4 + "\n");

                                try
                                {
                                    nodeList[i].SelectSingleNode("OvertimeHours").InnerText = Input("Overtime: ");
                                    employeeData.Save(m_FilePath);
                                    break;
                                }
                                catch (Exception)
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
                                if (i < nodeList.Count - 1)
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
                    if (currentCommand == GetCommand(CommandTypes.VIEWPAYROLL))
                    {
                        Console.Clear();
                        PrintTitle();
                        Print("View Payroll\n\n");

                        // Display payslip date range (1 Month)
                        string payrollDateRange = DateTime.Today.AddMonths(-1).ToString("dd/MM/yyyy") + " - " + DateTime.Today.ToString("dd/MM/yyyy");

                        XmlNodeList nodeList = employeeData.GetElementsByTagName("Employee");
                        for (int i = 0; i < nodeList.Count; ++i)
                        {
                            Console.Clear();
                            PrintTitle();
                            Print("View Payroll\n\n");
                            Print(payrollDateRange + "\n\n");

                            // Print employee information
                            employee.PrintEmployeeInformation(this, nodeList[i]);

                            // Calculate employee payroll
                            employee.Calculate(this, nodeList[i]);

                            employee.PrintPayroll(this, nodeList[i]);

                            // Save data into file
                            employeeData.Save(m_FilePath);

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
                                if (i < nodeList.Count - 1)
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
                    if (currentCommand == GetCommand(CommandTypes.NEWEMPLOYEE))
                    {
                        Console.Clear();
                        PrintTitle();
                        Print("New Employee\n\n");

                        // Add new employee
                        try
                        {
                            employee.name = Input("Name: ");
                            employee.age = uint.Parse(Input("Age: "));
                            employee.jobTitle = Input("Job Title: ");
                            employee.apprentice = bool.Parse(Input("Apprentice: "));
                            employee.weeklyHours = (int)Enum.Parse(typeof(PositionHours), employee.jobTitle);

                            // Store employee data into a file
                            employee.StoreEmployee(this, employee);

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
                    if (currentCommand == GetCommand(CommandTypes.REMOVEEMPLOYEE))
                    {
                        Console.Clear();
                        PrintTitle();
                        Print("Remove Employee\n\n");

                        string name = Input("Name: ");

                        // Remove an employee from the file
                        employee.RemoveEmployee(this, name);

                        Wait();
                    }

                    // Open a help file
                    if (currentCommand == GetCommand(CommandTypes.HELP))
                    {
                        OpenFile(@"Help.txt");
                    }
                }

                // Note: Stops the window from flashing in some instances
                Wait(100);
            }
        }
    }
}

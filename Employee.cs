using System;
using System.Xml;

namespace PayrollManagementSystem
{
    // List of hours for each position
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

    // Employee class
    public class Employee
    {
        public string name;
        public uint age;
        public string jobTitle;
        public bool apprentice;
        public int weeklyHours;

        private uint overtimeHours;

        private double hourlyPay;
        private double normalPay;
        private double overtimePay;
        private double grossPay;
        private double yearlyPay;

        private string taxCode;
        private double tax;
        private double nationalInsurance;

        private double totalDeductions;
        private double netPay;

        // Store employee data into xml file
        public void StoreEmployee(Application application, Employee employee)
        {
            // Create root element for all employees
            XmlElement employeeGroup = application.employeeData.CreateElement(string.Empty, "Employee", string.Empty);
            application.employeeRoot.AppendChild(employeeGroup);

            // Each employee will consist of the following elements
            application.StoreXMLElement(ref employeeGroup, "Name",              employee.name.ToString());
            application.StoreXMLElement(ref employeeGroup, "Age",               employee.age.ToString());
            application.StoreXMLElement(ref employeeGroup, "JobTitle",          employee.jobTitle);
            application.StoreXMLElement(ref employeeGroup, "Apprentice",        employee.apprentice.ToString());
            application.StoreXMLElement(ref employeeGroup, "WeeklyHours",       employee.weeklyHours.ToString());
            application.StoreXMLElement(ref employeeGroup, "OvertimeHours",     employee.overtimeHours.ToString());
            application.StoreXMLElement(ref employeeGroup, "HourlyPay",         employee.hourlyPay.ToString());
            application.StoreXMLElement(ref employeeGroup, "NormalPay",         employee.normalPay.ToString());
            application.StoreXMLElement(ref employeeGroup, "OvertimePay",       employee.overtimePay.ToString());
            application.StoreXMLElement(ref employeeGroup, "GrossPay",          employee.grossPay.ToString());
            application.StoreXMLElement(ref employeeGroup, "TaxCode",           employee.taxCode);
            application.StoreXMLElement(ref employeeGroup, "Tax",               employee.tax.ToString());
            application.StoreXMLElement(ref employeeGroup, "NationalInsurance", employee.nationalInsurance.ToString());
            application.StoreXMLElement(ref employeeGroup, "TotalDeductions",   employee.totalDeductions.ToString());
            application.StoreXMLElement(ref employeeGroup, "NetPay",            employee.netPay.ToString());


            // Write to XML file
            application.employeeData.Save(application.m_FilePath);
        }

        // Remove an employee along with their data from the xml file
        public void RemoveEmployee(Application application, string employeeName)
        {
            // Search for the employees name in the XML file
            XmlNode node = application.employeeData.SelectSingleNode("/Employees/Employee[Name='" + employeeName + "']");

            // Check if employee has been found
            if (node != null)
            {
                // Get parent node of the employee name element
                XmlNode parentNode = node.ParentNode;

                // Remove element
                parentNode.RemoveChild(node);

                // Save the XML file
                application.employeeData.Save(application.m_FilePath);

                application.Print("Employee " + employeeName + " has been removed\n", ConsoleColor.Green);
            }
            else
            {
                application.Print("\n");
                application.Print("Error removing employee!\n", ConsoleColor.Red);
                application.Print("Exiting back to main menu...\n", ConsoleColor.Yellow);
            }
        }

        // Display basic employee information
        public void PrintEmployeeInformation(Application application, XmlNode node)
        {
            application.Print("Name: " + application.GetValueFromFile(node, "Name") + "\n");
            application.Print("Age: " + application.GetValueFromFile(node, "Age") + "\n");
            application.Print("Job Title: " + application.GetValueFromFile(node, "JobTitle") + "\n");
        }

        // Set employee overtime hours
        public void SetOvertimeHours(uint overtime)
        {
            overtimeHours = overtime;
        }

        // Calculate hourly pay
        private void CalculateHourlyPay(Application application, XmlNode node)
        {
            // Calculate hourly pay
            uint age = uint.Parse(application.GetValueFromFile(node, "Age"));
            bool apprentice = bool.Parse(application.GetValueFromFile(node, "Apprentice"));

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
        }

        // Calculate overall pay
        private void CalculatePay()
        {
            // Calculate pay
            normalPay = Math.Round(hourlyPay * (weeklyHours * 4), 2);
            overtimePay = Math.Round(hourlyPay * overtimeHours, 2);
            grossPay = Math.Round(normalPay + overtimePay, 2);
            yearlyPay = Math.Round(grossPay * 12);
        }

        // Calculate tax
        private void CalculateTax()
        {
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

            // Calculate monthly national insurance
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
        }

        // Main calculation
        public void Calculate(Application application, XmlNode node)
        {
            CalculateHourlyPay(application, node);

            weeklyHours = int.Parse(application.GetValueFromFile(node, "WeeklyHours"));
            overtimeHours = uint.Parse(application.GetValueFromFile(node, "OvertimeHours"));

            CalculatePay();
            CalculateTax();

            // Calculate monthly total deduction
            totalDeductions = Math.Round((tax + nationalInsurance), 2);

            // Calculate monthly net pay
            netPay = Math.Round(grossPay - totalDeductions, 2);


            // Write values into file
            node.SelectSingleNode("HourlyPay").InnerText = hourlyPay.ToString();
            node.SelectSingleNode("NormalPay").InnerText = normalPay.ToString();
            node.SelectSingleNode("OvertimePay").InnerText = overtimePay.ToString();
            node.SelectSingleNode("GrossPay").InnerText = grossPay.ToString();
            node.SelectSingleNode("TaxCode").InnerText = taxCode.ToString();
            node.SelectSingleNode("Tax").InnerText = tax.ToString();
            node.SelectSingleNode("NationalInsurance").InnerText = nationalInsurance.ToString();
            node.SelectSingleNode("TotalDeductions").InnerText = totalDeductions.ToString();
            node.SelectSingleNode("NetPay").InnerText = netPay.ToString();
        }

        // Display payroll
        public void PrintPayroll(Application application, XmlNode node)
        {
            application.Print("Normal Pay: " + "£" +            application.GetValueFromFile(node, "NormalPay") + "\n");
            application.Print("Overtime Pay: " + "£" +          application.GetValueFromFile(node, "OvertimePay") + "\n");
            application.Print("Gross Pay: " + "£" +             application.GetValueFromFile(node, "GrossPay") + "\n");
            application.Print("Tax Code: " +                    application.GetValueFromFile(node, "TaxCode") + "\n");
            application.Print("Tax: " + "£" +                   application.GetValueFromFile(node, "Tax") + "\n");
            application.Print("National Insurance: " + "£" +    application.GetValueFromFile(node, "NationalInsurance") + "\n");
            application.Print("Total Deductions: " + "£" +      application.GetValueFromFile(node, "TotalDeductions") + "\n");
            application.Print("\n");                            
            application.Print("Net Pay: " + "£" +               application.GetValueFromFile(node, "NetPay") + "\n");
        }
    }
}

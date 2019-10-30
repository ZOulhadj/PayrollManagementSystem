using System;

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

    interface Employee
    {
        void CalculateHourlyPay();

        void CalculatePay();

        void CalculateTax();

        void CalculateNI();
    }

    // Employee class
    public class Person : Employee
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

        // Display employee information
        public void PrintEmployeeInformation(Application application)
        {
            application.Print("Name: " + name + "\n");
            application.Print("Age: " + age + "\n");
            application.Print("Job Title: " + jobTitle + "\n");
        }

        // Set employee overtime hours
        public void SetOvertimeHours(int overtime)
        {
            overtimeHours = overtime;
        }

        public void CalculateHourlyPay()
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
        }

        public void CalculatePay()
        {
            // Calculate pay
            normalPay = Math.Round(hourlyPay * (weeklyHours * 4), 2);
            overtimePay = Math.Round(hourlyPay * overtimeHours, 2);
            grossPay = Math.Round(normalPay + overtimePay, 2);
            yearlyPay = Math.Round(grossPay * 12);
        }

        public void CalculateTax()
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
        }

        public void CalculateNI()
        {
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

        // Calculate 
        public void CalculateEmployeeEarnings()
        {
            // Calculations
            CalculateHourlyPay();
            CalculatePay();
            CalculateTax();
            CalculateNI();

            // Calculate monthly total deduction
            totalDeductions = Math.Round((tax + nationalInsurance), 2);

            // Calculate monthly net pay
            netPay = Math.Round(grossPay - totalDeductions, 2);
        }

        public void PrintPayrollInformation(Application application)
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
}

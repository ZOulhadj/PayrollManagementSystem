namespace PayrollManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialise application
            Application application = new Application("Payroll Management System", 80, 30);

            // Main application loop
            application.Loop();
        }
    }
}

namespace OOP_Group_Final_Project
{
    public class Employee
    {
        public int EmployeeID { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Position { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int Performance { get; set; }
        public DateTime DateHired { get; set; }
        public DateTime? DateDeparted { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace OOP_Group_Final_Project
{
    public abstract class Employee
    {
        public int EmployeeID { get; set; }


        [Required] public string FirstName { get; set; } = string.Empty;


        [Required] public string LastName { get; set; } = string.Empty;


        [Required] public string Position { get; set; } = string.Empty;

        public decimal Pay { get; set; }


        [Required, EmailAddress] public string Email { get; set; } = string.Empty;

        public DateTime DateHired { get; set; } = DateTime.Today;
        public DateTime? DateDeparted { get; set; }

        public abstract decimal GetPay(decimal period = default);

    }
}

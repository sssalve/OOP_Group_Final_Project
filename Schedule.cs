using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOP_Group_Final_Project;

namespace OOP_Group_Final_Project
{
	public class Schedule
	{
		public int Id { get; set; }
		public DateTime Date { get; set; }
		public int EmployeeId { get; set; }
		public Employee? Employee { get; set; }
		public string? ShiftType { get; set; }
		public string? Notes { get; set; }
	}
}

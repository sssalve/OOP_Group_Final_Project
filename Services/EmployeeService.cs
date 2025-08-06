using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OOP_Group_Final_Project.Services
{
	public interface IEmployeeService
	{
		Task<List<Employee>> GetAllEmployeesAsync();
		Task<Employee> GetEmployeeByIdAsync(int id);
		Task AddEmployeeAsync(Employee employee);
		Task UpdateEmployeeAsync(Employee employee);
		Task DeleteEmployeeAsync(int id);
		Task<bool> EmployeeExistsAsync(int id);
	}
	public class EmployeeService : IEmployeeService
	{
		private List<Employee> _employees = new List<Employee>();
		private int _nextId = 1;

		public EmployeeService() {
			// add test/temp employees (Delete Later)
			_employees.Clear();
			_employees = new List<Employee>
			{
				new() {EmployeeID = _nextId++, FirstName = "John", LastName = "Doe", Position = "Dev"},
				new() {EmployeeID = _nextId++, FirstName = "Jane", LastName = "Smith", Position = "Dev" }
			};
		}

		public async Task<List<Employee>> GetAllEmployeesAsync()
		{
			return await Task.FromResult(_employees);
		}

		public async Task<Employee> GetEmployeeByIdAsync(int id)
		{
			return await Task.FromResult(_employees.FirstOrDefault(e => e.EmployeeID == id));
		}

		public async Task AddEmployeeAsync(Employee employee)
		{
			employee.EmployeeID = _nextId++;
			_employees.Add(employee);
			await Task.CompletedTask;
		}

		public async Task UpdateEmployeeAsync(Employee employee)
		{
			var existing = _employees.FirstOrDefault(e => e.EmployeeID == employee.EmployeeID);

			if (existing != null)
			{
				existing.FirstName = employee.FirstName;
				existing.LastName = employee.LastName;
				existing.Position = employee.Position;
				existing.Email = employee.Email;
			}
			await Task.CompletedTask;
		}

		public async Task DeleteEmployeeAsync(int id)
		{
			var employee = _employees.FirstOrDefault(e => e.EmployeeID == id);
			
			if (employee != null)
			{
				_employees.Remove(employee);
			}
			await Task.CompletedTask;
		}

		public async Task<bool> EmployeeExistsAsync(int id)
		{
			return await Task.FromResult(_employees.Any(e => e.EmployeeID == id));
		}
	}
}

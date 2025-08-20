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

        public EmployeeService()
        {
            // add test/temp employees (Delete Later)
            _employees.Clear();
            _employees = new List<Employee>
            {
                new Salaried() {EmployeeID = _nextId++, FirstName = "John", LastName = "Doe", Position = "Dev", Salary = 85000, Pay = 85000, Email = "legitemail@real.com", Performance = 4, DateHired = new DateTime(2023, 09, 23), DateDeparted = null},
                new FullTime() {EmployeeID = _nextId++, FirstName = "Jane", LastName = "Smith", Position = "Dev", Wage = 35,  Pay = 35, Email = "xXn00bDestroyerXx@gamer.com", Performance = 2, DateHired = new DateTime(2021, 01, 10), DateDeparted = new DateTime(2021, 06, 10) }
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

            if (employee.Performance is < 1 or > 5)
                throw new ArgumentOutOfRangeException(nameof(employee.Performance), "Performance must be 1–5.");
            if (employee.DateHired == default)
                employee.DateHired = DateTime.Today;

            _employees.Add(employee);
            await Task.CompletedTask;
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            var existing = _employees.FirstOrDefault(e => e.EmployeeID == employee.EmployeeID);

            Salaried updatedSalaried = new Salaried();
            FullTime updatedFullTime = new FullTime();

            if (employee.GetType() is Salaried && existing != null)
            {
                updatedSalaried.EmployeeID = employee.EmployeeID;
                updatedSalaried.FirstName = employee.FirstName;
                updatedSalaried.LastName = employee.LastName;
                updatedSalaried.Position = employee.Position;
                updatedSalaried.Pay = employee.Pay;
                updatedSalaried.Salary = employee.Pay;
                updatedSalaried.Email = employee.Email;
                if (employee.Performance is < 1 or > 5)
                    throw new ArgumentOutOfRangeException(nameof(employee.Performance), "Performance must be 1–5.");
                if (employee.DateHired != default)
                {
                    updatedSalaried.DateHired = employee.DateHired;
                }
                updatedSalaried.DateDeparted = employee.DateDeparted;
                existing = updatedSalaried;
            }
            else if (employee.GetType() is FullTime && existing != null)
            {
                updatedFullTime.EmployeeID = employee.EmployeeID;
                updatedFullTime.FirstName = employee.FirstName;
                updatedFullTime.LastName = employee.LastName;
                updatedFullTime.Position = employee.Position;
                updatedFullTime.Pay = employee.Pay;
                updatedFullTime.Wage = employee.Pay;
                updatedFullTime.Email = employee.Email;
                if (employee.Performance is < 1 or > 5)
                    throw new ArgumentOutOfRangeException(nameof(employee.Performance), "Performance must be 1–5.");
                if (employee.DateHired != default)
                {
                    updatedFullTime.DateHired = employee.DateHired;
                }
                updatedFullTime.DateDeparted = employee.DateDeparted;
                existing = updatedFullTime;
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
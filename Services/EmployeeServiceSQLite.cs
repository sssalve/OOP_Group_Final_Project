using Microsoft.Data.Sqlite;
using OOP_Group_Final_Project.Database;
using System.Globalization;
namespace OOP_Group_Final_Project.Services;

internal class EmployeeServiceSQLite : IEmployeeService //uses the interface from employee serivice that alex created 
{
    private static readonly string DateFmt = "yyyy-MM-dd"; //gives the format for the dates 

    private static bool HasColumn(SqliteDataReader r, string name) //check that all the columns exists in the new writable copy compared to the readonly.
    {
        for (int i = 0; i < r.FieldCount; i++)
            if (string.Equals(r.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }

    //makes the db dates to match the yyyy-mm-dd format and null format
    private static DateTime ParseDate(string s) =>
        DateTime.ParseExact(s, DateFmt, CultureInfo.InvariantCulture);

    private static DateTime? ParseNullableDate(SqliteDataReader r, int ord) =>
        r.IsDBNull(ord) ? (DateTime?)null : ParseDate(r.GetString(ord));

    //----------------------------------------------------------------------------------------
    //Read all the employees and uses Map() to assing concrete types of employees (salary and houly)
    public async Task<List<Employee>> GetAllEmployeesAsync()
    {
        var list = new List<Employee>();
        using var conn = new SqliteConnection(EmployeeDb.ConnString);
        await conn.OpenAsync();

        const string sql = "SELECT * FROM employees ORDER BY employee_id;"; //query to read all the employees
        using var cmd = new SqliteCommand(sql, conn);
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
            list.Add(Map(rd));
        return list;
    }
    //----------------------------------------------------------------------------------------
    //matches salaried vs fullTime by looking at salary vs hourly_rate 
    private static Employee Map(SqliteDataReader r) // i founf map usefull. is a helper that takes one row from SQLite and turns it into your domain object (Employee). and then pass that tp all the other methods
    {
        // old columns
        int oId = r.GetOrdinal("employee_id");
        int oFn = r.GetOrdinal("first_name");
        int oLn = r.GetOrdinal("last_name");
        int oPos = r.GetOrdinal("position");
        int oSal = r.GetOrdinal("salary");
        int oMail = r.GetOrdinal("email");
        int oHire = r.GetOrdinal("date_hired");
        int oDep = r.GetOrdinal("date_departed");

        // New columns
        int oRate = HasColumn(r, "hourly_rate") ? r.GetOrdinal("hourly_rate") : -1;
        int oType = HasColumn(r, "type") ? r.GetOrdinal("type") : -1;

        // Decide employee kind:
        // 1) If there's a 'type' column, use it (Hourly/Salaried)
        // 2) Else infer: if hourly_rate > 0 and salary <= 0 => hourly
        bool isHourly;
        if (oType >= 0)
        {
            var t = r.IsDBNull(oType) ? "" : r.GetString(oType);
            isHourly = t.Equals("Hourly", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            double sal = r.IsDBNull(oSal) ? 0d : r.GetDouble(oSal);
            double rate = (oRate >= 0 && !r.IsDBNull(oRate)) ? r.GetDouble(oRate) : 0d;
            isHourly = rate > 0 && sal <= 0;
        }

        Employee e = isHourly ? new FullTime() : new Salaried();

        // Common fields
        e.EmployeeID = r.GetInt32(oId);
        e.FirstName = r.GetString(oFn);
        e.LastName = r.GetString(oLn);
        e.Position = r.IsDBNull(oPos) ? string.Empty : r.GetString(oPos);
        e.Email = r.IsDBNull(oMail) ? string.Empty : r.GetString(oMail);
        e.DateHired = ParseDate(r.GetString(oHire));
        e.DateDeparted = ParseNullableDate(r, oDep);

        // Pay fields this was driving me nuts lol 
        if (e is Salaried se)
        {
            var salary = r.IsDBNull(oSal) ? 0d : r.GetDouble(oSal);
            se.Salary = Convert.ToDecimal(salary);
            e.Pay = se.Salary;
        }
        else if (e is FullTime ft)
        {
            var rate = (oRate >= 0 && !r.IsDBNull(oRate)) ? r.GetDouble(oRate) : 0d;

            ft.Wage = Convert.ToDecimal(rate);
            e.Pay = ft.Wage;
        }

        return e;
    }

    //----------------------------------------------------------------------------------------
    //gets a single employee by id. Returns null when not found.
    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        using var conn = new SqliteConnection(EmployeeDb.ConnString);
        await conn.OpenAsync();

        const string sql = "SELECT * FROM employees WHERE employee_id=@id;";
        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var rd = await cmd.ExecuteReaderAsync();
        return await rd.ReadAsync() ? Map(rd) : null;
    }

    //-----------------------------------------------------------------------------
    //adds a new employe. checks the date hired is there. matches the right pay type to the right type of employee
    //gets new id based on the last row (basically increments 1 for whatever the last employee id was)
    public async Task AddEmployeeAsync(Employee e)
    {
        if (e.DateHired == default)
            e.DateHired = DateTime.Today;

        if (e is Salaried se && se.Salary == 0) se.Salary = e.Pay;  //check type of pay/employee
        if (e is FullTime ft && ft.Wage == 0) ft.Wage = e.Pay;

        using var conn = new SqliteConnection(EmployeeDb.ConnString);
        await conn.OpenAsync();

        using var tx = await conn.BeginTransactionAsync();

        //insrt query
        const string insertSql = @"
        INSERT INTO employees 
        (first_name, last_name, position, salary, email, date_hired, date_departed, hourly_rate) 
        VALUES (@fn, @ln, @pos, @sal, @mail, @hired, @departed, @rate);";

        using (var cmd = new SqliteCommand(insertSql, conn, (SqliteTransaction)tx))
        {
            cmd.Parameters.AddWithValue("@fn", e.FirstName);
            cmd.Parameters.AddWithValue("@ln", e.LastName);
            cmd.Parameters.AddWithValue("@pos", (object?)e.Position ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@mail", (object?)e.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@hired", e.DateHired.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            cmd.Parameters.AddWithValue("@departed", (object?)e.DateDeparted?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? DBNull.Value);

            if (e is Salaried se2)
            {
                cmd.Parameters.AddWithValue("@sal", Convert.ToDouble(se2.Salary));
                cmd.Parameters.AddWithValue("@rate", DBNull.Value);
            }
            else if (e is FullTime ft2)
            {
                cmd.Parameters.AddWithValue("@sal", DBNull.Value);
                cmd.Parameters.AddWithValue("@rate", Convert.ToDouble(ft2.Wage));
            }
            else
            {
                cmd.Parameters.AddWithValue("@sal", Convert.ToDouble(e.Pay));
                cmd.Parameters.AddWithValue("@rate", DBNull.Value);
            }

            await cmd.ExecuteNonQueryAsync();
        }

        using (var idCmd = new SqliteCommand("SELECT last_insert_rowid();", conn, (SqliteTransaction)tx))
        {
            var newId = (long)await idCmd.ExecuteScalarAsync();
            e.EmployeeID = (int)newId;
        }

        await tx.CommitAsync();
    }
    //------------------------------------------------------------------------------------------------------------------
    public async Task UpdateEmployeeAsync(Employee e)
    {

        //keep Pay in sync with the right employee type. pretty similar to add new 
        if (e is Salaried se) se.Salary = e.Pay;
        if (e is FullTime ft) ft.Wage = e.Pay;

        using var conn = new SqliteConnection(EmployeeDb.ConnString);
        await conn.OpenAsync();

        const string sql = @"
        UPDATE employees
        SET first_name=@fn, last_name=@ln, position=@pos,
            salary=@sal, email=@mail,
            date_hired=@hired, date_departed=@departed,
            hourly_rate=@rate
        WHERE employee_id=@id;";
        using var cmd = new SqliteCommand(sql, conn);

        cmd.Parameters.AddWithValue("@fn", e.FirstName);
        cmd.Parameters.AddWithValue("@ln", e.LastName);
        cmd.Parameters.AddWithValue("@pos", (object?)e.Position ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@mail", (object?)e.Email ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@hired", (e.DateHired == default ? DateTime.Today : e.DateHired).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        cmd.Parameters.AddWithValue("@departed", (object?)e.DateDeparted?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", e.EmployeeID);

        if (e is Salaried se2)
        {
            cmd.Parameters.AddWithValue("@sal", Convert.ToDouble(se2.Salary));
            cmd.Parameters.AddWithValue("@rate", DBNull.Value);
        }
        else if (e is FullTime ft2)
        {
            cmd.Parameters.AddWithValue("@sal", DBNull.Value);
            cmd.Parameters.AddWithValue("@rate", Convert.ToDouble(ft2.Wage));
        }
        else
        {
            cmd.Parameters.AddWithValue("@sal", Convert.ToDouble(e.Pay));
            cmd.Parameters.AddWithValue("@rate", DBNull.Value);
        }

        await cmd.ExecuteNonQueryAsync();

    }

    //------------------------------------------------------------------------------------------------------
    public async Task DeleteEmployeeAsync(int id)
    {
        using var conn = new SqliteConnection(EmployeeDb.ConnString);
        await conn.OpenAsync();

        const string sql = "DELETE FROM employees WHERE employee_id=@id;";
        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        await cmd.ExecuteNonQueryAsync();
    }

    //------------------------------------------------------------------------------------------------------
    public async Task<bool> EmployeeExistsAsync(int id)
    {
        using var conn = new SqliteConnection(EmployeeDb.ConnString);
        await conn.OpenAsync();

        const string sql = "SELECT 1 FROM employees WHERE employee_id=@id LIMIT 1;";
        using var cmd = new SqliteCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        return (await cmd.ExecuteScalarAsync()) is not null;
    }


}

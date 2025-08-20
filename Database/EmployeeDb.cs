using Microsoft.Data.Sqlite;

namespace OOP_Group_Final_Project.Database
{
    internal class EmployeeDb
    {
        public const string AssetName = "employees.db"; //database file name in resources folder. 

        public static string DbPath => Path.Combine(FileSystem.AppDataDirectory, AssetName); //path for the writable copy. basically it creates a new folder with a copy of the db that can be manipulated. it generates once when the program is ran by the first time

        public static string ConnString => $"Data Source={DbPath};Cache=Shared"; //connection string 

        public static async Task InitializeAsync()  //it makes a copy of the db file to make it writable.
        {
            Directory.CreateDirectory(FileSystem.AppDataDirectory);

            // if the path doesn't exist, it creates a new one for the writable copy
            if (!File.Exists(DbPath))
            {
                using var src = await FileSystem.OpenAppPackageFileAsync(AssetName);
                using var dst = File.Create(DbPath); //creates the actual copy
                await src.CopyToAsync(dst);
            }

            // open a connectio to check the path is valid
            using var conn = new SqliteConnection(ConnString);
            await conn.OpenAsync();
        }
    }
}

using System;
using Microsoft.Data.SqlClient;
using Azure.Identity;
using System.Text;

class Program
{
    static async Task Main(string[] args)
    {
        // Define your connection string without user credentials
        string connectionString = "Server=tcp:abaranwal-sql-srv.database.windows.net,1433;Initial Catalog=abaranwal-primary;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            Console.WriteLine("Connected to database.");

            // Get initial count
            int initialCount = GetRecordCount(connection);
            Console.WriteLine($"Initial record count: {initialCount}");

            // Continuous read/write loop
            while (true)
            {
                // Generate a random 5-character string
                string randomString = GenerateRandomString(5);
                string dataToInsert = $"Test Data {randomString}";

                // Write data
                using (SqlCommand command = new SqlCommand($"INSERT INTO YourTable (Column1) VALUES (@Data)", connection))
                {
                    command.Parameters.AddWithValue("@Data", dataToInsert);
                    command.ExecuteNonQuery();
                    Console.WriteLine($"Inserted: {dataToInsert}");
                }

                // Get new count and check if it increased by 1
                int newCount = GetRecordCount(connection);
                Console.WriteLine($"New record count: {newCount}");

                if (newCount == initialCount + 1)
                {
                    Console.WriteLine("Record count increased by 1 as expected.");
                }
                else
                {
                    Console.WriteLine("Unexpected record count increase.");
                }

                initialCount = newCount;

                Thread.Sleep(500); // Wait for .5 seconds before next operation
            }

        }

    }

    static int GetRecordCount(SqlConnection connection)
    {
        using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM YourTable", connection))
        {
            return (int)command.ExecuteScalar();
        }
    }
    static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // Character set
        StringBuilder result = new StringBuilder();
        Random random = new Random();

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }
}

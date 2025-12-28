using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Dapper;

public class Reproduce
{
    private static bool _initialized = false;

    public static async Task Main()
    {
        Console.WriteLine("First call:");
        await GetToDoItemsAsync();
        
        Console.WriteLine("\nSecond call:");
        await GetToDoItemsAsync();
    }

    public static async Task GetToDoItemsAsync()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await EnsureInitializedAsync(connection);
        
        try 
        {
            var result = await connection.QueryAsync("SELECT * FROM ToDoItem");
            Console.WriteLine("Success!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }
    }

    private static async Task EnsureInitializedAsync(SqliteConnection connection)
    {
        if (_initialized) return;

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS ToDoItem (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT
            );
            """);
        
        Console.WriteLine("Tables created.");
        _initialized = true;
    }
}
Reproduce.Main().Wait();

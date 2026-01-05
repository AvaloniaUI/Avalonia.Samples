using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using AdvancedToDoList.Models;
using AdvancedToDoList.Services;
using Avalonia.Controls;
using Dapper;
using Microsoft.Data.Sqlite;

namespace AdvancedToDoList.Helper;

public static class DataBaseHelper
{
    static DataBaseHelper()
    {
        // Register a type handler to map between SQLite INTEGER (Int64) and our Priority enum
        SqlMapper.AddTypeHandler<Priority>(new PriorityTypeHandler());
    }

    private static bool _initialized;

    internal static async Task<SqliteConnection> GetOpenConnection()
    {
        if (Design.IsDesignMode && App.DbService == null)
        {
            App.RegisterDbService(new DesignDbService());
        }

        if (App.DbService == null)
            throw new InvalidOperationException("Database service not registered.");

        var dbPath = App.DbService.GetDatabasePath();
        var dbSource = $"Data Source='{dbPath}'";

        // Ensure the directory exists for the database file
        string? dir = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var connection = new SqliteConnection(dbSource);
        await connection.OpenAsync();

        await EnsureInitializedAsync(connection);
        Console.WriteLine($"Opened database at {dbPath}");
        
        return connection;
    }

    private static async Task EnsureInitializedAsync(SqliteConnection connection)
    {
        if (_initialized) return;

        // Create tables if they do not exist
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS ToDoItem (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CategoryId INTEGER NULL,
                Title TEXT NULL,
                Priority INTEGER NOT NULL,
                Description TEXT NULL,
                DueDate TEXT NOT NULL,
                Progress INTEGER NOT NULL,
                CreatedDate TEXT NOT NULL,
                CompletedDate TEXT NULL
            );
            """);

        Console.WriteLine("Created ToDoItem table.");

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Category (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NULL,
                Color TEXT NULL
            )
            """);

        Console.WriteLine("Created Category table.");

        if (Design.IsDesignMode)
        {
            var categoryCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Category");
            if (categoryCount == 0)
            {
                await AddSampleDataAsync(connection);
                Console.WriteLine($"Added sample data to database.");
            }
        }
        
        // If we have a connection, the DbService is known to be created. Thus, we can safely surpress the null warning here. 
        // For in memory DataSource, we cannot set the _init flag to true. 
        _initialized = App.DbService!.GetDatabasePath() != ":memory:";
    }

    internal static async Task UpdateIndexedDbAsync()
    {
        if (App.DbService == null) 
            return;
        await App.DbService.SaveAsync();
    }
    
    public static async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        await using var connection = await GetOpenConnection();
        return (await connection.QueryAsync<Category>("SELECT * FROM Category"));
    }

    public static async Task<IEnumerable<ToDoItem>> GetToDoItemsAsync(bool loadAlsoCompletedItems = false)
    {
        Console.WriteLine("GetToDoItemsAsync");
        await using var connection = await GetOpenConnection();
        const string sql = """
                           SELECT t.*, c.*
                           FROM ToDoItem t
                           LEFT JOIN Category c ON t.CategoryId = c.Id
                           WHERE t.Progress < 100 OR @loadAlsoCompletedItems;
                           """;

        return await connection.QueryAsync<ToDoItem, Category, ToDoItem>(
            sql,
            (toDoItem, category) =>
            {
                toDoItem.Category = category;
                return toDoItem;
            },
            splitOn: "Id", 
            param: new {loadAlsoCompletedItems});
    }

    private static async Task AddSampleDataAsync(SqliteConnection connection)
    {
        await connection.ExecuteAsync(
            """
            INSERT INTO Category (Id, Name, Description, Color) VALUES 
            (null, 'Category 1', 'Description 1', '#FF0000'), 
            (null, 'Category 2', 'Description 2', '#00FF00');

            INSERT INTO ToDoItem (Id, CategoryId, Title, Priority, Description, DueDate, Progress, CreatedDate, CompletedDate) VALUES 
            (null, 1, 'Item 1', 1, 'Description 1', '2026-01-01', 0, '2026-01-01', null),
            (null, 2, 'Item 2', 2, 'Description 2', '2026-01-02', 0, '2026-01-02', null),
            (null, 5, 'Item 3', 2, 'A completed item', '2026-01-02', 100, '2026-01-02', null);
            """
        );
    }
}

public class PriorityTypeHandler : SqlMapper.TypeHandler<Priority>
{
    public override void SetValue(IDbDataParameter parameter, Priority value)
    {
        // Store enum as integer value in SQLite
        parameter.Value = (int)value;
    }

    public override Priority Parse(object value)
    {
        // SQLite returns INTEGER as Int64 by default; handle common representations robustly
        return value switch
        {
            null => default,
            DBNull => default,
            long l => (Priority)(int)l,
            int i => (Priority)i,
            short s => (Priority)s,
            byte b => (Priority)b,
            string str when int.TryParse(str, out var parsed) => (Priority)parsed,
            _ => default
        };
    }
}
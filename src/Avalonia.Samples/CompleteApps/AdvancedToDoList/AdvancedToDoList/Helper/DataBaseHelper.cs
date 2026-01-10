
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AdvancedToDoList.Models;
using AdvancedToDoList.Services;
using Avalonia.Controls;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

[module: DapperAot]

namespace AdvancedToDoList.Helper;

public static class DataBaseHelper
{
    private static bool _initialized;
    
    internal static async Task<SqliteConnection> GetOpenConnection()
    {
        if (Design.IsDesignMode)
        {
            Console.WriteLine(App.Services.GetService<IDbService>() is not null ? "Service found" : "Service not found");
        }

        var dbService = App.Services.GetRequiredService<IDbService>();

        var dbPath = dbService.GetDatabasePath();
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
                           SELECT *
                           FROM ToDoItem 
                           WHERE Progress < 100 OR @loadAlsoCompletedItems;
                           """;

        var toDoItems = 
            (await connection.QueryAsync<ToDoItem>(sql,
            new {loadAlsoCompletedItems}))
            .ToArray();
        
        var categories = await connection.QueryAsync<Category>("SELECT * FROM Category");
        var categoriesDict = new Dictionary<long, Category>(
            categories.Select(x => new KeyValuePair<long, Category>(x.Id ?? -1, x)));
        
        foreach (var item in toDoItems)
        {
            item.Category = categoriesDict.GetValueOrDefault(item.CategoryId ?? -1);
        }
        
        return toDoItems;
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
        _initialized = App.Services.GetRequiredService<IDbService>().GetDatabasePath() != ":memory:";
    }

        
    /// <summary>
    /// Syncs the database with the underlying data store if needed.
    /// </summary>
    /// <remarks>
    /// Wasm uses IndexedDB to store the data. This needs to be synced.
    /// This helper will do it for us.
    /// </remarks>
    public static async Task SyncUnderlyingDatabaseAsync()
    {
        await App.Services.GetRequiredService<IDbService>().SaveAsync();
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
    
    
    /// <summary>
    /// Returns a JSON representation of the entire database.
    /// </summary>
    /// <param name="targetStream">The target Stream to save to</param>
    public static async Task ExportToJsonAsync(Stream targetStream)
    {
        var dto = new DataBaseDto()
        {
            Categories = (await DataBaseHelper.GetCategoriesAsync()).ToArray(),
            ToDoItems = (await DataBaseHelper.GetToDoItemsAsync(true)).ToArray()
        };
        
        await JsonSerializer.SerializeAsync(targetStream, dto, JsonContextHelper.Default.DataBaseDto);
    }
}

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
    
    internal static async Task<SqliteConnection> GetOpenConnectionAsync()
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
        await using var connection = await GetOpenConnectionAsync();
        return (await connection.QueryAsync<Category>("SELECT * FROM Category"));
    }

    public static async Task<IEnumerable<ToDoItem>> GetToDoItemsAsync(bool loadAlsoCompletedItems = false)
    {
        Console.WriteLine("GetToDoItemsAsync");
        await using var connection = await GetOpenConnectionAsync();
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
    
    /// <summary>
    /// Ensures that all tables are created and the database is ready to be used.
    /// </summary>
    /// <param name="connection">the connection to use</param>
    /// <param name="force">the creating will be skipped by default, unless you set this parameter to true</param>
    internal static async Task EnsureInitializedAsync(SqliteConnection connection, bool force = false)
    {
        if (_initialized && !force) return;

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

        Category[] testCategories =
        [
            new Category()
            {
                Name = "Category 1",
                Color = "Red", // Color by name
                Description = "This is Category 1"
            },
            new Category()
            {
                Name = "Category 2",
                Color = "#FFABCDEF", // Color by Hex
                Description = "This is Category 2"
            },
            new Category()
            {
                Name = "Category 3",
                Color = "#ThisIsNoColor", // testing ivnalid color name
                Description = "This is Category 3"
            }
        ];

        ToDoItem[] TestToDoItems =
        [
            new ToDoItem()
            {
                CategoryId = 1,
                Title = "Test Item 1",
                Priority = 1,
                Description = "This is Test Item 1",
                DueDate = DateTime.Today.AddDays(3),
                CompletedDate = DateTime.Today,
                Progress = 50
            },
            new ToDoItem()
            {
                CategoryId = 2,
                Title = "Test Item 2",
                Priority = 2,
                Description = "This is Test Item 2",
                DueDate = DateTime.Today.AddDays(-1),
                CreatedDate =  DateTime.Today,
                Progress = -1
            },
            new ToDoItem()
            {
                CategoryId = 0,
                Title = "Test Item 3",
                Priority = 3,
                Description = "This is Test Item 3",
                DueDate = DateTime.Today.AddDays(-1),
                CompletedDate = DateTime.Today,
                Progress = 100,
                CreatedDate =  DateTime.Today.AddDays(-2),
            }
        ];
        
        await connection.ExecuteAsync(
            """
            REPLACE INTO Category (Id, Name, Description, Color) VALUES 
            (@Id, @Name, @Description, @Color);
            """, testCategories);
        
        await connection.ExecuteAsync(
            """
            REPLACE INTO ToDoItem (Id, CategoryId, Title, Priority, Description, DueDate, Progress, CreatedDate, CompletedDate) VALUES 
            (@Id, @CategoryId, @Title, @Priority, @Description, @DueDate, @Progress, @CreatedDate, @CompletedDate);
            """, TestToDoItems);
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
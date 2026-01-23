
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

// Needed to make Dapper AOT-friendly.
// See: https://aot.dapperlib.dev for usage details
[module: DapperAot]

namespace AdvancedToDoList.Helper;

/// <summary>
/// This is a helper class for working with our DataBase.
/// </summary>
public static class DataBaseHelper
{
    // A flag that indicates if the DB is yet initialized.
    private static bool _initialized;
    
    /// <summary>
    /// Opens a new <see cref="SqliteConnection"/> and opens it for usage. 
    /// </summary>
    /// <remarks>
    /// Ensure the connection is disposed of after use.
    /// </remarks>
    /// <returns>The open connection.</returns>
    internal static async Task<SqliteConnection> GetOpenConnectionAsync()
    {
        // Get the DB-service to resolve the DB per platfrom correctly.
        var dbService = App.Services.GetRequiredService<IDbService>();
        
        var dbPath = dbService.GetDatabasePath();
        var dbSource = $"Data Source='{dbPath}'";

        // Ensure the directory exists for the database file
        var dir = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var connection = new SqliteConnection(dbSource);
        await connection.OpenAsync();

        // make sure the necessary DB-schema is created.
        await EnsureInitializedAsync(connection);
        
        return connection;
    }

    /// <summary>
    /// Gets all available Categories from the DB.
    /// </summary>
    /// <returns>the loaded Categories</returns>
    public static async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        await using var connection = await GetOpenConnectionAsync();
        return (await connection.QueryAsync<Category>("SELECT * FROM Category"));
    }

    /// <summary>
    /// Gets all available ToDoItems from the DB, filtered by its status.
    /// </summary>
    /// <param name="loadAlsoCompletedItems">If true, also loads items that are marked as compleded (Progess = 100 %). The default is false.</param>
    /// <returns>the loaded ToDoItems</returns>
    public static async Task<IEnumerable<ToDoItem>> GetToDoItemsAsync(bool loadAlsoCompletedItems = false)
    {
        await using var connection = await GetOpenConnectionAsync();
        // The trick here is to pass @loadAlsoCompletedItems as a parameter.
        // If it is true, the condition will always be true. 
        // The alternative would be to write different SQL queries.
        const string sql = """
                           SELECT *
                           FROM ToDoItem 
                           WHERE @loadAlsoCompletedItems OR Progress < 100;
                           """;

        // store the items into an array
        var toDoItems = 
            (await connection.QueryAsync<ToDoItem>(sql,
            new {loadAlsoCompletedItems}))
            .ToArray();
        
        // map the categories.
        // Dapper could also to it directly in the query.
        // However, this would need reflection and NativeAOT would not work properly. 
        // Thus, we will map the categories to CategoryId on our own. 
        
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
    /// <param name="force">the creating will be skipped by default, unless you set this parameter to true.</param>
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

        // Populate some data for the designer if it has none yet. 
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
    /// This helper will do it for us. If you don't need the WASM-target, this can be omitted. 
    /// </remarks>
    public static async Task SyncUnderlyingDatabaseAsync()
    {
        await App.Services.GetRequiredService<IDbService>().SaveAsync();
    }

    /// <summary>
    /// Adds some sample data for the designer.
    /// </summary>
    /// <param name="connection">The connection to use.</param>
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
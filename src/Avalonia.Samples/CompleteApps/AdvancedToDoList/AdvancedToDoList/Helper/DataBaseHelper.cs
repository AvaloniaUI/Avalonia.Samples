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

        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS Category (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NULL,
                GroupColorHex TEXT NULL
            )
            """);

        _initialized = true;
    }

    public static async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        await using var connection = await GetOpenConnection();
        return (await connection.QueryAsync<Category>("SELECT * FROM Category"));
    }
    
    public static async Task<IEnumerable<ToDoItem>> GetToDoItemsAsync()
    {
        await using var connection = await GetOpenConnection();
        return (await connection.QueryAsync<ToDoItem>("SELECT * FROM ToDoItem"));
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
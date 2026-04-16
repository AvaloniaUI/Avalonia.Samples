using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Dapper;

namespace AdvancedToDoList.Models;

/// <summary>
/// The ToDoItem-Model
/// </summary>
public record ToDoItem
{
    /// <summary>
    /// Gets or sets the ID of the ToDoItem.
    /// </summary>
    public long? Id { get; set; }
    
    /// <summary>
    /// Gets or sets the Category of the ToDoItem.
    /// </summary>
    /// <remarks>
    /// This property is ignored during I/O operations. Use <see cref="CategoryId"/> instead.
    /// </remarks>
    [JsonIgnore]
    public Category? Category { get; set; }

    /// <summary>
    /// Foreign key to the related Category (nullable).
    /// </summary>
    public long? CategoryId { get; set; }
    
    /// <summary>
    /// Gets or sets the Title of the ToDoItem.
    /// </summary>
    /// <remarks>This property is required.</remarks>
    public string? Title { get; set; }
    
    /// <summary>
    /// Gets or sets the Priority of the ToDoItem. The default value is <see cref="Priority.Medium"/>.
    /// </summary>
    /// <remarks>We cast this to int to avoid reflection during I/O operations.</remarks>
    public int Priority { get; set; } = (int)Models.Priority.Medium;
    
    /// <summary>
    /// Gets or sets the Description of the ToDoItem. 
    /// </summary>
    /// <remarks>This property is optional.</remarks>
    public string? Description { get; set; } 
    
    /// <summary>
    /// Gets or sets the DueDate of the ToDoItem. The default value is 7 days from now.
    /// </summary>
    /// <remarks>This property is required</remarks>
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    
    /// <summary>
    /// Gets or sets the Progress of the ToDoItem. The default value is 0.
    /// </summary>
    public int Progress { get; set; }
    
    /// <summary>
    /// Gets or sets the CreatedDate of the ToDoItem. The default value is <see cref="DateTime.Now"/>.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Gets or sets the CompletedDate of the ToDoItem. As long as this property is null, the ToDoItem is not completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }
    
    /// <summary>
    /// This method tries to save this item into the DB.
    /// </summary>
    /// <returns>true if save was successful, otherwise false</returns>
    public async Task<bool> SaveAsync()
    {
        try
        {
            await using var connection = await DatabaseHelper.GetOpenConnectionAsync();
            
            // We are using SQLite. This allows us to use the "REPLACE INTO" statement, which will 
            // update an existing record if found, otherwise it will create a new record.
            // The function "Last_insert_rowid()" will return the ID of the record we saved. 
            Id = await connection.ExecuteScalarAsync<long?>(
                """
                REPLACE INTO ToDoItem (Id, CategoryId, Title, Priority, Description, DueDate, Progress, CreatedDate, CompletedDate)
                        VALUES (@Id, @CategoryId, @Title, @Priority, @Description, @DueDate, @Progress, @CreatedDate, @CompletedDate);
                SELECT Last_insert_rowid();
                """, this //{ Id, CategoryId, Title, Priority, Description, DueDate, Progress, CreatedDate, CompletedDate }
            );
            
            // Remember to sync the Indexed-DB.
            await DatabaseHelper.SyncUnderlyingDatabaseAsync();
            
            return Id != null;
        }
        catch (Exception e)
        {
            // We send exceptions to the Trace listener, this helps us to debug the App.
            Trace.TraceError(e.Message);
            return false;
        }
    }
    
    /// <summary>
    /// This method tries to remove this item from the DB.
    /// </summary>
    /// <returns>true if deletion was successful, otherwise false</returns>
    public async Task<bool> DeleteAsync()
    {
        try
        {
            // If the ID isn't set, we cannot delete anything.
            if (Id == null)
                return false;
            
            await using var connection = await DatabaseHelper.GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                """
                DELETE FROM ToDoItem WHERE Id = @Id;
                """, this
            );
            
            // Remember to sync the Indexed-DB.
            await DatabaseHelper.SyncUnderlyingDatabaseAsync();
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}
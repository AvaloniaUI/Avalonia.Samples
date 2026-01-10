using System;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Dapper;

namespace AdvancedToDoList.Models;

public record ToDoItem
{
    /// <summary>
    /// Gets or sets the Id of the ToDoItem.
    /// </summary>
    public long? Id { get; set; }
    
    /// <summary>
    /// Gets or sets the Category of the ToDoItem.
    /// </summary>
    [JsonIgnore]
    public Category? Category { get; set; }

    /// <summary>
    /// Foreign key to the related Category (nullable).
    /// </summary>
    public long? CategoryId { get; set; }
    
    /// <summary>
    /// Gets or sets the Title of the ToDoItem.
    /// This item is required.
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// Gets or sets the Priority of the ToDoItem. The default value is Medium.
    /// </summary>
    public int Priority { get; set; } = (int)Models.Priority.Medium;
    
    /// <summary>
    /// Gets or sets the Description of the ToDoItem. This property is optional.
    /// </summary>
    public string? Description { get; set; } 
    
    /// <summary>
    /// Gets or sets the DueDate of the ToDoItem. The default value is 7 days from now.
    /// </summary>
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(7);
    
    /// <summary>
    /// Gets or sets the Progress of the ToDoItem. The default value is 0.
    /// </summary>
    public int Progress { get; set; }
    
    /// <summary>
    /// Gets or sets the CreatedDate of the ToDoItem. The default value is now.
    /// </summary>
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    
    /// <summary>
    /// Gets or sets the CompletedDate of the ToDoItem. As long as this property is null, the ToDoItem is not completed.
    /// </summary>
    public DateTime? CompletedDate { get; set; }
    
    public async Task<bool> SaveAsync()
    {
        try
        {
            await using var connection = await DataBaseHelper.GetOpenConnection();
            Id = await connection.ExecuteScalarAsync<long?>(
                """
                REPLACE INTO ToDoItem (Id, CategoryId, Title, Priority, Description, DueDate, Progress, CreatedDate, CompletedDate)
                        VALUES (@Id, @CategoryId, @Title, @Priority, @Description, @DueDate, @Progress, @CreatedDate, @CompletedDate);
                SELECT Last_insert_rowid();
                """, this //{ Id, CategoryId, Title, Priority, Description, DueDate, Progress, CreatedDate, CompletedDate }
            );
            
            await DataBaseHelper.SyncUnderlyingDatabaseAsync();
            
            return Id != null;
        }
        catch (Exception e)
        {
            Trace.TraceError(e.Message);
            return false;
        }
    }

    public async Task<bool> DeleteAsync()
    {
        try
        {
            if (Id == null)
                return false;
            
            await using var connection = await DataBaseHelper.GetOpenConnection();
            await connection.ExecuteAsync(
                """
                DELETE FROM ToDoItem WHERE Id = @Id;
                """, this
            );

            await DataBaseHelper.SyncUnderlyingDatabaseAsync();
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}
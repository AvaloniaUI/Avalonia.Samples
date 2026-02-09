using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Dapper;

namespace AdvancedToDoList.Models;

/// <summary>
/// Represents a category for organizing ToDo items.
/// Uses record type for value-based equality and immutable design principles.
/// Categories provide color coding and organizational structure to ToDo items.
/// </summary>
public record Category
{
    /// <summary>
    /// Gets or sets the unique identifier for the category.
    /// Null for new categories that haven't been saved to the database yet.
    /// </summary>
    public long? Id { get; set; }
    
    /// <summary>
    /// Gets or sets the display name of the category.
    /// This property is required and must be provided for valid categories.
    /// </summary>
    /// <remarks>This property is required</remarks>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the optional description of the category.
    /// Provides additional context about the category's purpose or scope.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the color of the category in "#AARRGGBB" hexadecimal format.
    /// Used for visual distinction in the UI to help users quickly identify categories.
    /// The format includes alpha (transparency) and RGB components.
    /// </summary>
    /// <example>#FFFF0000 => (A=255, R=255, G=0, B=0) => Red</example>
    public string? Color { get; set; }

    /// <summary>
    /// Asynchronously saves this category to the database.
    /// Uses SQLite's REPLACE INTO statement to handle both insert and update operations.
    /// Updates the Id property with the database-assigned identifier for new records.
    /// </summary>
    /// <returns>true if save was successful, otherwise false</returns>
    public async Task<bool> SaveAsync()
    {
        try
        {
            await using var connection = await DatabaseHelper.GetOpenConnectionAsync();
            
            // SQLite REPLACE INTO statement handles both insert and update operations:
            // - If Id is null: creates new record and returns the new ID
            // - If Id exists: updates existing record and returns the same ID
            // Last_insert_rowid() returns the ID of the saved record
            Id = await connection.ExecuteScalarAsync<long?>(
                """
                REPLACE INTO Category (Id, Name, Description, Color)
                        VALUES (@Id, @Name, @Description, @Color);
                SELECT Last_insert_rowid();
                """, this
            );
            
            // Sync with Indexed-DB for browser platform compatibility
            await DatabaseHelper.SyncUnderlyingDatabaseAsync();
            
            return Id != null;
        }
        catch
        {
            // In production, consider logging the exception for debugging
            return false;
        }
    }

    /// <summary>
    /// Asynchronously deletes this category from the database.
    /// Only works if the category has been previously saved (has a valid Id).
    /// Note: Deleting a category that's in use by ToDo items may cause orphaned references.
    /// </summary>
    /// <returns>true if deletion was successful, otherwise false</returns>
    public async Task<bool> DeleteAsync()
    {
        try
        {
            // Cannot delete if the category hasn't been saved to the database yet
            if (Id == null)
            {
                return false;
            }
            
            await using var connection = await DatabaseHelper.GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                """
                DELETE FROM Category WHERE Id = @Id;
                """, this
            );
            
            // Sync changes with Indexed-DB for browser platform compatibility
            await DatabaseHelper.SyncUnderlyingDatabaseAsync();
            
            return true;
        }
        catch
        {
            // In production, consider logging the exception for debugging purposes
            return false;
        }
    }
}
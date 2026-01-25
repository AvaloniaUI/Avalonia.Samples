using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Dapper;

namespace AdvancedToDoList.Models;

/// <summary>
/// The Category-Model
/// </summary>
public record Category
{
    /// <summary>
    /// Gets or sets the ID of the category.
    /// </summary>
    public long? Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    /// <remarks>This property is required</remarks>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the category.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the color of the category in "#AARRGGBB" format.
    /// </summary>
    /// <example>#FFFF0000 => (A=255, R=255, G=0, B=0) => Red</example>
    public string? Color { get; set; }

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
                REPLACE INTO Category (Id, Name, Description, Color)
                        VALUES (@Id, @Name, @Description, @Color);
                SELECT Last_insert_rowid();
                """, this
            );
            
            // Remember to update the Indexed-DB if (Browser-target only)
            await DatabaseHelper.SyncUnderlyingDatabaseAsync();
            
            return Id != null;
        }
        catch
        {
            // In production, one probably wants to log the exception. 
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
            {
                return false;
            }
            
            await using var connection = await DatabaseHelper.GetOpenConnectionAsync();
            await connection.ExecuteAsync(
                """
                DELETE FROM Category WHERE Id = @Id;
                """, this
            );
            
            // Remember to sync the Indexed-DB.
            await DatabaseHelper.SyncUnderlyingDatabaseAsync();
            
            return true;
        }
        catch
        {
            // In production, one probably wants to log the exception.
            return false;
        }
    }
}
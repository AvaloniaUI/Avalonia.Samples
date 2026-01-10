using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Dapper;

namespace AdvancedToDoList.Models;

public record Category
{
    /// <summary>
    /// Gets or sets the Id of the category.
    /// </summary>
    public long? Id { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the category.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the color of the category in hex format.
    /// </summary>
    public string? Color { get; set; }

    public async Task<bool> SaveAsync()
    {
        try
        {
            await using var connection = await DataBaseHelper.GetOpenConnection();
            Id = await connection.ExecuteScalarAsync<long?>(
                """
                REPLACE INTO Category (Id, Name, Description, Color)
                        VALUES (@Id, @Name, @Description, @Color);
                SELECT Last_insert_rowid();
                """, this
            );
            
            await DataBaseHelper.SyncUnderlyingDatabaseAsync();
            
            return Id != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync()
    {
        try
        {
            if (Id == null)
            {
                return false;
            }
            
            await using var connection = await DataBaseHelper.GetOpenConnection();
            await connection.ExecuteAsync(
                """
                DELETE FROM Category WHERE Id = @Id;
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
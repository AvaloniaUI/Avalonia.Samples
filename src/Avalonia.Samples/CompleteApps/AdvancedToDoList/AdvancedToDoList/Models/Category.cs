using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Dapper;

namespace AdvancedToDoList.Models;

public record Category
{
    /// <summary>
    /// Gets or sets the Id of the category.
    /// </summary>
    public int? Id { get; set; }
    
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
    public string? GroupColorHex { get; set; }
    
    public async Task<bool> SaveAsync()
    {
        return true;
        await using var connection = await DataBaseHelper.GetOpenConnection();
        connection.ExecuteScalarAsync(
            """
            REPLACE INTO Category 
            """
            );
    }
}
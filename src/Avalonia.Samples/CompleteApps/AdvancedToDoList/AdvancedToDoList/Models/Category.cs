using System;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using Avalonia.Media;
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

    public Color? GroupColor => Color.TryParse(GroupColorHex, out var color) ? color : null;

    public async Task<bool> SaveAsync()
    {
        try
        {
            await using var connection = await DataBaseHelper.GetOpenConnection();
            Id = await connection.ExecuteScalarAsync<int?>(
                """
                REPLACE INTO Category (Id, Name, Description, GroupColorHex)
                        VALUES (@Id, @Name, @Description, @GroupColorHex);
                SELECT Last_insert_rowid();
                """, this
            );

            return Id != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync()
    {
        if (Id == null)
        {
            return false;
        }
        try
        {
            await using var connection = await DataBaseHelper.GetOpenConnection();
            await connection.ExecuteAsync(
                """
                DELETE FROM Category WHERE Id = @Id;
                """, this
            );

            return true;
        }
        catch
        {
            return false;
        }
    }
}
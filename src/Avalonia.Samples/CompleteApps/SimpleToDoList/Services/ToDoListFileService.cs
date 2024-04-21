using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleToDoList.Models;

namespace SimpleToDoList.Services;

/// <summary>
/// This class provides the needed functions to save and restore a ToDoList. This step is fully optional for this tutorial
/// </summary>
public static class ToDoListFileService
{
    // This is a hard coded path to the file. It may not be available on every platform. In your real world App you may 
    // want to make this configurable
    private static string _jsonFileName = 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Avalonia.SimpleToDoList", "MyToDoList.txt");

    /// <summary>
    /// Stores the given items into a file on disc
    /// </summary>
    /// <param name="itemsToSave">The items to save</param>
    public static async Task SaveToFileAsync(IEnumerable<ToDoItem> itemsToSave)
    {
        // Ensure all directories exists
        Directory.CreateDirectory(Path.GetDirectoryName(_jsonFileName)!);
        
        // We use a FileStream to write all items to disc
        using (var fs = File.Create(_jsonFileName))
        {
            await JsonSerializer.SerializeAsync(fs, itemsToSave);
        }
    }

    /// <summary>
    /// Loads the file from disc and returns the items stored inside
    /// </summary>
    /// <returns>An IEnumerable of items loaded or null in case the file was not found</returns>
    public static async Task<IEnumerable<ToDoItem>?> LoadFromFileAsync()
    {
        try
        {
            // We try to read the saved file and return the ToDoItemsList if successful
            using (var fs = File.OpenRead(_jsonFileName))
            {
                return await JsonSerializer.DeserializeAsync<IEnumerable<ToDoItem>>(fs);
            }
        }
        catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
        {
            // In case the file was not found, we simply return null
            return null;
        }
    }
}
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Messages;
using AdvancedToDoList.Models;
using AdvancedToDoList.Properties;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Dapper;
using SharedControls.Controls;
using SharedControls.Services;

namespace AdvancedToDoList.ViewModels;

public partial class SettingsViewModel : ViewModelBase, IDialogParticipant
{
    public Settings Settings => Settings.Default;

    public string[] AvailableThemeVariants { get; } =
    [
        ThemeVariant.Default.ToString(),
        ThemeVariant.Dark.ToString(),
        ThemeVariant.Light.ToString()
    ];

    /// <summary>
    /// A command that will export the entire database as Json-File
    /// </summary>
    [RelayCommand]
    private async Task ExportDataAsync()
    {
        try
        {
            var safeFilePickerResult = await this.SafeFileDialogAsync(
                "Export Data", 
                [FileHelper.JsonFileType]);

            if (safeFilePickerResult?.File is {  } storageFile)
            {
                await using var fs = await storageFile.OpenWriteAsync();

                try
                {
                    await DataBaseHelper.ExportToJsonAsync(fs);
                }
                catch (Exception e)
                {
                    await this.ShowOverlayDialogAsync<DialogResult>(
                        "Error",
                        "An error occured during exporting data. " + e.Message,
                        DialogCommands.OkOnly);
                }
            
                await this.ShowOverlayDialogAsync<DialogResult>(
                    "Exported Data",
                    $"Successfully exported data to '{storageFile.Name}'.",
                    DialogCommands.Ok);
            }
        }
        catch (Exception e)
        {
            await this.ShowOverlayDialogAsync<DialogResult>("Could not export the data", 
                e.Message, DialogCommands.OkOnly);
        }
    }
    
    /// <summary>
    /// A command that will import the selected json file into the current database.
    /// Existing items will be updated. 
    /// </summary>
    [RelayCommand]
    private async Task ImportDataAsync()
    {
        try
        {
            // NOTE: Existing items will be updated / overridden. You may want to let the user choose 
            // how to handle it. 
            
            var openFilePickerResult = await this.OpenFileDialogAsync(
                "Import Data",
                [FileHelper.JsonFileType]);

            if (openFilePickerResult?.FirstOrDefault() is { } storageFile)
            {
                await using var fs = await storageFile.OpenReadAsync();
            
                var dto = await JsonSerializer.DeserializeAsync<DataBaseDto>(fs, 
                    JsonContextHelper.Default.DataBaseDto);

                if (dto is null)
                {
                    throw new FileLoadException("Could not load data");
                }
            
                foreach (var category in dto.Categories ?? [])
                {
                    await category.SaveAsync();
                }
            
                foreach (var toDoItem in dto.ToDoItems ?? [])
                {
                    await toDoItem.SaveAsync();
                }

                // Notify other ViewModels about updated DB. 
                WeakReferenceMessenger.Default.Send(new UpdateDataRequest<ToDoItem>());
                WeakReferenceMessenger.Default.Send(new UpdateDataRequest<Category>());
            }
        }
        catch (Exception e)
        {
            await this.ShowOverlayDialogAsync<DialogResult>("Error importing JSON file",
                e.Message, DialogCommands.OkOnly);
        }
    }

    [RelayCommand]
    private async Task ClearDatabaseAsync()
    {
        var choice = await this.ShowOverlayDialogAsync<DialogResult>(
            "Clear Database",
            """
            Are you sure you want to clear the database? This cannot be undone.
            TIP: Consider to export the data before you continue.
            
            Press "Yes" to continue.
            """,
            DialogCommands.YesNo);

        if (choice == DialogResult.Yes)
        {
            await using var connection = await DataBaseHelper.GetOpenConnectionAsync();

            await connection.ExecuteAsync(
                """
                DROP TABLE IF EXISTS Category;
                DROP TABLE IF EXISTS ToDoItem;
                
                VACUUM;
                """);
            
            await DataBaseHelper.EnsureInitializedAsync(connection, true);
            
            // Notify other ViewModels about updated DB. 
            WeakReferenceMessenger.Default.Send(new UpdateDataRequest<ToDoItem>());
            WeakReferenceMessenger.Default.Send(new UpdateDataRequest<Category>());
        }
    }
    
}
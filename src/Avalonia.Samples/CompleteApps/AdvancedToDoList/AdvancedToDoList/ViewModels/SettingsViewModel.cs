using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Properties;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
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
    
    [RelayCommand]
    private async Task ImportDataAsync()
    {
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
            // ToDo need to save the items to the db and requery.
        }
    }
}
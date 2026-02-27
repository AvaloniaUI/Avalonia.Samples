using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AdvancedToDoList.Helper;
using AdvancedToDoList.Services;
using Avalonia.Media;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedToDoList.Properties;

/// <summary>
/// This class defines the App settings. It implements INotifyPropertyChanged so that the App can
/// subscribe to settings changes.
/// </summary>
public class Settings : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the default settings instance.
    /// </summary>
    public static Settings Default { get; } = new Settings();

    /// <summary>
    /// Gets or sets the AppTheme where allowed values are: <br/>
    /// <see cref="ThemeVariant.Light" />, <see cref="ThemeVariant.Dark"/> and <see cref="ThemeVariant.Default"/> <br/>
    /// We use the Light-theme as the default value.
    /// </summary>
    /// <remarks>
    /// ThemeVariant.Default will try to inherit the systems theme. <seealso href="https://docs.avaloniaui.net/docs/guides/styles-and-resources/how-to-use-theme-variants"/>
    /// </remarks>
    public string AppTheme
    {
        get;
        set => SetField(ref field, value);
    } = ThemeVariant.Light.ToString();

    /// <summary>
    /// Gets or sets the accent color to use. The default is Avalonia-blue.
    /// </summary>
    /// <remarks>
    /// Since we want to reduce reflection usage as much as possible, we use a <see cref="JsonConverter"/>
    /// to manage the parsing.
    /// </remarks>
    [JsonConverter(typeof(JsonColorConverter))]
    public Color AccentColor
    {
        get;
        set => SetField(ref field, value);
    } = new Color(0xFF, 0x35, 0x78, 0xE5); // "#FF3578E5", Avalonia-Blue

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    // implementation for PropertyChanged
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // We save the settings automatically with each change. The alternative would be to provide a "save"-Button 
        // in the Settings-UI. 
        SaveSettingsAsync();
    }

    // implementation for PropertyChanged
    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    // this flag is used to skip saving if that causes conflict, especially during loading. 
    private bool _canSave;

    /// <summary>
    /// This method will save the settings using <see cref="ISettingsStorageService"/>
    /// </summary>
    private async void SaveSettingsAsync()
    {
        try
        {
            if (!_canSave)
                return;
            
            _canSave = false;
            var settingsStorageService = App.Services.GetRequiredService<ISettingsStorageService>();
            var json = JsonSerializer.Serialize(this, JsonContextHelper.Default.Settings);
            await settingsStorageService.WriteAsync(json);
        }
        catch (Exception exception)
        {
            Trace.WriteLine(exception);
            // We can safely ignore failed settings changes. In this case the user will see the default settings at next app launch.
        }
        finally
        {
            // remember to enable _canSave again
            _canSave = true;
        }
    }

    /// <summary>
    /// This method will load the settings using <see cref="ISettingsStorageService"/>
    /// </summary>
    internal async Task LoadSettingsAsync()
    {
        try
        {
            // Remember to disable _canSave before reading the settings.
            _canSave = false;
            
            var settingsStorageService = App.Services.GetRequiredService<ISettingsStorageService>();
            var json = await settingsStorageService.ReadAsync();

            var settings =
                JsonSerializer.Deserialize<Settings>(json ?? string.Empty, JsonContextHelper.Default.Settings);

            if (settings != null)
            {
                Default.AccentColor = settings.AccentColor;
                Default.AppTheme = settings.AppTheme;
            }
        }
        catch (Exception exception)
        {
            // Log any exception to trace or any logger you prefer.
            Trace.TraceError(exception.Message);
            // We can safely ignore failed settings changes
        }
        finally
        {
            // Remember to enable _canSave.
            _canSave = true;
        }
    }
}
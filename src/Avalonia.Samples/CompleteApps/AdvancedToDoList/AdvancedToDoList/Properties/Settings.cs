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
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedToDoList.Properties;

public class Settings : INotifyPropertyChanged
{
    public static Settings Default { get; } = new Settings();

    public Settings()
    {
    }

    public string AppTheme
    {
        get;
        set => SetField(ref field, value);
    } = "Light";

    [JsonConverter(typeof(JsonColorConverter))]
    public Color AccentColor
    {
        get;
        set => SetField(ref field, value);
    } = new Color(0xFF, 0x35, 0x78, 0xE5); // "#FF3578E5", Avalonia Blue

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        SaveSettingsAsync();
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private bool _canSave = false;

    private async void SaveSettingsAsync()
    {
        if (!_canSave)
            return;

        try
        {
            _canSave = false;
            var settingsStorageService = App.Services.GetRequiredService<ISettingsStorageService>();
            var json = JsonSerializer.Serialize(this, JsonContextHelper.Default.Settings);
            await settingsStorageService.WriteAsync(json);
        }
        catch (Exception exception)
        {
            Trace.WriteLine(exception);
            // We can safely ignore failed settings changes
        }
        finally
        {
            _canSave = true;
        }
    }

    internal async Task LoadSettingsAsync()
    {
        try
        {
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
            Trace.TraceError(exception.Message);
            // We can safely ignore failed settings changes
        }
        finally
        {
            _canSave = true;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BattleCity.Model;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace BattleCity.Infrastructure;

public class TerrainTileConverter : IValueConverter
{
    private static Dictionary<TerrainTileType, Bitmap> _cache;
    public static TerrainTileConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return GetCache()[(TerrainTileType)value];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private Dictionary<TerrainTileType, Bitmap> GetCache()
    {
        var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();

        return _cache ??= Enum.GetValues(typeof(TerrainTileType)).OfType<TerrainTileType>().ToDictionary(
            t => t,
            t => new Bitmap(assetLoader.Open(new Uri($"avares://BattleCity/Assets/{t}.png"))));
    }
}
using System.Collections.Generic;
using Avalonia;

namespace BattleCity.Model;

public enum TerrainTileType
{
    Plain, //passable, shoot-thru
    WoodWall, //impassable, takes 1 shot to bring down
    StoneWall, //impassable, indestructible
    Water, //impassable, shoot-thru
    Pavement, //passable, 2x speed
    Forest //passable at half speed, shoot-thru
}

public class TerrainTile : GameObject
{
    private static readonly Dictionary<TerrainTileType, double> Speeds = new()
    {
        { TerrainTileType.Plain, 1 },
        { TerrainTileType.WoodWall, 0 },
        { TerrainTileType.StoneWall, 0 },
        { TerrainTileType.Water, 0 },
        { TerrainTileType.Pavement, 2 },
        { TerrainTileType.Forest, 0.5 }
    };

    private static readonly Dictionary<TerrainTileType, bool> ShootThrus = new()
    {
        { TerrainTileType.Plain, true },
        { TerrainTileType.WoodWall, false },
        { TerrainTileType.StoneWall, false },
        { TerrainTileType.Water, true },
        { TerrainTileType.Pavement, true },
        { TerrainTileType.Forest, true }
    };

    public TerrainTile(Point location, TerrainTileType type) : base(location)
    {
        Type = type;
    }


    public double Speed => Speeds[Type];
    public bool ShootThru => ShootThrus[Type];
    public bool IsPassable => Speed > 0.1;
    public TerrainTileType Type { get; set; }
}
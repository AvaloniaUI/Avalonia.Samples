using System;
using System.Collections.ObjectModel;
using Avalonia;
using BattleCity.Infrastructure;

namespace BattleCity.Model;

public class GameField : PropertyChangedBase
{
    public const double CellSize = 32;

    public GameField() : this(20, 15)
    {
    }

    public GameField(int width, int height)
    {
        Width = width;
        Height = height;
        Tiles = new TerrainTile[width, height];
        for (var x = 0; x < width; x++)
        for (var y = 0; y < height; y++)
            GameObjects.Add(
                Tiles[x, y] =
                    new TerrainTile(new Point(x * CellSize, y * CellSize), GetTypeForCoords(x, y)));
        GameObjects.Add(
            Player = new Player(this, new CellLocation(width / 2, height / 2), Facing.East));

        for (var c = 0; c < 10;)
        {
            var x = Random.Next(Width - 1);
            var y = Random.Next(Height - 1);
            if (!Tiles[x, y].IsPassable)
                continue;
            c++;
            GameObjects.Add(new Tank(this, new CellLocation(x, y), (Facing)Random.Next(4),
                Random.NextDouble() * 4 + 1));
        }
    }

    public static GameField DesignInstance { get; } = new();

    public ObservableCollection<GameObject> GameObjects { get; } = new();

    public TerrainTile[,] Tiles { get; }

    public Player Player { get; }
    public int Height { get; }
    public int Width { get; }

    private Random Random { get; } = new();

    private TerrainTileType GetTypeForCoords(int x, int y)
    {
        if (x / 2 == Width / 4)
            return TerrainTileType.Pavement;
        if (y / 2 == Height / 4) return TerrainTileType.Water;

        if (x * y == 0) return TerrainTileType.StoneWall;
        if ((x + 1 - Width) * (y + 1 - Height) == 0) return TerrainTileType.WoodWall;


        //if(Random.NextDouble()<0.1) return TerrainTileType.WoodWall;
        if (Random.NextDouble() < 0.3) return TerrainTileType.Forest;
        return TerrainTileType.Plain;
    }
}
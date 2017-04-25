using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.BattleCity.Infrastructure;

namespace Avalonia.BattleCity.Model
{
    public class GameField : PropertyChangedBase
    {

        public static GameField DesignInstance { get; } = new GameField();
        public const double CellSize = 32;

        public ObservableCollection<GameObject> GameObjects { get; } = new ObservableCollection<GameObject>();

        public TerrainTile[,] Tiles { get; }

        public Player Player { get; }
        public int Height { get; }
        public int Width{ get; }

        public GameField() : this(20, 15)
        {
            
        }

        Random Random { get; } = new Random();

        TerrainTileType GetTypeForCoords(int x, int y)
        {
            if (x / 2 == Width / 4)
                return TerrainTileType.Pavement;
            if (y/2 == Height/4)
            {

                return TerrainTileType.Water;
            }

            if (x*y == 0) return TerrainTileType.StoneWall;
            if((x+1-Width)*(y+1-Height) == 0) return TerrainTileType.WoodWall;


            //if(Random.NextDouble()<0.1) return TerrainTileType.WoodWall;
            if(Random.NextDouble()<0.3) return TerrainTileType.Forest;
            return TerrainTileType.Plain;
        }

        public GameField(int width, int height)
        {
            Width = width;
            Height = height;
            Tiles = new TerrainTile[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObjects.Add(
                        Tiles[x, y] =
                            new TerrainTile(new Point(x*CellSize, y*CellSize), GetTypeForCoords(x,y)));
                }
            }
            GameObjects.Add(
                Player = new Player(this, new CellLocation(width/2, height/2), Facing.North));

            for (var c = 0; c < 10;)
            {
                var x = Random.Next(Width - 1);
                var y = Random.Next(Height - 1);
                if (!Tiles[x, y].IsPassable)
                    continue;
                c++;
                GameObjects.Add(new Tank(this, new CellLocation(x, y), (Facing) Random.Next(4),
                    Random.NextDouble()*4 + 1));
            }
        }

    }
}
using System.Collections.Generic;
using Avalonia;
using Avalonia.BattleCity.Infrastructure;

namespace Avalonia.BattleCity.Model
{ 
    public abstract class GameObject : PropertyChangedBase
    {
        private Point _location;

        public Point Location
        {
            get { return _location; }
            protected set
            {
                if (value.Equals(_location)) return;
                _location = value;
                OnPropertyChanged();
            }
        }

        public virtual int Layer => 0;

        protected GameObject(Point location)
        {
            Location = location;
        }
    }

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
        private static readonly Dictionary<TerrainTileType, double> Speeds = new Dictionary<TerrainTileType, double>
        {
            {TerrainTileType.Plain, 1},
            {TerrainTileType.WoodWall, 0},
            {TerrainTileType.StoneWall, 0},
            {TerrainTileType.Water, 0},
            {TerrainTileType.Pavement, 2},
            {TerrainTileType.Forest, 0.5}
        };
        private static readonly Dictionary<TerrainTileType, bool> ShootThrus = new Dictionary<TerrainTileType, bool>
        {
            {TerrainTileType.Plain, true},
            {TerrainTileType.WoodWall, false},
            {TerrainTileType.StoneWall, false},
            {TerrainTileType.Water, true},
            {TerrainTileType.Pavement, true},
            {TerrainTileType.Forest, true},
        };


        public double Speed => Speeds[Type];
        public bool ShootThru => ShootThrus[Type];
        public bool IsPassable => Speed > 0.1;
        public TerrainTileType Type { get; set; }

        public TerrainTile(Point location, TerrainTileType type) : base(location)
        {
            Type = type;
        }
    }
}

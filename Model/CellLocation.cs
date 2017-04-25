using Avalonia;

namespace Avalonia.BattleCity.Model
{
    public struct CellLocation
    {
        public bool Equals(CellLocation other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is CellLocation && Equals((CellLocation) obj);
        }

        public static bool operator ==(CellLocation l1, CellLocation l2) => l1.Equals(l2);

        public static bool operator !=(CellLocation l1, CellLocation l2) => !(l1 == l2);

        public override int GetHashCode()
        {
            unchecked
            {
                return (X*397) ^ Y;
            }
        }

        public override string ToString() => $"({X}:{Y})";

        public Point ToPoint() => new Point(GameField.CellSize*X, GameField.CellSize*Y);

        public CellLocation(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get;  }

        public CellLocation WithX(int x) => new CellLocation(x, Y);
        public CellLocation WithY(int y) => new CellLocation(X, y);
    }
}
namespace Avalonia.BattleCity.Model;

public readonly struct CellLocation
{
    public bool Equals(CellLocation other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is CellLocation && Equals((CellLocation)obj);
    }

    public static bool operator ==(CellLocation l1, CellLocation l2)
    {
        return l1.Equals(l2);
    }

    public static bool operator !=(CellLocation l1, CellLocation l2)
    {
        return !(l1 == l2);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X * 397) ^ Y;
        }
    }

    public override string ToString()
    {
        return $"({X}:{Y})";
    }

    public Point ToPoint()
    {
        return new(GameField.CellSize * X, GameField.CellSize * Y);
    }

    public CellLocation(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }

    public CellLocation WithX(int x)
    {
        return new(x, Y);
    }

    public CellLocation WithY(int y)
    {
        return new(X, y);
    }
}
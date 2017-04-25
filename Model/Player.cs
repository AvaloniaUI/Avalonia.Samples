namespace Avalonia.BattleCity.Model
{
    public class Player : MovingGameObject
    {
        public Player(GameField field, CellLocation location, Facing facing) : base(field, location, facing)
        {
        }
    }
}
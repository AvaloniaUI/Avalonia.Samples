namespace Avalonia.BattleCity.Model
{
    public class Tank : MovingGameObject
    {
        private readonly double _speed;

        protected override double SpeedFactor => _speed*base.SpeedFactor;

        public Tank(GameField field, CellLocation location, Facing facing, double speed) : base(field, location, facing)
        {
            _speed = speed;
        }
    }
}
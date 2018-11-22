using System;
using System.Linq;
using Avalonia.Input;

namespace Avalonia.BattleCity.Model
{
    public class Game : GameBase
    {
        private readonly GameField _field;

        public Game(GameField field)
        {
            _field = field;
        }

        private Random Random { get; } = new Random();

        protected override void Tick()
        {
            if (!_field.Player.IsMoving)
            {
                if (Keyboard.IsKeyDown(Key.Up))
                    _field.Player.SetTarget(Facing.North);
                else if (Keyboard.IsKeyDown(Key.Down))
                    _field.Player.SetTarget(Facing.South);
                else if (Keyboard.IsKeyDown(Key.Left))
                    _field.Player.SetTarget(Facing.West);
                else if (Keyboard.IsKeyDown(Key.Right))
                    _field.Player.SetTarget(Facing.East);
            }
            foreach (var tank in _field.GameObjects.OfType<Tank>())
                if (!tank.IsMoving)
                {
                    if (!tank.SetTarget(tank.Facing))
                    {
                        if (!tank.SetTarget((Facing) Random.Next(4)))
                            tank.SetTarget(null);
                    }
                }

            foreach(var obj in _field.GameObjects.OfType<MovingGameObject>())
                obj.MoveToTarget();
        }
    }
}
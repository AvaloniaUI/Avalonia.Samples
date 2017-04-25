using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Input;

namespace Avalonia.BattleCity
{
    static class Keyboard
    {
        public static readonly HashSet<Key> Keys = new HashSet<Key>();
        public static bool IsKeyDown(Key key) => Keys.Contains(key);
        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
namespace TechnoViking.GlobalData
{
    class GlobalData
    {
        
        public static GameData GameData
        {
            set;
            get;
        }

        public static KeyboardDispatcher keyboardDispatcher
        {
            set;
            get;
        }

        public static SpriteFont Font
        {

            get;
            set;

        }

        public static void Initialize()
        {
            GameData = new GameData();
            GameData.TypeOfGame = GameData.GameType.Local;
        }
    }
}

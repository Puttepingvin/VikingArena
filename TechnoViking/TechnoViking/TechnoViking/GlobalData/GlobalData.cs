using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechnoViking.GlobalData
{
    class GlobalData
    {
        public static GameData GameData
        {
            set;
            get;
        }
        public static void Initialize()
        {
            GameData = new GameData();
            GameData.TypeOfGame = GameData.GameType.Local;
        }
    }
}

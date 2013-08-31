using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechnoViking.GlobalData
{
    class GameData
    {
        public enum GameType
        {
            Server,
            Client,
            Local
        }
        public GameType TypeOfGame
        {
            get;
            set;
        }
    }
}

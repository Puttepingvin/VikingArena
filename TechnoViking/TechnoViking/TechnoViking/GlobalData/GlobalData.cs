using System;
using System.Collections.Generic;
using System.Windows;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Utilities;

using Microsoft.Xna.Framework;
#if !FRB_MDX
using System.Linq;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FlatRedBall.Gui;
using FlatRedBall.Math.Geometry;
#endif
using FlatRedBall.Screens;
using FlatRedBall.Graphics.Animation;
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

        public static Effect Lighting
        {
            get;
            set;
        }

        public static void Initialize(Game game)
        {
            GameData = new GameData();
            GameData.TypeOfGame = GameData.GameType.Local;
            Lighting = game.Content.Load<Effect>("Lighting");
        }
    }
}

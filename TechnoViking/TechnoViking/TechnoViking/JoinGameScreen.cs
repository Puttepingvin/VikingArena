using System;
using System.Collections.Generic;

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

namespace TechnoViking
{
    class JoinGameScreen :GameObject
    {
        TextBox ipbox;
        public JoinGameScreen(Game game, Sprite sprite, List<GameObject> gameObjects)
            : base(game, sprite) 
        {
            
        }
        public override void Update(List<GameObject> gameObjects)
        { 
            
        }
        public override void Kill(List<GameObject> gameObjects)
        {

        }
    }
}

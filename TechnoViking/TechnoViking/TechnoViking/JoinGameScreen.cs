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
        private Menuscreen mmainmenu;
        TextBox ipbox;
        FlatRedBall.Graphics.Text prompter;
        public JoinGameScreen(Game game, Sprite sprite, List<GameObject> gameObjects, Menuscreen mainmenu)
            : base(game, sprite) 
        {
            ipbox = new TextBox(0, 0, 20);
            ipbox.Selected = true;
            prompter = FlatRedBall.Graphics.TextManager.AddText("Enter the IP of the server you wish to join");
            prompter.Position.Y += 7;
            prompter.Position.X -= 5;
            GlobalData.GlobalData.keyboardDispatcher.Subscriber = ipbox;
            mmainmenu = mainmenu;
        }
        public override void Update(List<GameObject> gameObjects)
        {
            ipbox.Update(gameObjects);
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
            GlobalData.GlobalData.GameData.TypeOfGame = GlobalData.GameData.GameType.Client;
            mmainmenu.Creategamescreen(gameObjects, ipbox.Text);
                
            this.Kill(gameObjects);
            }
        }
        public override void Kill(List<GameObject> gameObjects)
        {
            ipbox.Kill(gameObjects);
            FlatRedBall.Graphics.TextManager.RemoveText(prompter);
            gameObjects.Remove(this);
        }
    }
}

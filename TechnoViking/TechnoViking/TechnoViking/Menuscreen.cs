using System;
using System.Collections.Generic;

using Lidgren.Network;

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

namespace TechnoViking
{
    class Menuscreen : GameObject
    {
        Screentype screentype;
        TextBox ipbox;
        FlatRedBall.Graphics.Text prompter;
        Gamescreen multiplayerarena1;
        MouseState mousestate;
        List<MenuButton> menubuttons = new List<MenuButton>();
        public Menuscreen(Game game, Sprite sprite, List<GameObject> gameObjects)
            : base(game, sprite) 
        {
            Createmainmenu(gameObjects);
        }

        public override void Update(List<GameObject> gameObjects)
        {
            if (screentype == Screentype.Main)
            {
                mousestate = Mouse.GetState();
                if (mousestate.LeftButton == ButtonState.Pressed)
                {
                    foreach (MenuButton btn in new List<MenuButton>(menubuttons))
                    {
                        if (btn.MouseOver())
                        {
                            CreateScreen((byte)btn.Buttonindex, gameObjects);
                        }
                    }
                }
            }
            if (screentype == Screentype.Join)
            {
                ipbox.Update(gameObjects);
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    GlobalData.GlobalData.GameData.TypeOfGame = GlobalData.GameData.GameType.Client;
                    Creategamescreen(gameObjects, ipbox.Text);
                }
            }
        }

        private void CreateScreen(byte screentype, List<GameObject> gameObjects) 
        {
            switch (screentype)
            {
                
                case 1:
                    GlobalData.GlobalData.GameData.TypeOfGame = GlobalData.GameData.GameType.Server;
                    Creategamescreen(gameObjects);
                    break;
                case 2:
                    GlobalData.GlobalData.GameData.TypeOfGame = GlobalData.GameData.GameType.Client;
                    CreateJoinScreen(gameObjects);
                    break;
            }
        }

        /// <summary>
        /// //Creates all the classes for the mainmenu and adds them to the proper lists
        /// </summary>
        /// <param name="gameObjects"></param>
        /// 
        private void Createmainmenu(List<GameObject> gameObjects) 
        {

            MenuButton CreateServer = new MenuButton(game, SpriteManager.AddSprite(Game1.MenuButtonTexture1), 1.0f, this);
            gameObjects.Add(CreateServer);
            menubuttons.Add(CreateServer);
            CreateServer.Sprite.Position.Y += 6;
            MenuButton JoinServer = new MenuButton(game, SpriteManager.AddSprite(Game1.MenuButtonTexture2), 2.0f, this);
            gameObjects.Add(JoinServer);
            menubuttons.Add(JoinServer);
            screentype = Screentype.Main;

        }

        /// <summary>
        /// Creates the gamescreen without specifying the IP, only as server
        /// </summary>
        /// <param name="gameObjects"></param>
        public void Creategamescreen(List<GameObject> gameObjects)
        {
            Creategamescreen(gameObjects, "");
        }

        /// <summary>
        /// Creates a gamescreen and adds it to the proper list
        /// </summary>
        /// <param name="gameObjects"></param>
        /// <param name="ip"></param>
        public void Creategamescreen(List<GameObject> gameObjects, string ip)
        {
            this.Kill(gameObjects);
            multiplayerarena1 = new Gamescreen(game, null, gameObjects, ip);
            gameObjects.Add(multiplayerarena1);

            if (multiplayerarena1.Stop)
            {
                gameObjects.Remove(multiplayerarena1);
                Createmainmenu(gameObjects);
            }
            else gameObjects.Remove(this);
        }

        /// <summary>
        /// Should create all the classes needed for a joinscreen and add them to the proper lists
        /// </summary>
        /// <param name="gameObjects"></param>
        public void CreateJoinScreen(List<GameObject> gameObjects) //FIX
            {
                //JoinGameScreen joinScreen = new JoinGameScreen(game, null, gameObjects, this);
                //gameObjects.Add(joinScreen);
                this.Kill(gameObjects);
                ipbox = new TextBox(0, 0, 20);
                ipbox.Selected = true;
                prompter = FlatRedBall.Graphics.TextManager.AddText("Enter the IP of the server you wish to join");
                prompter.Position.Y += 7;
                prompter.Position.X -= 5;
                GlobalData.GlobalData.keyboardDispatcher.Subscriber = ipbox;
                screentype = Screentype.Join;
            }

        /// <summary>
        /// Unloads the menuscreen
        /// </summary>
        /// <param name="gameObjects"></param>
        public override void Kill(List<GameObject> gameObjects) 
        {
            if (screentype == Screentype.Main)
            {
                foreach (GameObject g in new List<GameObject>(gameObjects))
                {
                    if (g is MenuButton)
                    {
                        gameObjects.Remove(g);
                        SpriteManager.RemoveSprite(g.Sprite);
                    }
                }
            }
            else if (screentype == Screentype.Join)
            {
                ipbox.Kill(gameObjects);
                FlatRedBall.Graphics.TextManager.RemoveText(prompter);
            }

        }

        public override void SendState(NetworkAgent mAgent)
        {
        }

        private enum Screentype 
        {
            Main,
            Join
        }
    }
}
//using System;
//using System.Collections.Generic;

//using FlatRedBall;
//using FlatRedBall.Graphics;
//using FlatRedBall.Utilities;

//using Microsoft.Xna.Framework;
//#if !FRB_MDX
//using System.Linq;

//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
//using FlatRedBall.Gui;
//using FlatRedBall.Math.Geometry;
//#endif

//namespace TechnoViking
//{
//    class JoinGameScreen :GameObject
//    {
//        private Menuscreen mmainmenu;
//        TextBox ipbox;
//        FlatRedBall.Graphics.Text prompter;
//        public JoinGameScreen(Game game, Sprite sprite, List<GameObject> gameObjects, Menuscreen mainmenu)
//            : base(game, sprite) 
//        {
//            ipbox = new TextBox(0, 0, 20);
//            ipbox.Selected = true;
//            prompter = FlatRedBall.Graphics.TextManager.AddText("Enter the IP of the server you wish to join");
//            prompter.Position.Y += 7;
//            prompter.Position.X -= 5;
//            GlobalData.GlobalData.keyboardDispatcher.Subscriber = ipbox;
//            mmainmenu = mainmenu;
//        }
//        public override void Update(List<GameObject> gameObjects)
//        {
//            ipbox.Update(gameObjects);
//            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
//            {
//            GlobalData.GlobalData.GameData.TypeOfGame = GlobalData.GameData.GameType.Client;
//            mmainmenu.Creategamescreen(gameObjects, ipbox.Text);
//            this.Kill(gameObjects);
//            }
//        }
//        public override void Kill(List<GameObject> gameObjects)
//        {
//            ipbox.Kill(gameObjects);
//            FlatRedBall.Graphics.TextManager.RemoveText(prompter);
//            gameObjects.Remove(this);
//        }
//    }
//}
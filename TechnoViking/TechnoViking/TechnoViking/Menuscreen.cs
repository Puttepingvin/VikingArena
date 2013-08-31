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
        Gamescreen multiplayerarena1;
        Sprite dummysprite = null;
        public Menuscreen(Game game, Sprite sprite, List<GameObject> gameObjects)
            : base(game, sprite) 
        {
            Createmainmenu(gameObjects);
        }

        public override void Update(List<GameObject> gameObjects)
        {

        }

        private void Createmainmenu(List<GameObject> gameObjects)
        {

            MenuButton CreateServer = new MenuButton(game, SpriteManager.AddSprite(Game1.MenuButtonTexture1), 1.0f, this);
            gameObjects.Add(CreateServer);
            CreateServer.Sprite.Position.Y += 6;
            MenuButton JoinServer = new MenuButton(game, SpriteManager.AddSprite(Game1.MenuButtonTexture2), 2.0f, this);
            gameObjects.Add(JoinServer);
            MenuButton PlayOffline = new MenuButton(game, SpriteManager.AddSprite(Game1.MenuButtonTexture3), 3.0f, this);
            PlayOffline.Sprite.Position.Y -= 6;
            gameObjects.Add(PlayOffline);

        }

        public void Creategamescreen(List<GameObject> gameObjects)
        {
            multiplayerarena1 = new Gamescreen(game, dummysprite, gameObjects);
            gameObjects.Add(multiplayerarena1);
            this.Kill(gameObjects);
        }


        private void Kill(List<GameObject> gameObjects) 
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


    }
}

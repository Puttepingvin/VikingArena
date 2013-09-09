﻿using System;
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
using FlatRedBall.Screens;

namespace TechnoViking
{
    class MenuButton : GameObject
    {
        
        Sprite sprite;
        float buttonindex;
        Player player1;
        MouseState mousestate;
        Menuscreen screen;

        public MenuButton(Game game, Sprite sprite, float buttonindex, Menuscreen screen)
            : base(game, sprite) 
        {
            this.sprite = sprite;
            this.buttonindex = buttonindex;
            this.screen = screen;
            float texturePixelWidth = sprite.Texture.Width;
            float texturePixelHeight = sprite.Texture.Height;

            ////Now, we need to find out how many pixels per unit there are in our view at the Sprite's Z position:
            //float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(sprite.Z);

            ////Now, we just have to use those two values to set the scale.
            //sprite.ScaleX = .5f * texturePixelWidth / pixelsPerUnit;
            //sprite.ScaleY = .5f * texturePixelHeight / pixelsPerUnit;
        }

        public override void Update(List<GameObject> gameObjects)
        {
            mousestate  = Mouse.GetState();
            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(sprite.Z);
            if ((GuiManager.Cursor.WorldXAt(sprite.Z) < sprite.Position.X + sprite.Texture.Width / pixelsPerUnit / 2 && GuiManager.Cursor.WorldXAt(sprite.Z) > sprite.Position.X - sprite.Texture.Width / pixelsPerUnit / 2)
                && (GuiManager.Cursor.WorldYAt(sprite.Z) < sprite.Position.Y + sprite.Texture.Height / pixelsPerUnit / 2) && (GuiManager.Cursor.WorldYAt(sprite.Z) > sprite.Position.Y - sprite.Texture.Height / pixelsPerUnit / 2)
                && (mousestate.LeftButton == ButtonState.Pressed) ) 
            {
                switch ((int)buttonindex) 
                {
                    case 1:
                        GlobalData.GlobalData.GameData.TypeOfGame = GlobalData.GameData.GameType.Server;
                        screen.Creategamescreen(gameObjects);
                        break;
                    case 2:
                        GlobalData.GlobalData.GameData.TypeOfGame = GlobalData.GameData.GameType.Client;
                        screen.CreateJoinScreen(gameObjects);
                        break;
                    case 3:
                        GlobalData.GlobalData.GameData.TypeOfGame = GlobalData.GameData.GameType.Local;
                        screen.Creategamescreen(gameObjects);
                        break;


                }
            }
        }
        private void LoadGame(List<GameObject> gameObjects)
        {
            player1 = new Player(game, SpriteManager.AddSprite(Game1.PlayerTexture1));
            gameObjects.Add(player1);
            player1.Sprite.ScaleX = 1.3f;
            player1.Sprite.ScaleY = 1.3f;
        }
        public override void Kill(List<GameObject> gameObjects)
        {
            
        }
    }
}

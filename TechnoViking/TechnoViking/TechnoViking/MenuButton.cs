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
using FlatRedBall.Screens;

namespace TechnoViking
{
    class MenuButton : GameObject
    {
        
        Sprite sprite;

        public MenuButton(Game game, Sprite sprite, byte buttonindex)
            : base(game, sprite) 
        {
            this.sprite = sprite;
            this.Buttonindex = buttonindex;
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
        }

        public bool MouseOver() 
        {
            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(sprite.Z);
            if ((GuiManager.Cursor.WorldXAt(sprite.Z) < sprite.Position.X + sprite.Texture.Width / pixelsPerUnit / 2 && GuiManager.Cursor.WorldXAt(sprite.Z) > sprite.Position.X - sprite.Texture.Width / pixelsPerUnit / 2)
                && (GuiManager.Cursor.WorldYAt(sprite.Z) < sprite.Position.Y + sprite.Texture.Height / pixelsPerUnit / 2) && (GuiManager.Cursor.WorldYAt(sprite.Z) > sprite.Position.Y - sprite.Texture.Height / pixelsPerUnit / 2))
            {

                return true;
            }
            else return false;
        }

        public byte Buttonindex
        {
            get;
            set;
        }

        public override void Kill(List<GameObject> gameObjects)
        {
            
        }

        public override void SendState(NetworkAgent mAgent)
        {
        }
    }
}

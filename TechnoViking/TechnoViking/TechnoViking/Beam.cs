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
    
    class Beam :GameObject
    {
        private Sprite sprite;
        private float duration = 5f;
        private float rotationoffset = (float)Math.PI / 2.0f;
        private float offsetY = 0.0f;
        private float offsetX = 0.0f;
        private float angle;
        private Player player;
        Emittor beamsparks;

        public Beam(Game game, Sprite sprite, Player player, List<GameObject> gameObjects, float angle)
            : base(game, sprite)

        {
            this.sprite = sprite;
            this.player = player;
            this.angle = angle;
            PositionRotation(); 
            beamsparks = new Emittor(game, SpriteManager.AddSprite(Game1.Pixeltexture), 0.5f , Color.White, Color.Pink, Color.LightPink, 0.3f,
                float.MaxValue, this, 1.0f, (angle - (float)Math.PI / 2) - 0.3f, (angle - (float)Math.PI / 2) + 0.3f, rotationoffset, true);
            gameObjects.Add(beamsparks);
        }


        public override void Update(List<GameObject> gameObjects)
        {
            player.Locked = true;
            if (TimeManager.CurrentTime >= sprite.TimeCreated + duration || Mouse.GetState().LeftButton != ButtonState.Pressed) 
            {
                SpriteManager.RemoveSprite(sprite);
                gameObjects.Remove(this);
                player.Locked = false;
                beamsparks.Kill = true;
            }
            

        }

        private void PositionRotation() 
        {

            
            float worldX = GuiManager.Cursor.WorldXAt(sprite.Z);
            float worldY = GuiManager.Cursor.WorldYAt(sprite.Z);
            angle = (float)Math.Atan2(
            worldY - player.Sprite.Y, worldX - player.Sprite.X);

            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(sprite.Z);

            offsetX = ((sprite.Texture.Height + player.Sprite.Texture.Height) / pixelsPerUnit) / 2 * (float)Math.Cos(angle);
            offsetY = ((sprite.Texture.Height + player.Sprite.Texture.Height) / pixelsPerUnit) / 2 * (float)Math.Sin(angle);

            sprite.Position.X = player.Sprite.Position.X + offsetX;
            sprite.Position.Y = player.Sprite.Position.Y + offsetY;
            sprite.RotationZ = angle + rotationoffset;

        }

    }
}

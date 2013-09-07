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
    class Projectile : Actor
    {
        private Sprite sprite;
        private const float projectilevelocity = 20.0f;
        private float offsetX = 0.0f;
        private float offsetY = 0.0f;
        private Vector3 startingposition;
        private float life = .4f;
        Emittor firetail;
        int selectedSpell;
        private byte mplayerID;

        public byte playerID 
        {
            get
            {
                return mplayerID;
            }
        }


        public Projectile(Game game, Sprite sprite, Player player, int selectedSpell, List<GameObject> gameObjects, float angle)
            : base(game, sprite)

        {
            
            this.sprite = sprite;
            
            sprite.ScaleX = 0.3f;
            sprite.ScaleY = 0.3f;

            sprite.Position = player.Sprite.Position;

            mplayerID = (byte)player.Playerindex;

            sprite.Velocity.X = (float)Math.Cos(angle) * projectilevelocity;
            sprite.Velocity.Y = (float)Math.Sin(angle) * projectilevelocity;
            sprite.RotationZ = angle;
            
            
            float texturePixelWidth = sprite.Texture.Width;
            float texturePixelHeight = sprite.Texture.Height;

            //Now, we need to find out how many pixels per unit there are in our view at the Sprite's Z position:
            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(sprite.Z);

            //Now, we just have to use those two values to set the scale.
            //sprite.ScaleX = .5f * texturePixelWidth / pixelsPerUnit;
            //sprite.ScaleY = .5f * texturePixelHeight / pixelsPerUnit;

            offsetX = ((texturePixelWidth + player.Sprite.Texture.Width) / pixelsPerUnit) / 2 * (float)Math.Cos(angle);
            offsetY = ((texturePixelHeight + player.Sprite.Texture.Width) / pixelsPerUnit) / 2 * (float)Math.Sin(angle);
            sprite.Position.X = player.Sprite.Position.X + offsetX;
            sprite.Position.Y = player.Sprite.Position.Y + offsetY;
            this.selectedSpell = selectedSpell;
            if (selectedSpell == 1)
            {
                firetail = new Emittor(game, SpriteManager.AddSprite(Game1.Pixeltexture), .15f, Color.Tomato, Color.Yellow, Color.Orange,
                    0.05f, float.MaxValue, this, 1, (angle + (float)Math.PI / 2), ((angle + (float)Math.PI * 3 / 2)), 0, false);
                gameObjects.Add(firetail);
            }


        }





        /* 
        enum Spellbook : int
        {
            shadowbolt,
            fireball,
            beam
        }
         */





        public override void Update(List<GameObject> gameObjects)
        {




            if (TimeManager.CurrentTime >= sprite.TimeCreated + life)
            {
                this.Kill(gameObjects);
            }







        }

        public override void Kill(List<GameObject> gameObjects) 
        {
            SpriteManager.RemoveSprite(sprite);
            gameObjects.Remove(this);
            if (selectedSpell != 1)
            {
                Emittor explosion = new Emittor(game, SpriteManager.AddSprite(Game1.Pixeltexture), .3f, Color.Tomato, Color.Yellow, Color.Orange,
                    0.3f, 0.3f, sprite.Position, 3, (0), ((float)Math.PI * 2));
                gameObjects.Add(explosion);
            }
            if (selectedSpell == 1)
            {
                firetail.Kill(gameObjects);
            }
        }

    }
}

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
        private float projectilevelocity = 30.0f;
        private float offsetX = 0.0f;
        private float offsetY = 0.0f;
        private float life = 0.5f;
        Emittor firetail;
        int selectedSpell;
        public byte damage = 0;
        private byte mplayerID;
        Player player;
        float CursorX;
        float CursorY;
        float angle;
        int mspellindex;
        bool positionset;

        public int Spellindex 
        {
            get 
            {
                return mspellindex;
            }
        }

        public byte playerID 
        {
            get
            {
                return mplayerID;
            }
        }

        public Projectile(Game game, Sprite sprite, Player player, int selectedSpell, List<GameObject> gameObjects, float CursorX, float CursorY)
            : base(game, sprite)
        {
            mspellindex = selectedSpell;
            this.sprite = sprite;
            this.CursorX = CursorX;
            this.CursorY = CursorY;
            mplayerID = (byte)player.Playerindex;
            this.player = player;
            switch(selectedSpell){
                case 0:
                    sprite.ScaleX = 0.3f;
                    sprite.ScaleY = 0.3f;
                    base.InvMass = 2.0f;
                    damage = 20;
            break;
                case 1:
                    sprite.ScaleX = 0.6f;
                    sprite.ScaleY = 0.6f;
                    projectilevelocity = 20f;
                    life = 0.7f;
                    base.InvMass = 10.0f;
                    damage = 20;
            break;
            }
            this.selectedSpell = selectedSpell;

        }


        public void SetPosition(List<GameObject> gameObjects)
        {
            angle = (float)Math.Atan2(
                CursorY - player.Sprite.Y, CursorX - player.Sprite.X);
            sprite.Position = player.Sprite.Position;
            sprite.Velocity.X = (float)Math.Cos(angle) * projectilevelocity;
            sprite.Velocity.Y = (float)Math.Sin(angle) * projectilevelocity;
            sprite.RotationZ = angle;
            float texturePixelWidth = sprite.Texture.Width;
            float texturePixelHeight = sprite.Texture.Height;
            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(sprite.Z);
            offsetX = ((texturePixelWidth + player.Sprite.Texture.Width) / pixelsPerUnit) / 2 * (float)Math.Cos(angle);
            offsetY = ((texturePixelHeight + player.Sprite.Texture.Width) / pixelsPerUnit) / 2 * (float)Math.Sin(angle);
            sprite.Position.X = player.Sprite.Position.X + offsetX;
            sprite.Position.Y = player.Sprite.Position.Y + offsetY;
            positionset = true;
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

        public override void SendState(NetworkAgent mAgent)
        {
            if (positionset) 
            {
                mAgent.WriteMessage((byte)GlobalData.MessageType.Spell);
                mAgent.WriteMessage((byte)selectedSpell);
                mAgent.WriteMessage(angle);
                mAgent.WriteMessage(sprite.Position.X);
                mAgent.WriteMessage(sprite.Position.Y);
                mAgent.WriteMessage((byte)playerID);
                
            }
        }
    }
}

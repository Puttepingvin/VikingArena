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
    class Particle : GameObject
    {

        private Sprite sprite;
        float life;

        public Particle(Game game, Sprite sprite, float life)
            : base(game, sprite)
        {
            this.life = life;
            this.sprite = sprite;
            sprite.ScaleX = 0.15f;
            sprite.ScaleY = 0.15f;
        }
        
        
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
        }
    }
}
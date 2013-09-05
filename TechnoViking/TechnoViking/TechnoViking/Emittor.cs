using System;
using System.Collections.Generic;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Utilities;
using FlatRedBall.Graphics.Animation;
using Microsoft.Xna.Framework;
#if !FRB_MDX
using System.Linq;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;
using FlatRedBall.Graphics.Texture;

using FlatRedBall.Gui;
using FlatRedBall.Math.Geometry;
#endif
using FlatRedBall.Screens;
namespace TechnoViking

{
    class Emittor : GameObject
    {
        Sprite sprite;
        //List<Particle> particles = new List<Particle>();
        Color color1;
        Color color2;
        Color color3;
        float life;
        Random rng = new Random();
        float frequency;
        float lastparticlecreated;
        const float particlespeed = 5.0f;
        float emmissionduration;
        float particlecount;
        float anglemin;
        float anglemax;
        bool moving;
        GameObject gameObject;
        bool multiplespawnpoints;
        float rotationoffset;
        bool twoside;
        int particlecreatedcount = 0;

        //AnimationChain animationChain = new AnimationChain();
        

        
        //private float rotationoffset;

        public Emittor(Game game, Sprite sprite, float life, Color color1, Color color2, Color color3, float frequency, float emmissionduration, Vector3 position,
            float particlecount, float anglemin, float anglemax)
            : base(game, sprite)
        {
            this.sprite = sprite;
            this.color1 = color1;
            this.color2 = color2;
            this.color3 = color3;
            this.life = life;
            this.emmissionduration = emmissionduration;
            this.frequency = frequency;
            this.lastparticlecreated = (float)TimeManager.CurrentTime - frequency;
            sprite.Position = position;
            this.particlecount = particlecount;
            this.anglemin = anglemin * 180 / (float)Math.PI;
            this.anglemax = anglemax * 180 / (float)Math.PI;
            sprite.ScaleX = float.MinValue;
            sprite.ScaleY = float.MinValue;
            //animationChain.Add(new AnimationFrame("/Content/particle1.png", .05f, "Global"));
            //animationChain.Add(new AnimationFrame("/Content/particle2.png", .05f, "Global"));
            //animationChain.Add(new AnimationFrame("/Content/particle3.png", .05f, "Global"));
            //animationChain.Add(new AnimationFrame("/Content/particle4.png", .05f, "Global"));
            //animationChain.Add(new AnimationFrame("/Content/particle5.png", .05f, "Global"));
            //animationChain.Add(new AnimationFrame("/Content/particle6.png", .05f, "Global"));
        }

        public Emittor(Game game, Sprite sprite, float life, Color color1, Color color2, Color color3, float frequency, float emmissionduration, GameObject gameObject,
            float particlecount, float anglemin, float anglemax, float rotationoffset, bool twoside)
            : base(game, sprite)
        {
            this.sprite = sprite;
            this.color1 = color1;
            this.color2 = color2;
            this.color3 = color3;
            this.life = life;
            this.emmissionduration = emmissionduration;
            this.frequency = frequency;
            this.lastparticlecreated = (float)TimeManager.CurrentTime - frequency;
            sprite.Position = gameObject.Sprite.Position;
            this.particlecount = particlecount;
            this.anglemin = anglemin * 180 / (float)Math.PI;
            this.anglemax = anglemax * 180 / (float)Math.PI;
            this.gameObject = gameObject;
            moving = true;
            multiplespawnpoints = true;
            sprite.ScaleX = float.MinValue;
            sprite.ScaleY = float.MinValue;
            this.rotationoffset = rotationoffset;
            this.twoside = twoside;
        }

        
        public override void Update(List<GameObject> gameObjects)
        {

            if (moving == true) 
            {
                sprite.Position = gameObject.Sprite.Position;
            }

            if (TimeManager.CurrentTime >= lastparticlecreated + frequency)
            {
                lastparticlecreated = (float)TimeManager.CurrentTime;
                for (int i = 0; i < particlecount; i++)
                {
                    CreateParticle(gameObjects);
                }

            }

            if (TimeManager.CurrentTime >= sprite.TimeCreated + emmissionduration)
            {
                this.Kill(gameObjects);

            }

        }

        private void CreateParticle(List<GameObject> gameObjects) 
        {


            Color thiscolor = Color.White;

            switch (particlecreatedcount % 3)
            {
                case 0:
                    thiscolor = color1;
                    break;
                case 1:
                    thiscolor = color2;
                    break;
                case 2:
                    thiscolor = color3;
                    break;
            }
            particlecreatedcount++;
            Sprite particlesprite = SpriteManager.AddSprite(Game1.Pixeltexture);
            particlesprite.Red = thiscolor.R / 255;
            particlesprite.Green = thiscolor.G / 255;
            particlesprite.Blue = thiscolor.B / 255;
            particlesprite.ColorOperation = ColorOperation.ColorTextureAlpha;
            particlesprite.Position = sprite.Position;
            if (multiplespawnpoints) 
            {
                int topoffsetY = (int)((gameObject.Sprite.Height / 2) * 1000 );
                int botoffsetY = -(int)((gameObject.Sprite.Height / 2) * 1000);
                int topoffsetX = (int)((gameObject.Sprite.Height / 2) * 1000 );
                int botoffsetX = -(int)((gameObject.Sprite.Height / 2) * 1000 );
                float tempfloat = ((float)(rng.Next(botoffsetY, topoffsetY)) / 1000);

                particlesprite.Position.Y += tempfloat * (float)Math.Sin(gameObject.Sprite.RotationZ - rotationoffset);
                particlesprite.Position.X += tempfloat * (float)Math.Cos(gameObject.Sprite.RotationZ - rotationoffset);

            }

            float angle = (float)rng.Next((int)anglemin, (int)anglemax) * (int)Game1.pi / 180;
            if (twoside)
            {
                if (particlecreatedcount % 2 == 0)
                {
                    angle += Game1.pi;
                }
            }
            particlesprite.Velocity.X = (float)Math.Cos(angle) * particlespeed;
            particlesprite.Velocity.Y = (float)Math.Sin(angle) * particlespeed;
            Particle particle = new Particle(game, particlesprite, life);
            gameObjects.Add(particle);
        }

        public override void Kill(List<GameObject> gameObjects) 
        {
            SpriteManager.RemoveSprite(sprite);
            gameObjects.Remove(this);
        }
    }
}

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
    class Player : Actor
    {

        float offset = (float)Math.PI / 2.0f;
        public float desiredRotation = 0;
        private Sprite sprite;
        private KeyboardState mkeystate;
        private MouseState mmousestate;
        public float goalVelocityX = 0.0f;
        public float goalVelocityY = 0.0f;
        public int keycount = 0;
        private bool rotationlocked;
        private bool rotated;
        private int score;
        double[] lastcasted = new double[3] { -100, -100, -100 };
        Vector3 startposition = new Vector3(0, 0, 0);
        float mrotationspeed = 2*(float)Math.PI;

        public float Offset 
        {
            get { return offset; }
        }

        public float RotationSpeed 
        {
            get { return mrotationspeed; }
        }

        public double[] Lastcasted 
        {
            get { return lastcasted; }
            set { lastcasted = value; }
        }

        public KeyboardState keystate
        {
            get
            {
                return mkeystate;
            }
        }
        public MouseState mousestate
        {
            get
            {
                return mmousestate;
            }
        }
        //public float CurrentXAcceleration
        //{
        //    get;
        //    set;
        //}
        //public float CurrentYAcceleration
        //{
        //    get;
        //    set;
        //}
        public float Playerindex
        {
            get;
            set;
        }

        public bool Locked
        {
            get;
            set;
        }

        public Player(Game game, Sprite sprite)
            : base(game, sprite)

        {
            this.sprite = sprite;
        }

        public override void Update(List<GameObject> gameObjects)
        {
            mkeystate = Keyboard.GetState();
            mmousestate = Mouse.GetState();
            
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                UnloadGame(gameObjects);
            }
        }

        


        //public void Castspell(int spellindex, List<GameObject> gameObjects, float angle) 
        //{
        //    switch ((int)spellindex)
        //    {
        //        case 0:
        //            tempprojectile = new Projectile(game, SpriteManager.AddSprite(Game1.Shadowbolttexture1), this, spellindex, gameObjects, angle);
        //            gameObjects.Add(tempprojectile);
        //            break;
        //        case 1:
        //            tempprojectile = new Projectile(game, SpriteManager.AddSprite(Game1.Fireballtexture1), this, spellindex, gameObjects, angle);
        //            gameObjects.Add(tempprojectile);
        //            break;
        //        case 2:
        //            tempbeam = new Beam(game, SpriteManager.AddSprite(Game1.Beamtexture1), this, gameObjects, angle);
        //            gameObjects.Add(tempbeam);
        //            break;
        //    }
        //}




        public override void Kill(List<GameObject> gameObjects) 
        {
            gameObjects.Remove(this);
            SpriteManager.RemoveSprite(this.Sprite);
        }



        public bool RotationLocked 
        {
            get { return rotationlocked; }
            set { rotationlocked = value; }
        }

        //public bool Rotated
        //{
        //    get { return rotated; }
        //    set { rotated = value; }
        //}

        private void UnloadGame(List<GameObject> gameObjects)
        {
            SpriteManager.RemoveSprite(sprite);
            gameObjects.Remove(this);
        }

    }
    
}


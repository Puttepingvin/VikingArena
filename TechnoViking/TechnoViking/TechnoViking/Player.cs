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
        //This class is practicly just a bunch of variables, could remove from gameobjects?
        float offset = (float)Math.PI / 2.0f;
        public float desiredRotation = 0;
        private Sprite sprite;
        public float goalVelocityX = 0.0f;
        public float goalVelocityY = 0.0f;
        public int keycount = 0;
        public int oldkeycount = 0;
        private bool rotationlocked;
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
        public bool RotationLocked
        {
            get { return rotationlocked; }
            set { rotationlocked = value; }
        }
        public double[] Lastcasted 
        {
            get { return lastcasted; }
            set { lastcasted = value; }
        }
        public KeyboardState keystate
        {
            get;
            set;
        }
        public MouseState mousestate
        {
            get;
            set;
        }
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
            keystate = Keyboard.GetState();
            mousestate = Mouse.GetState();
        }

        public override void Kill(List<GameObject> gameObjects) 
        {
            gameObjects.Remove(this);
            SpriteManager.RemoveSprite(this.Sprite);
        }





    }
    
}


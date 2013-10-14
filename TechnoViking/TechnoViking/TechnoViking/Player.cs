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
    //81.230.67.177
    class Player : Actor
    {
        float offset = (float)Math.PI / 2.0f;
        public float desiredRotation = 0;
        private Sprite sprite; //Velocity, position
        public float goalVelocityX = 0.0f; 
        public float goalVelocityY = 0.0f; 
        public int keycount = 0;
        public int oldkeycount = 0;
        public float Radius
        {
            get;
            set;
        }
        private bool rotationlocked;
        private byte hp = 100;
        public byte HP 
        {
            get { return hp; }
            set { hp = value; }

        }
        public int Score
        {
            get;
            set;
        } 
        double[] lastcasted = new double[3] { -100, -100, -100 };
        double[] pickedspell = new double[3] { 0, 1, 2 };
        public double[] PickedSpell
        {
            get { return pickedspell; }
            set { pickedspell = value; }
        }
        //Vector3 startposition = new Vector3(0, 0, 0);
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
        } //Synka om en ny projektil upptäcks
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
        public byte Playerindex
        {
            get;
            set;
        }
        public bool Locked
        {
            get;
            set;
        }
        public float MouseX
        {
            get;
            set;
        }
        public float MouseY
        {
            get;
            set;
        }
        public int OldMouseValue
        {
            get;
            set;
        }
        private const byte prevLenght = 20;
        private Vector3 interPos = new Vector3(0 ,0 ,0);
        public Vector3 InterPos
        {
            get { return interPos; }
            set { interPos = value - sprite.Position; }
        }
        List<Vector2> prevPos = new List<Vector2>();
        public List<Vector2> PrevPos 
        {
            get { return prevPos; }
        }
        public Spellbook selectedSpell = Spellbook.shadowbolt; //

        public Player(Game game, Sprite sprite)
            : base(game, sprite)

        {
            this.sprite = sprite;
            base.InvMass = 3;
        }

        public override void Update(List<GameObject> gameObjects)
        {
            keystate = Keyboard.GetState();
            mousestate = Mouse.GetState();
            if (prevPos.Count >= prevLenght) 
            {
                prevPos.RemoveAt(0);
            }
            prevPos.Add(new Vector2(sprite.Position.X, sprite.Position.Y));

        }

        public override void Kill(List<GameObject> gameObjects) 
        {
            gameObjects.Remove(this);
            SpriteManager.RemoveSprite(this.Sprite);
        }
        
        public enum Spellbook : int
        {
            shadowbolt,
            fireball,
            //meadbeam,
        }

        public override void SendState(NetworkAgent mAgent)
        {
            mAgent.WriteMessage(Playerindex);
            mAgent.WriteMessage(sprite.Position.X);
            mAgent.WriteMessage(sprite.Position.Y);
            mAgent.WriteMessage(sprite.Velocity.X);
            mAgent.WriteMessage(sprite.Velocity.Y);              
        }


    }
    
}


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
    abstract class GameObject
    {
        protected Game game;
        private Sprite sprite = new Sprite();
        private float mass;
        private float radius;
        private float softness = 2;

        public GameObject(Game game, Sprite sprite)
        {
            this.game = game;
            this.sprite = sprite;



            if (sprite != null)
                {

                    Pixelsieze();
                }
            //if (this is Actor)
            //{
            //    if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Client)
            //    {
            //        RemoteRole = ENetRole.ROLE_Authority;
            //    }
            //    else if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server)
            //    {
            //        Role = ENetRole.ROLE_Authority;
            //    }
            //}


        }

        // Net variables.
        public enum ENetRole
        {
            ROLE_None,              // No role at all.
            ROLE_SimulatedProxy,    // Locally simulated proxy of this actor.
            ROLE_AutonomousProxy,   // Locally autonomous proxy of this actor.
            ROLE_Authority,         // Authoritative control over the actor.
        };
        
        //ENetRole RemoteRole, Role;

        public void Pixelsieze() 
        {
            float texturePixelWidth = sprite.Texture.Width;
            float texturePixelHeight = sprite.Texture.Height;

            //Now, we need to find out how many pixels per unit there are in our view at the Sprite's Z position:
            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(sprite.Z);

            //Now, we just have to use those two values to set the scale.
            sprite.ScaleX = .5f * texturePixelWidth / pixelsPerUnit;
            sprite.ScaleY = .5f * texturePixelHeight / pixelsPerUnit;
        }
        
        public Sprite Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }

        public float Softness
        {
            get { return softness; }
            set { softness = value; }
        }

        public float InvMass 
        {
            get { return mass; }
            set {
                if (value == 0) 
                {
                    mass = 0;
                }    
                else mass = 1/value;
            
            }

        }

        public bool CircleCollidesWith(GameObject gameobject)
        {
            return CircleCollidesWith(gameobject, Math.Max(gameobject.Sprite.Height, gameobject.Sprite.Width), Math.Max(Sprite.Height, Sprite.Width));
        }

        public bool CircleCollidesWith(GameObject gameobject, float r1, float r2) 
        {
            float distancesquared;
            float distanceX = sprite.Position.X - gameobject.Sprite.Position.X; 
            float distanceY = sprite.Position.Y - gameobject.Sprite.Position.Y; 
            distancesquared = distanceX *distanceX + distanceY * distanceY;


            if (((r1 / 2 ) * (r1 / 2))*2 + ((r2 / 2) * (r2 / 2))*2 > distancesquared)
            {
            return true;
            }
            else return false;
        }

        

        public abstract void Update(List<GameObject> gameObjects);

        public abstract void Kill(List<GameObject> gameObjects);

        public abstract void SendState(NetworkAgent mAgent);

    }
}

using System;
using System.Collections.Generic;
using System.Windows;

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
using FlatRedBall.Graphics.Animation;
namespace TechnoViking
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        /*
         TODO:
         Clean up code
         Comment code
         Improve spellcasting
         Loock player during beam
         Update net code
         * abstract send in actor classes
         Update Menu
         Display scores
         Save r^2
         connectionscreen
         rotation is cosmetic, shouldn't ever be sent over network
         only input should be sent over network outside of ticks, this takes more of a toll on the computer and less on the network, wich should be ok since our game shouldn't be that demanding from a cpu stantpoint anyway
         */



        GraphicsDeviceManager graphics; //Needed to draw things to screen

        List<GameObject> gameObjects; //Saves all the things you see in the game


        /// <summary>
        /// Saves all the references to external content
        /// </summary>
        public const string PlayerTexture1 = "/Content/viking1.png";
        public const string Shadowbolttexture1 = "/Content/viking1.png";
        public const string Fireballtexture1 = "/Content/fireball.png";
        public const string Beamtexture1 = "/Content/Beam.png";
        public const string Castingtexture1 = "/Content/fireball.png";
        public const string Pixeltexture = "/Content/pixel.png";
        public const string MenuButtonTexture1 = "/Content/menubutton1.png";
        public const string MenuButtonTexture2 = "/Content/menubutton2.png";
        public const string MenuButtonTexture3 = "/Content/menubutton3.png";
        public const string GameFont = "/Content/GameFont.xml";


        Menuscreen mainmenu; //The menu class

        public Game1() //Runs when the game starts, before the initalize method
        {
            graphics = new GraphicsDeviceManager(this); //Needed to draw things to screen
            gameObjects = new List<GameObject>(); //initilizes the list
            Content.RootDirectory = "Content"; //Specifies what folder the external content is located in
            graphics.PreferredBackBufferHeight = 720; //sets the height of the window
            graphics.PreferredBackBufferWidth = 1080;//sets the width of the window
        }

        protected override void Initialize() //Runs when the game starts, after the constructor
        {

            //Flatredball stuff
            Renderer.UseRenderTargets = false;
            FlatRedBallServices.InitializeFlatRedBall(this, graphics);
            FlatRedBallServices.Game.IsMouseVisible = true;

            //Creates the menuclass
            mainmenu = new Menuscreen(this, null, gameObjects);
            gameObjects.Add(mainmenu);
            //Creates the global data
            GlobalData.GlobalData.Initialize();
            
            
            //Content.RootDirectory = "Content"; 

            GlobalData.GlobalData.keyboardDispatcher = new KeyboardDispatcher(this.Window); //Initializes the 
            
            //GlobalData.GlobalData.Font = Content.Load<SpriteFont>(GameFont);
                
            //ScreenManager.Start(typeof(SomeScreen).FullName);
               
            base.Initialize();
        }

        

        protected override void Update(GameTime gameTime) //Runs once every frame
        {
            FlatRedBallServices.Update(gameTime); //does flatredball stuff


            foreach (GameObject g in new List<GameObject>(gameObjects)) //Runs the update method in all 
            {
                g.Update(gameObjects);
            }

            FlatRedBall.Screens.ScreenManager.Activity(); //Again, flatredball stuff

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            FlatRedBallServices.Draw(); //Draws everything in the spritemanager

            base.Draw(gameTime);
        }














    }


    /* Might be relevant at a later stage    
    #if WINDOWS_PHONE || ANDROID || IOS

    // Frame rate is 30 fps by default for Windows Phone.
    TargetElapsedTime = TimeSpan.FromTicks(333333);
    graphics.IsFullScreen = true;
    #else
     
    //public enum Gamestate
    //{
    //    Menu,
    //    Playing,
    //} 
    
    //    public Gamestate GameState
    //    {
    //        get { return gamestate; }
    //        set { gamestate = value; }
    //    }
     
    */

}

    






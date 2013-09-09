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
         
         */



        GraphicsDeviceManager graphics;
        List<GameObject> gameObjects;
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
        Gamestate gamestate;
        Sprite dummysprite = null;
        Menuscreen mainmenu;
        Player player1;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            gameObjects = new List<GameObject>();
            Content.RootDirectory = "Content";
			
/*#if WINDOWS_PHONE || ANDROID || IOS

			// Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            graphics.IsFullScreen = true;
#else*/
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferredBackBufferWidth = 1080;
//#endif
        }
        public const float pi = (float)Math.PI;

        protected override void Initialize()
        {
            Renderer.UseRenderTargets = false;
            FlatRedBallServices.InitializeFlatRedBall(this, graphics);
            FlatRedBallServices.Game.IsMouseVisible = true;


            mainmenu = new Menuscreen(this, dummysprite, gameObjects);

            GlobalData.GlobalData.Initialize();
            
            
            Content.RootDirectory = "Content";
            GlobalData.GlobalData.keyboardDispatcher = new KeyboardDispatcher(this.Window);
            //GlobalData.GlobalData.Font = Content.Load<SpriteFont>(GameFont);




                
            //ScreenManager.Start(typeof(SomeScreen).FullName);
               
            base.Initialize();
        }


        
        private void LoadGame() 
        {
            player1 = new Player(this, SpriteManager.AddSprite(PlayerTexture1));
            gameObjects.Add(player1);
            player1.Sprite.ScaleX = 1.3f;
            player1.Sprite.ScaleY = 1.3f;
        }

        

        protected override void Update(GameTime gameTime)
        {
            FlatRedBallServices.Update(gameTime);


            foreach (GameObject g in new List<GameObject>(gameObjects)) 
            {
                g.Update(gameObjects);
            }

            FlatRedBall.Screens.ScreenManager.Activity();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            FlatRedBallServices.Draw();

            base.Draw(gameTime);
        }



        



        //public Player Player1
        //{
        //    get { return player1; }
        //    set { player1 = value; }
        //}



        public Gamestate GameState
        {
            get { return gamestate; }
            set { gamestate = value; }
        }
        
            

        }
            
        public enum Gamestate 
        {
            Menu,
            Playing,
        }


    }

    






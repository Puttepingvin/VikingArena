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
         Fix beam
         Update net code
         * abstract send in actor classes
         Update Menu
         Display scores
         Save r^2
         connectionscreen
         * rework projectilequeue
         */


        GraphicsDeviceManager graphics; //Needed to draw things to screen
        Effect lighting;
        List<GameObject> gameObjects; //Saves all the things you see in the game





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

            SpriteManager.Camera.UsePixelCoordinates();

            //Creates the menuclass
            mainmenu = new Menuscreen(this, null, gameObjects);
            gameObjects.Add(mainmenu);
            //Creates the global data
            GlobalData.GlobalData.Initialize(this);

            lighting = Content.Load<Effect>("Lighting");
            
            //Content.RootDirectory = "Content"; 

            GlobalData.GlobalData.keyboardDispatcher = new KeyboardDispatcher(this.Window); //Initializes the 


            //Sprite testlight = SpriteManager.AddManagedInvisibleSprite();
            //testlight.Texture = FlatRedBallServices.Load<Texture2D>("/content/blue.jpg");
            //SpriteManager.AddDrawableBatch(new CustomSpriteDrawableBatch(testlight, GlobalData.GlobalData.Lighting));
            //float texturePixelWidth = testlight.Texture.Width;
            //float texturePixelHeight = testlight.Texture.Height;

            ////Now, we need to find out how many pixels per unit there are in our view at the Sprite's Z position:
            //float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(testlight.Z);

            ////Now, we just have to use those two values to set the scale.
            //testlight.ScaleX = .5f * texturePixelWidth / pixelsPerUnit;
            //testlight.ScaleY = .5f * texturePixelHeight / pixelsPerUnit;
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
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            //spriteBatch.Begin();

            //foreach (Sprite s in SpriteManager.OrderedSprites)
            //{
            //    spriteBatch.Draw(s.Texture, new Vector2(s.Position.X, s.Position.Y), Color.White);
            //}

            //spriteBatch.End();
                        
            
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

    






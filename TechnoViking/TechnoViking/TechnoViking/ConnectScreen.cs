using System;
using System.Collections.Generic;
using System.Linq;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Gui;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Screens;
using FlatRedBall.Utilities;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TechnoViking.GlobalData;

namespace TechnoViking
{
    
    class ConnectScreen : GameObject
    {
        NetworkAgent mAgent;
        string mIP = "";
        double timeout;
        byte tries = 0;
        FlatRedBall.Graphics.Text prompter;
        public ConnectScreen(Sprite sprite, Game game, string ip)
            : base(game, sprite) 
        {         
            mIP = ip;
            timeout = TimeManager.CurrentTime;
            mAgent = new NetworkAgent(AgentRole.Client, "VikingArcade");
            mAgent.Connect(mIP);
            tries++;
            prompter = FlatRedBall.Graphics.TextManager.AddText("Attempting to Connect, Attempt #" + tries + " of 4");
            prompter.Position.Y += 7;
            prompter.Position.X -= 5;
        }
        public override void Update(List<GameObject> gameObjects)
        {
            if (tries > 3)
            {
                StopTrying(gameObjects);
            }
            else if (mAgent.Connections.Count == 1) 
            {
                Creategamescreen(gameObjects);
            }
            else if (timeout + 5 < TimeManager.CurrentTime)
            {
                mAgent.Connect(mIP);
                tries++;
                prompter.DisplayText = "Attempting to Connect, Attempt #" + tries + " of 4";
                timeout = TimeManager.CurrentTime;
            }
        }

        public void Creategamescreen(List<GameObject> gameObjects)
        {
            this.Kill(gameObjects);
            Gamescreen multiplayerarena1 = new Gamescreen(game, null, gameObjects, mAgent);
            gameObjects.Add(multiplayerarena1);
          
        }

        public void StopTrying(List<GameObject> gameObjects)
        {
            this.Kill(gameObjects);
            Menuscreen mainmenu = new Menuscreen(game, null, gameObjects);
            gameObjects.Add(mainmenu);
           
        }

        public override void Kill(List<GameObject> gameObjects)
        {
            
            gameObjects.Remove(this);
            FlatRedBall.Graphics.TextManager.RemoveText(prompter);
        }

        public override void SendState(NetworkAgent mAgent)
        {
            throw new NotImplementedException();
        }
    }
}

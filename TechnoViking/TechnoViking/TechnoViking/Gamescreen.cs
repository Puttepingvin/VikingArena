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
    /// <summary>
    /// BUGS:
    /// Mouse doesn't transfer over net
    /// Collision only works on server
    /// 
    /// DO NEXT:
    /// Angle in collision
    /// 
    /// 
    /// </summary>


    class Gamescreen : GameObject
    {
        NetworkAgent mAgent;
        Player tempplayer;
        const float ticktime = 0.2f;
        const float speedLimit = 20.0f;
        const float accelerationspeed = 30.0f;
        const float breakacc = 20.0f;
        List<Player> PlayerList = new List<Player>();
        List<Player> Aliveplayers = new List<Player>();
        byte PlayerID = 0;
        string mIP = "";
        //byte oldconnectionammount;
        byte Playercount;
        double roundstarted;
        AbilityManager abilitys;
        bool pause;
        double LastTick = 0;
        float lastcollision = 0;

        /*
         Bugs:
         * Mouse wheel doesn't transfer over network
         
         Important TODO:
         * Spell tick
         */

        public Gamescreen(Game game, Sprite sprite, List<GameObject> gameObjects)
            : base(game, sprite) 
        {
            Stop = false;
            StartServerAndClient(gameObjects);
            abilitys = new AbilityManager(game);

        }
        public Gamescreen(Game game, Sprite sprite, List<GameObject> gameObjects, string ip)
            : base(game, sprite)
        {
            Stop = false;
            mIP = ip;
            StartServerAndClient(gameObjects);
            abilitys = new AbilityManager(game);
        }

        /// <summary>
        /// Creates all the player classes and adds them to the proper lists
        /// </summary>
        /// <param name="gameObjects"></param>
        private void CreatePlayers(List<GameObject> gameObjects)
        {
            for (byte i = 0; i < Playercount; i++)
            {

                tempplayer = new Player(game, SpriteManager.AddSprite(Game1.PlayerTexture1));
                tempplayer.Sprite.Position.X += -16 + (i%3*16);
                tempplayer.Sprite.Position.Y -= -14 + ((byte)(i/3) * 14);
                PlayerList.Add(tempplayer);
                gameObjects.Add(PlayerList[PlayerList.Count-1]);
                Aliveplayers.Add(PlayerList[PlayerList.Count-1]);
            }


            AssignPlayerIndices();
        }

        /// <summary>
        /// Gives all players a unique number
        /// </summary>
        private void AssignPlayerIndices()
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerList[i].Playerindex = i;
            }
        }

        /// <summary>
        /// Updates the game screen
        /// </summary>
        /// <param name="gameObjects"></param>
        public override void Update(List<GameObject> gameObjects)
        {
            if (PlayerList.Count > PlayerID) InputCheck();
            
            ServerAndClientActivity(gameObjects);
            foreach (Player p in Aliveplayers) 
            {
                PlayerMovement(p);
                Spellselection(p);
                Spellcasting(gameObjects, p);
                Rotation(p);
                SaveVariables(p);
                Vector3 temp = PlayerList[PlayerID].Sprite.Velocity;
            }
            CollisionActivity(gameObjects);
            abilitys.Update(gameObjects, PlayerList);

            

        }

        /// <summary>
        /// Resets all variables
        /// </summary>
        /// <param name="gameObjects"></param>
        private void StartNewRound(List<GameObject> gameObjects)
        {
            foreach (Player p in PlayerList)
            {
                p.Kill(gameObjects);
            }
            foreach (GameObject g in new List<GameObject>(gameObjects)) 
            {
                if (g is Projectile) 
                {
                    g.Kill(gameObjects);
                }
            }
            PlayerList.Clear();
            Aliveplayers.Clear();
            CreatePlayers(gameObjects);
            if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server)
            {

                foreach (NetConnection Player in mAgent.Connections)
                {
                    mAgent.WriteMessage((byte)MessageType.Action);
                    mAgent.WriteMessage((byte)ActionType.ServerRestart);
                    mAgent.WriteMessage((byte)mAgent.Connections.Count + 1);
                    mAgent.SendMessage(Player, true);
                }
            }
            roundstarted = TimeManager.CurrentTime;
            
        }

        /// <summary>
        /// Takes all the player input and compresses it as far as possible
        /// </summary>
        private void InputCheck()
        {
            //Skapar ett värde beroende på vilka knappar som är intryckta
            PlayerList[PlayerID].oldkeycount = PlayerList[PlayerID].keycount;
            PlayerList[PlayerID].keycount = 0;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.W)) PlayerList[PlayerID].keycount += 1;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.S)) PlayerList[PlayerID].keycount += 2;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.A)) PlayerList[PlayerID].keycount += 4;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.D)) PlayerList[PlayerID].keycount += 8;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.Q)) PlayerList[PlayerID].keycount += 16;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.E)) PlayerList[PlayerID].keycount += 32;
            if (PlayerList[PlayerID].mousestate.LeftButton == ButtonState.Pressed) PlayerList[PlayerID].keycount += 64;
            //8
            if (PlayerList[PlayerID].mousestate.ScrollWheelValue > PlayerList[PlayerID].OldMouseValue)
            {
                PlayerList[PlayerID].wheelup = true;
                PlayerList[PlayerID].wheeldown = false; 
            }
            else if (PlayerList[PlayerID].mousestate.ScrollWheelValue < PlayerList[PlayerID].OldMouseValue)
            {
                PlayerList[PlayerID].wheelup = false;
                PlayerList[PlayerID].wheeldown = true;
            }
            else 
            {
                PlayerList[PlayerID].wheelup = false; 
                PlayerList[PlayerID].wheeldown = false;
            }
            PlayerList[PlayerID].MouseX = GuiManager.Cursor.WorldXAt(PlayerList[PlayerID].Sprite.Z);
            PlayerList[PlayerID].MouseY = GuiManager.Cursor.WorldYAt(PlayerList[PlayerID].Sprite.Z);
            PlayerList[PlayerID].OldMouseValue = PlayerList[PlayerID].mousestate.ScrollWheelValue;
            //Skicka redan här, sen, eventuellt
        }

        /// <summary>
        /// Decides the acceleration of all players, only called serverside.
        /// </summary>
        private void PlayerMovement(Player p)
        {
                p.goalVelocityX = 0;
                p.goalVelocityY = 0;
                if ((p.keycount & 3) == 3)
                {
                    p.goalVelocityY = 0;
                }
                else if ((p.keycount & 1) == 1)
                {
                    p.goalVelocityY = speedLimit;
                }
                else if ((p.keycount & 2) == 2)
                {
                    p.goalVelocityY = -speedLimit;
                }

                if ((p.keycount & 12) == 12)
                {
                    p.goalVelocityX = 0;
                }
                else if ((p.keycount & 4) == 4)
                {
                    p.goalVelocityX = -speedLimit;
                }
                else if ((p.keycount & 8) == 8)
                {
                    p.goalVelocityX = speedLimit;
                }

                

                if (p.goalVelocityX > p.Sprite.Velocity.X + 0.1f)
                {
                    p.Sprite.Acceleration.X = accelerationspeed;
                }

                else if (p.goalVelocityX < p.Sprite.Velocity.X - 0.1f)
                {
                    p.Sprite.Acceleration.X = -accelerationspeed;
                }
                else
                {
                    p.Sprite.Acceleration.X = 0;
                }



                //Y 
                if (p.goalVelocityY > p.Sprite.Velocity.Y + 0.1f)
                {
                    p.Sprite.Acceleration.Y = accelerationspeed;
                }

                else if (p.goalVelocityY < p.Sprite.Velocity.Y - 0.1f)
                {
                    p.Sprite.Acceleration.Y = -accelerationspeed;
                }
                else
                {
                    p.Sprite.Acceleration.Y = 0;
                }

                //if (p.InterPos != Vector3.Zero)
                //{
                //    p.Sprite.Position.X += p.InterPos.X * (ticktime * (float)TimeManager.SecondDifference);
                //    p.Sprite.Position.Y += p.InterPos.Y * (ticktime * (float)TimeManager.SecondDifference);
                //}

        }

        

        /// <summary>
        /// Decides the desired rotationm should be rewritten
        /// </summary>
        private void Rotation(Player p)
        {
           
            if (p.Sprite.Velocity.X != 0 || p.Sprite.Velocity.Y != 0 && !p.RotationLocked)
            {
                p.desiredRotation = (float)Math.Atan2(p.Sprite.Velocity.Y, p.Sprite.Velocity.X) + p.Offset;
            }
            
            if (p.desiredRotation < 0)
            {
                p.desiredRotation += (2 * (float)Math.PI);
            }

            if (p.desiredRotation - p.Sprite.RotationZ < p.RotationSpeed * TimeManager.SecondDifference && p.desiredRotation - p.Sprite.RotationZ > -p.RotationSpeed * TimeManager.SecondDifference)
            {
                p.Sprite.RotationZ = p.desiredRotation;
            }
            else if (p.desiredRotation - p.Sprite.RotationZ < -Math.PI || (p.desiredRotation - p.Sprite.RotationZ >= 0 && p.desiredRotation - p.Sprite.RotationZ <= Math.PI))
            {
                p.Sprite.RotationZ += p.RotationSpeed * TimeManager.SecondDifference;
            }
            else
            {
                p.Sprite.RotationZ -= p.RotationSpeed * TimeManager.SecondDifference;
            }

        }

        private void Spellselection(Player p) 
        {
            if ((((p.keycount & 16) == 16) && !((p.oldkeycount & 16) == 16))) //|| p.mousestate.ScrollWheelValue > p.OldMouseValue)
            {
                if ((int)p.selectedSpell > 0)
                {
                    p.selectedSpell--;
                }
                else p.selectedSpell = (TechnoViking.Player.Spellbook)Enum.GetNames(typeof(TechnoViking.Player.Spellbook)).Length - 1;
            }
            if ((((p.keycount & 32) == 32) && !((p.oldkeycount & 32) == 32))) //|| p.mousestate.ScrollWheelValue > p.OldMouseValue)
            {
                if ((int)p.selectedSpell < Enum.GetNames(typeof(TechnoViking.Player.Spellbook)).Length - 1)
                {
                    p.selectedSpell++;
                }
                else p.selectedSpell = 0;
            }
        }

        /// <summary>
        /// Handles everything related to spellcasting
        /// </summary>
        /// <param name="gameObjects"></param>
        private void Spellcasting(List<GameObject> gameObjects, Player p)
        {
            if ((p.keycount & 64) == 64)
            {
                abilitys.CastAbility((int)p.selectedSpell, gameObjects, p.MouseX, p.MouseY, p);
            }
        }

        /// <summary>
        /// Checks all relevant collisions, only called serverside
        /// </summary>
        /// <param name="gameObjects"></param>
        private void CollisionActivity(List<GameObject> gameObjects)
        {

            foreach (Player pl in new List<Player>(PlayerList))
            {
                foreach (GameObject g in new List<GameObject>(gameObjects))
                {
                    
                        if (g is Projectile)
                        {
                            if (pl.CircleCollidesWith(g))
                            {
                                pl.Kill(gameObjects);
                                g.Kill(gameObjects);
                                Aliveplayers.Remove(pl);

                            }
                        }
                        if (g is Player && g != pl)
                        {
                            if (pl.CircleCollidesWith(g))
                            {
                                if (pl.Sprite.Velocity != Vector3.Zero || g.Sprite.Velocity != Vector3.Zero) Bounce(pl, g);
                            }
                            
                        }
                    
                }
            }

        }

        private void Bounce(GameObject pl, GameObject g) 
        {
            Vector3 adjustment = g.Sprite.Velocity;

            g.Sprite.Velocity -= adjustment;
            pl.Sprite.Velocity -= adjustment;

            double V1 = pl.Sprite.Velocity.Length();
            double V2 = 0;
            Vector3 posdiff = pl.Sprite.Position - g.Sprite.Position;
            //float temp = posdiff.X / pl.Sprite.Velocity.X;
            float angle = (float)Math.Atan2(pl.Sprite.Velocity.Y, pl.Sprite.Velocity.X);

            float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(pl.Sprite.Position.Z);
            pl.Sprite.Position.X -= ((((pl.Sprite.Width / pixelsPerUnit) + (g.Sprite.Width+1 / pixelsPerUnit)) - posdiff.Length()) / 2) * (float)Math.Cos(angle);
            pl.Sprite.Position.Y -= ((((pl.Sprite.Width / pixelsPerUnit) + (g.Sprite.Width+1 / pixelsPerUnit)) - posdiff.Length()) / 2) * (float)Math.Sin(angle);
            g.Sprite.Position.X += ((((pl.Sprite.Width / pixelsPerUnit) + (g.Sprite.Width+1 / pixelsPerUnit)) - posdiff.Length()) / 2) * (float)Math.Cos(angle);
            g.Sprite.Position.Y += ((((pl.Sprite.Width / pixelsPerUnit) + (g.Sprite.Width+1 / pixelsPerUnit)) - posdiff.Length()) / 2) * (float)Math.Sin(angle);
            posdiff = pl.Sprite.Position - g.Sprite.Position;

            double a = pl.Sprite.Velocity.Length();
            double b = posdiff.Length();
            Vector3 triangle3 = g.Sprite.Position - (pl.Sprite.Position + pl.Sprite.Velocity);
            double c = triangle3.Length();
            double theta = 0;
            if (a* b!=0){
                theta = 2 * Math.PI - Math.Acos((a * a + b * b - c * c) / (2 * a * b));
                if ((a * a + b * b - c * c) / (2 * a * b) > 1)
                {
                    theta = 2 * Math.PI;
                }
            }

            double M1 = pl.Mass;
            double M2 = g.Mass;
            double A1;
            double A2;
            
            V2 = ((2 * M1 * Math.Cos(theta)) * V1) / (M1 + M2);
            V1 = (Math.Sqrt(M1 * M1 + M2 * M2 - 2 * M1 * M2 * Math.Cos(2 * theta)) * V1) / (M1 + M2);
            A1 = angle + Math.Atan2(M1 - M2 * Math.Cos(2 * theta) , M2 * Math.Sin(2 * theta));
            A2 = theta + angle;




            pl.Sprite.Velocity.X = (float)(V1 * Math.Cos(A1));
            pl.Sprite.Velocity.Y = (float)(V1 * Math.Sin(A1));
            g.Sprite.Velocity.X = (float)(V2 * Math.Cos(A2));
            g.Sprite.Velocity.Y = (float)(V2 * Math.Sin(A2));

            pl.Sprite.Velocity += adjustment;
            g.Sprite.Velocity += adjustment;
        }

        /// <summary>
        /// Unneccecary right now but might become useful later, keeping for now
        /// </summary>
        /// <param name="p"></param>
        private void SaveVariables(Player p) 
        {
            p.oldkeycount = p.keycount;
        }



        /// <summary>
        /// Inililizes the netcode
        /// </summary>
        /// <param name="gameObjects"></param>
        private void StartServerAndClient(List<GameObject> gameObjects)
        {
            if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server)
            {
                mAgent = new NetworkAgent(AgentRole.Server, "VikingArcade");
                mAgent.forwardport();
            }
            else if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Client)
            {
                mAgent = new NetworkAgent(AgentRole.Client, "VikingArcade");
                //mAgent.forwardport();
                mAgent.Connect(mIP);
                Stop = false;
                double timeout = TimeManager.CurrentTime;
                int i = 0;
                while (mAgent.Connections.Count == 0 && !Stop) 
                {
                    if (i >= 30) 
                    {
                        Stop = true;
                        
                    }
                    System.Threading.Thread.Sleep(100);
                    i++;
                }


                if (!Stop)
                {
                    mAgent.WriteMessage((byte)MessageType.Action);
                    mAgent.WriteMessage((byte)ActionType.ServerRestart);
                    mAgent.SendMessage(mAgent.Connections[0], true);
                }
                
            }
        }

        /// <summary>
        /// Called every frame to handle net code
        /// </summary>
        /// <param name="gameObjects"></param>
        private void ServerAndClientActivity(List<GameObject> gameObjects)
        {
            if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server)
            {
                ServerActivity(gameObjects);
                Playercount = (byte)(mAgent.Connections.Count + 1); //?
            }
            else if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Client)
            {
                ClientActivity(gameObjects);
            }
            else
            {
                //Local activity
            }
        }
        private void ClientActivity(List<GameObject> gameObjects)
            {
                //SEND INPUT
                if (mAgent.IsPlayerConnected)
                {
                    if (PlayerList.Count > PlayerID)
                    {
                        if (!(PlayerList[PlayerID].keycount == PlayerList[PlayerID].oldkeycount) || LastTick + ticktime < TimeManager.CurrentTime)
                        {
                        mAgent.WriteMessage(PlayerID);
                        mAgent.WriteMessage((byte)PlayerList[PlayerID].keycount);
                        if ((PlayerList[PlayerID].keycount & 64) == 64)
                        {
                            mAgent.WriteMessage(PlayerList[PlayerID].MouseX);
                            mAgent.WriteMessage(PlayerList[PlayerID].MouseY);
                        }
                        mAgent.WriteMessage(PlayerList[PlayerID].wheelup);
                        mAgent.WriteMessage(PlayerList[PlayerID].wheeldown);
                        mAgent.SendMessage(mAgent.Connections[0]);
                        }
                        if (LastTick + ticktime < TimeManager.CurrentTime) 
                        {
                            LastTick = TimeManager.CurrentTime;
                        }
                    }
                }

                //RECEIVE MESSAGE AND UPDATE POSITIONS
	            List<NetIncomingMessage> incomingMessages;
                incomingMessages = mAgent.CheckForMessages();
                foreach (NetIncomingMessage incomingMessage in incomingMessages)
                {
                    if (incomingMessage.MessageType == NetIncomingMessageType.Data)
                {
                // server sent a data message
                byte type = incomingMessage.ReadByte();
                for (byte i = 0; i < PlayerList.Count; i++)
                {

                    if (type == i && i != PlayerID)
                    {
                        PlayerList[i].keycount = incomingMessage.ReadByte();
                        if ((PlayerList[i].keycount & 64) == 64)
                        {
                            PlayerList[i].MouseX = incomingMessage.ReadFloat();
                            PlayerList[i].MouseY = incomingMessage.ReadFloat();
                        }
                        PlayerList[i].wheelup = incomingMessage.ReadBoolean();
                        PlayerList[i].wheeldown = incomingMessage.ReadBoolean();
                    }
                }
                switch (type)
                {
                    case (byte)MessageType.Collision:
                        byte playerindex = incomingMessage.ReadByte();
                        if (roundstarted + 1 < TimeManager.CurrentTime)
                        {
                            PlayerList[playerindex].Kill(gameObjects);
                            foreach (GameObject g in new List<GameObject>(gameObjects))
                            {
                                if (g is Actor)
                                {
                                    if (g.CircleCollidesWith(PlayerList[playerindex]))
                                    {
                                        g.Kill(gameObjects);
                                    }
                                }
                            }
                            Aliveplayers.Remove(PlayerList[playerindex]);
                        }
                    
                    

                    break;

                case (byte)MessageType.Action:
                    switch (incomingMessage.ReadByte()) 
                    {
                        case (byte)ActionType.ServerRestart:
                            Playercount = incomingMessage.ReadByte();
                            StartNewRound(gameObjects);
                        break;
                    }
                    break;

                    case (byte)MessageType.Tick:
                    ReadActor(incomingMessage);


                        break;
                //case (byte)MessageType.Scores:
                //    ScoreHudInstance.Score1 = incomingMessage.ReadByte();
                //    ScoreHudInstance.Score2 = incomingMessage.ReadByte();
                //    break;

                //case (byte)MessageType.Puck:
                //    PuckInstance.X = incomingMessage.ReadFloat();
                //    PuckInstance.Y = incomingMessage.ReadFloat();
                //    break;

                case (byte)MessageType.PlayerID:
                    PlayerID = incomingMessage.ReadByte();
                    break;
            }
        }
    }

        }
        private void ServerActivity(List<GameObject> gameObjects)
        {
            List<NetIncomingMessage> incomingMessages; 
            incomingMessages = mAgent.CheckForMessages();foreach(NetIncomingMessage incomingMessage in incomingMessages)
	    
		    if (incomingMessage.MessageType == NetIncomingMessageType.Data)
		    {
                byte type = incomingMessage.ReadByte();
                        for (byte i = 0; i < PlayerList.Count; i++)
                        {
                            if (type == i)
                            {
                                PlayerList[i].keycount = incomingMessage.ReadByte();
                                if ((PlayerList[i].keycount & 64) == 64)
                                {
                                    PlayerList[i].MouseX = incomingMessage.ReadFloat();
                                    PlayerList[i].MouseY = incomingMessage.ReadFloat();
                                }
                                PlayerList[i].wheelup = incomingMessage.ReadBoolean();
                                PlayerList[i].wheeldown = incomingMessage.ReadBoolean();
                            }
                        }
                switch (type)
                {
                    case (byte)MessageType.Action:
                        switch (incomingMessage.ReadByte())
                        {
                            case (byte)ActionType.ServerRestart:
                                StartNewRound(gameObjects);
                                break;
                        }
                        break;
                }
		    }
	        
            //Server only logic
            if (Aliveplayers.Count < 2 && mAgent.Connections.Count > 0)
            {
                StartNewRound(gameObjects);
            }
            
            //Send the message to each player (client)
            foreach (NetConnection Player in mAgent.Connections)
            {
                for (byte i = 0; i < PlayerList.Count; i++)
                {
                    
                    if (!(PlayerList[i].keycount == PlayerList[i].oldkeycount))
                    {
                        mAgent.WriteMessage(i);
                        mAgent.WriteMessage((byte)PlayerList[i].keycount);
                        if ((PlayerList[i].keycount & 64) == 64)
                        {
                            mAgent.WriteMessage(PlayerList[i].MouseX);
                            mAgent.WriteMessage(PlayerList[i].MouseY);
                        }
                        mAgent.WriteMessage(PlayerList[i].wheelup);
                        mAgent.WriteMessage(PlayerList[i].wheeldown);
                        mAgent.SendMessage(Player);
                    }
                }

                //Tick
                if (LastTick + ticktime < TimeManager.CurrentTime) 
                {
                    mAgent.WriteMessage((byte)MessageType.Tick);
                    foreach (GameObject g in gameObjects) 
                    {
                        if (g is Actor) 
                        {
                            g.SendState(mAgent);
                        }
                    }
                    mAgent.WriteMessage((byte)MessageType.Stop);
                    mAgent.SendMessage(Player);
                    LastTick = TimeManager.CurrentTime;
                }
                //if (sendspell)
                //{
                //    mAgent.WriteMessage((byte)MessageType.Spell);
                //    mAgent.WriteMessage(PlayerID);
                //    mAgent.WriteMessage((Int16)selectedSpell);
                //    mAgent.WriteMessage(worldX);
                //    mAgent.WriteMessage(worldY);
                //    mAgent.SendMessage(Player);
                //}


                //mAgent.WriteMessage((byte)MessageType.Scores);
                //mAgent.WriteMessage((byte)ScoreHudInstance.Score1);
                //mAgent.WriteMessage((byte)ScoreHudInstance.Score2);
                //mAgent.SendMessage(player);
            }
        }

        private void ReadActor(NetIncomingMessage incomingMessage) 
        {
            byte actor = incomingMessage.ReadByte();
            for (int i = 0; i < PlayerList.Count; i++)
            {
                if (i == actor)
                {
                    bool old = false;
                    float tX = incomingMessage.ReadFloat();
                    float tY = incomingMessage.ReadFloat();
                    float tVX = incomingMessage.ReadFloat();
                    float tVY = incomingMessage.ReadFloat();

                    foreach (Vector2 pos in PlayerList[i].PrevPos) 
                    {
                        if (pos.X == tX && pos.Y == tY)
                        {
                            old = true;
                            break;
                        }
                    }

                    if (!old)
                    {

                        PlayerList[i].Sprite.Position = new Vector3(tX, tY, 0);
                        PlayerList[i].Sprite.Velocity.X = tVX;
                        PlayerList[i].Sprite.Velocity.Y = tVY;
                    }
                    ReadActor(incomingMessage);
                }
            } 
            switch (actor) 
            {
                case (byte)MessageType.Spell:
                //mAgent.WriteMessage((byte)GlobalData.MessageType.Spell);
                //mAgent.WriteMessage((byte)selectedSpell);
                //mAgent.WriteMessage(angle);
                //mAgent.WriteMessage(sprite.Position.X);
                //mAgent.WriteMessage(sprite.Position.Y);
                   // mAgent.WriteMessage(playerID);
                    byte spellID = incomingMessage.ReadByte();
                    float angle = incomingMessage.ReadFloat();
                    float tX = incomingMessage.ReadFloat();
                    float tY = incomingMessage.ReadFloat();
                    byte spellcaster = incomingMessage.ReadByte();
                    ReadActor(incomingMessage);
                    break;
                case (byte)MessageType.Stop:
                    //End of tick
                    break;
            }

   //81.230.67.177

        }


        public void Pausecheck() 
        {
            if (FlatRedBall.Input.InputManager.Keyboard.KeyPushed(Keys.Escape)) 
            {
                //pause = true;
                //add menubuttons, manualy update?
                //Add a semi-transparent layer
            }


        } //implement

        /// <summary>
        /// Unloads the gamescreen
        /// </summary>
        /// <param name="gameObjects"></param>
        public override void Kill(List<GameObject> gameObjects) 
        {
            mAgent.Shutdown();
            foreach (GameObject g in new List<GameObject>(gameObjects))
            {
                if (g != this)
                {
                    g.Kill(gameObjects);
                }
            }
            gameObjects.Remove(this);
        }

        public override void SendState(NetworkAgent mAgent)
        {
        }

        /// <summary>
        /// For easy spell handling
        /// </summary>


        /// <summary>
        /// If connection fails
        /// </summary>
        public bool Stop
        {
            
            get;
            set;
        }
    }
}

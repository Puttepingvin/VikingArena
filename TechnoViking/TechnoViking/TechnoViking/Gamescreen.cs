using System;
using System.Collections.Generic;

using Lidgren.Network;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Utilities;
using TechnoViking.GlobalData;


using Microsoft.Xna.Framework;
using System.Linq;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FlatRedBall.Gui;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Screens;
namespace TechnoViking
{
    /// <summary>
    /// BUGS:
    /// Movemeny rotation is broken for clients
    /// 
    /// REWRITE:
    /// Rotation
    /// Server code
    /// 
    /// </summary>
    class Gamescreen : GameObject
    {
        NetworkAgent mAgent;
        Player tempplayer;
        const float speedLimit = 20.0f;
        const float accelerationspeed = 30.0f;
        const float breakacc = 20.0f;
        List<Player> PlayerList = new List<Player>();
        List<Player> Aliveplayers = new List<Player>();
        byte PlayerID = 0;
        bool edown = false;
        float scrollvalue = 0.0f;
        private bool mouseup;
        private Spellbook selectedSpell = Spellbook.shadowbolt;
        private List<int> scores = new List<int>();
        bool sendspell;
        string mIP = "";
        //byte oldconnectionammount;
        byte Playercount;
        double roundstarted;
        AbilityManager abilitys;
        float worldX;
        float worldY;
        bool pause;
        

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
                gameObjects.Add(tempplayer);
                PlayerList.Add(tempplayer);
                Aliveplayers.Add(tempplayer);
            }


            //player2 = new Player(game, SpriteManager.AddSprite(Game1.PlayerTexture1));
            //player2.Sprite.Position.X -= 8;
            //gameObjects.Add(player2);
            //PlayerList.Add(player2);
            //Aliveplayers.Add(player2);

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
            if (PlayerList.Count > 0) //if there are any players in the game
            {
                InputCheck();
                if (!PlayerList[PlayerID].RotationLocked) 
                {
                    Rotation();
                }
                Spellcasting(gameObjects);
                abilitys.Update(gameObjects, PlayerList);
            }
            ServerAndClientActivity(gameObjects);


            

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
        /// Checks key input and decides the desired velocity
        /// </summary>
        private void InputCheck()
        {
            //Skapar ett värde mellan 0-15 beroende på vilka knappar som är intryckta
            PlayerList[PlayerID].oldkeycount = PlayerList[PlayerID].keycount;
            PlayerList[PlayerID].keycount = 0;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.W)) PlayerList[PlayerID].keycount += 1;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.S)) PlayerList[PlayerID].keycount += 2;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.A)) PlayerList[PlayerID].keycount += 4;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.D)) PlayerList[PlayerID].keycount += 8;

            //Skicka redan här, sen, eventuellt
        }

        /// <summary>
        /// Decides the acceleration of all players, only called serverside.
        /// </summary>
        private void playermovement(Player p)
        {

            
            //X


                switch (p.keycount)
                {
                    case 0:
                        p.goalVelocityX = 0;
                        p.goalVelocityY = 0;
                        break;

                    case 1:
                        p.goalVelocityY = speedLimit;
                        p.goalVelocityX = 0;
                        break;

                    case 2:
                        p.goalVelocityY = -speedLimit;
                        p.goalVelocityX = 0;
                        break;

                    case 4:
                        p.goalVelocityX = -speedLimit;
                        p.goalVelocityY = 0;
                        break;

                    case 8:
                        p.goalVelocityX = speedLimit;
                        p.goalVelocityY = 0;
                        break;
                    case 5:

                        p.goalVelocityY = speedLimit * 0.7f;

                        p.goalVelocityX = -speedLimit * 0.7f;

                        break;
                    case 6:
                        p.goalVelocityY = -speedLimit * 0.7f;
                        p.goalVelocityX = -speedLimit * 0.7f;
                        break;
                    case 9:
                        p.goalVelocityY = speedLimit * 0.7f;
                        p.goalVelocityX = speedLimit * 0.7f;
                        break;
                    case 10:
                        p.goalVelocityY = -speedLimit * 0.7f;
                        p.goalVelocityX = speedLimit * 0.7f;
                        break;
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
            
        }

        /// <summary>
        /// Decides the desired rotationm should be rewritten
        /// </summary>
        private void Rotation()
        {
            
            //if (PlayerList[PlayerID].mousestate.LeftButton == ButtonState.Pressed)
            //{
            //    worldX = GuiManager.Cursor.WorldXAt(PlayerList[PlayerID].Sprite.Z);
            //    worldY = GuiManager.Cursor.WorldYAt(PlayerList[PlayerID].Sprite.Z);

            //    PlayerList[PlayerID].desiredRotation = (float)Math.Atan2(
            //    worldY - PlayerList[PlayerID].Sprite.Y, worldX - PlayerList[PlayerID].Sprite.X);
            //    // Sätt rotationen
            //}

            if (PlayerList[PlayerID].Sprite.Velocity.X != 0 || PlayerList[PlayerID].Sprite.Velocity.Y != 0)
                {
                   PlayerList[PlayerID].desiredRotation = (float)Math.Atan2(PlayerList[PlayerID].Sprite.Velocity.Y, PlayerList[PlayerID].Sprite.Velocity.X) + PlayerList[PlayerID].Offset;
                }
            }

        /// <summary>
        /// Handles everything related to spellcasting
        /// </summary>
        /// <param name="gameObjects"></param>
        private void Spellcasting(List<GameObject> gameObjects)
        {




            if (PlayerList[PlayerID].mousestate.ScrollWheelValue > scrollvalue || (PlayerList[PlayerID].keystate.IsKeyDown(Keys.E) && !edown))
            {
                edown = true;
                if ((int)selectedSpell < Enum.GetNames(typeof(Spellbook)).Length - 1)
                {
                    selectedSpell++;
                }
                else selectedSpell = 0;
            }
            else if (PlayerList[PlayerID].keystate.IsKeyUp(Keys.E)) edown = false;

            if (PlayerList[PlayerID].mousestate.ScrollWheelValue < scrollvalue)
            {
                if ((int)selectedSpell > 0)
                {
                    selectedSpell--;
                }
                else selectedSpell = (Spellbook)Enum.GetNames(typeof(Spellbook)).Length - 1;
            }

            scrollvalue = PlayerList[PlayerID].mousestate.ScrollWheelValue;

            if (PlayerList[PlayerID].mousestate.LeftButton == ButtonState.Pressed && mouseup == true)
            {
                worldX = GuiManager.Cursor.WorldXAt(PlayerList[PlayerID].Sprite.Z);
                worldY = GuiManager.Cursor.WorldYAt(PlayerList[PlayerID].Sprite.Z);
                
                mouseup = false;
                abilitys.CastAbility((int)selectedSpell, gameObjects, worldX, worldY, PlayerList[PlayerID]);
                    sendspell = true;
                
                    

                //player1.Castspell((int)selectedSpell, gameObjects);
            }
            else if (PlayerList[PlayerID].mousestate.LeftButton == ButtonState.Released)
            {
                mouseup = true;
                sendspell = false;
            }
            else sendspell = false;
        }

        /// <summary>
        /// Sets the rotation of all sprites, only called serverside
        /// </summary>
        private void Serverrotation() 
        {
            

            foreach (Player p in PlayerList) 
            {
                if (p.Sprite.Velocity.X != 0 || p.Sprite.Velocity.Y != 0)
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

        }

        /// <summary>
        /// Checks all relevant collisions, only called serverside
        /// </summary>
        /// <param name="gameObjects"></param>
        private void CollisionActivity(List<GameObject> gameObjects)
        {

            foreach (Player pl in new List<Player>(Aliveplayers))
            {
                foreach (GameObject g in new List<GameObject>(gameObjects))
                {
                    if (g is Projectile)
                    {
                        if (pl.CircleCollidesWith(g))
                        {
                                SendCollision((byte)pl.Playerindex);
                                pl.Kill(gameObjects);
                                g.Kill(gameObjects);
                                Aliveplayers.Remove(pl);
                            
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Sends a collision to clients, only called serverside
        /// </summary>
        /// <param name="playerindex"></param>
        private void SendCollision(byte playerindex)
        {
            foreach (NetConnection Player in mAgent.Connections)
            {
                mAgent.WriteMessage((byte)MessageType.Collision);
                mAgent.WriteMessage(playerindex);
                mAgent.SendMessage(Player);
            }
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
                Playercount = (byte)(mAgent.Connections.Count + 1);
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
                if (PlayerList.Count > PlayerID)
                {
                    playermovement(PlayerList[PlayerID]);
                }
                //SEND INPUT
                if (mAgent.IsPlayerConnected)
                {
                    if (PlayerList.Count > PlayerID)
                    {
                        if (!(PlayerList[PlayerID].keycount == PlayerList[PlayerID].oldkeycount))
                        {
                        mAgent.WriteMessage(PlayerID);
                        mAgent.WriteMessage((byte)PlayerList[PlayerID].keycount); //|| keycount >= mousepressnumber
                            // if keycount >= mousepress number
                            // send mouse position
                            // can you trust the server to be up to date on selected spell?
                            // probably not, but the server is king, so just change the selected spell on tic?
                            //
                        //mAgent.WriteMessage(PlayerList[PlayerID].desiredRotation);
                        mAgent.SendMessage(mAgent.Connections[0]);
                        }
                    }

                    if (sendspell)
                    {
                        mAgent.WriteMessage((byte)MessageType.Spell);
                        mAgent.WriteMessage(PlayerID);
                        mAgent.WriteMessage((Int16)selectedSpell);
                        mAgent.WriteMessage(worldX);
                        mAgent.WriteMessage(worldY);
                        mAgent.SendMessage(mAgent.Connections[0]);
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
                    if (type == i)
                    {
                        PlayerList[i].Sprite.X = incomingMessage.ReadFloat();
                        PlayerList[i].Sprite.Y = incomingMessage.ReadFloat();
                        PlayerList[i].Sprite.RotationZ = incomingMessage.ReadFloat();
                    }
                }
                switch (type)
                {
                case (byte)MessageType.Spell:
                        byte tPlayerID = incomingMessage.ReadByte();
                        if (tPlayerID != PlayerID)
                        {
                            abilitys.CastAbility(incomingMessage.ReadInt16(), gameObjects, incomingMessage.ReadFloat(), incomingMessage.ReadFloat(), PlayerList[tPlayerID]);
                        }
                    break;

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

                                //PlayerList[i].desiredRotation = incomingMessage.ReadFloat();

                            }
                        }
                switch (type)
                {
                        //för mycket, måste gå att komprimera
                    case (byte)MessageType.Spell:
                        byte tPlayerID = incomingMessage.ReadByte();
                        Int16 tSpellselect = incomingMessage.ReadInt16();
                        float tWorldX = incomingMessage.ReadFloat();
                        float tWorldY = incomingMessage.ReadFloat();
                        abilitys.CastAbility(tSpellselect, gameObjects, tWorldX, tWorldY, PlayerList[tPlayerID]);
                        foreach (NetConnection Player in mAgent.Connections)
                        {
                            mAgent.WriteMessage((byte)MessageType.Spell);
                            mAgent.WriteMessage(tPlayerID);
                            mAgent.WriteMessage(tSpellselect);
                            mAgent.WriteMessage(tWorldX);
                            mAgent.WriteMessage(tWorldY);
                            mAgent.SendMessage(Player);
                        }
                        ////If dashed was pressed
                        //if (incomingMessage.ReadBoolean())
                        //{
                        //    PlayerList[(byte)MessageType.Player2].Speed = incomingMessage.ReadFloat();
                        //    PlayerList[(byte)MessageType.Player2].Angle = incomingMessage.ReadFloat();
                        //    PlayerList[(byte)MessageType.Player2].Dash();
                        //}
                        break;
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
	    

	        //PHYSICS AND OTHER LOGIC
            if (PlayerList.Count > 0)
            {
                foreach (Player p in PlayerList)
                {
                    playermovement(p);
                }
            }
            Serverrotation();
            
            CollisionActivity(gameObjects);
            if (Aliveplayers.Count < 2 && mAgent.Connections.Count > 0)
            {
                StartNewRound(gameObjects);
            }
            //Send the message to each player (client)
            foreach (NetConnection Player in mAgent.Connections)
            {
                for (byte i = 0; i < PlayerList.Count; i++)
                {
                    //Send a separate message for each object
                    mAgent.WriteMessage(i);
                    mAgent.WriteMessage(PlayerList[i].Sprite.X);
                    mAgent.WriteMessage(PlayerList[i].Sprite.Y);
                    mAgent.WriteMessage(PlayerList[i].Sprite.RotationZ);
                    //mAgent.WriteMessage(PlayerList[(byte)MessageType.Player1].CooldownCircleRadius);
                    //mAgent.WriteMessage(PlayerList[(byte)MessageType.Player1].DashPressed);
                    mAgent.SendMessage(Player);
                }

                //mAgent.WriteMessage((byte)MessageType.Player2);
                //mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].Sprite.X);
                //mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].Sprite.Y);
                //mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].Sprite.RotationZ);
                ////mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].CooldownCircleRadius);
                ////mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].DashPressed);
                //mAgent.SendMessage(Player);
                

                if (sendspell)
                {
                    mAgent.WriteMessage((byte)MessageType.Spell);
                    mAgent.WriteMessage(PlayerID);
                    mAgent.WriteMessage((Int16)selectedSpell);
                    mAgent.WriteMessage(worldX);
                    mAgent.WriteMessage(worldY);
                    mAgent.SendMessage(Player);
                }


                //mAgent.WriteMessage((byte)MessageType.Scores);
                //mAgent.WriteMessage((byte)ScoreHudInstance.Score1);
                //mAgent.WriteMessage((byte)ScoreHudInstance.Score2);
                //mAgent.SendMessage(player);
            }
        }

        public void Pausecheck() 
        {
            if (FlatRedBall.Input.InputManager.Keyboard.KeyPushed(Keys.Escape)) 
            {
                //pause = true;
                //add menubuttons, manualy update?
                //Add a semi-transparent layer
            }


        }

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
       
        /// <summary>
        /// For easy spell handling
        /// </summary>
        enum Spellbook : int
        {
            shadowbolt,
            fireball,
            meadbeam,
        }

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

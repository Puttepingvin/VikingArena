using System;
using System.Collections.Generic;

using Lidgren.Network;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Utilities;
using TechnoViking.GlobalData;


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
    //Leta upp vart koden där den nya spelaren connectar ligger och cleara projektillistan där
    class Gamescreen : GameObject
    {
        NetworkAgent mAgent;
        Player player1;
        Player player2;
        float offset = (float)Math.PI / 2.0f;
        const float speedLimit = 20.0f;
        const float accelerationspeed = 30.0f;
        const float breakacc = 20.0f;
        List<Player> PlayerList = new List<Player>();
        List<Player> Aliveplayers = new List<Player>();
        List<Projectile> ProjectileList = new List<Projectile>();
        byte PlayerID = 0;
        bool edown = false;
        float scrollvalue = 0.0f;
        private bool mouseup;
        private Spellbook selectedSpell = Spellbook.shadowbolt;
        private List<int> scores = new List<int>();
        bool sendspell;
        float angle;
        string ip = "81.230.67.177";

        public Gamescreen(Game game, Sprite sprite, List<GameObject> gameObjects)
            : base(game, sprite) 
        {
            
            StartServerAndClient();
            CreatePlayers(gameObjects);

        }

        private void CreatePlayers(List<GameObject> gameObjects) 
        {
            player1 = new Player(game, SpriteManager.AddSprite(Game1.PlayerTexture1));
            player1.Sprite.Position.X += 4;
            gameObjects.Add(player1);
            PlayerList.Add(player1);
            Aliveplayers.Add(player1);


            player2 = new Player(game, SpriteManager.AddSprite(Game1.PlayerTexture1));
            player2.Sprite.Position.X -= 4;
            gameObjects.Add(player2);
            PlayerList.Add(player2);
            Aliveplayers.Add(player2);

            AssignPlayerIndices();
        }

        private void AssignPlayerIndices()
        {
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerList[i].Playerindex = i;
            }
        }

        public override void Update(List<GameObject> gameObjects)
        {
            PlayerMovement();
            Spellcasting(gameObjects);
            ServerAndClientActivity(gameObjects);
            Rotation();
            
                if (Aliveplayers.Count < 2)
                {
                    StartNewRound(gameObjects);
                }
            

        }

        private void StartNewRound(List<GameObject> gameObjects)
        {
            ProjectileList.Clear();
            foreach (Player p in PlayerList)
            {
                p.Kill(gameObjects);
            }
            PlayerList.Clear();
            Aliveplayers.Clear();
            CreatePlayers(gameObjects);
            //if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server)
            //{

            //    foreach (NetConnection Player in mAgent.Connections)
            //    {
            //        mAgent.WriteMessage((byte)MessageType.Action);
            //        mAgent.WriteMessage((byte)ActionType.ServerRestart);
            //        mAgent.SendMessage(Player, true);
            //    }
            //}
            
        }

        private void PlayerMovement()
        {
            //Skapar ett värde mellan 0-15 beroende på vilka knappar som är intryckta
            PlayerList[PlayerID].keycount = 0;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.W)) PlayerList[PlayerID].keycount += 1;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.S)) PlayerList[PlayerID].keycount += 2;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.A)) PlayerList[PlayerID].keycount += 4;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.D)) PlayerList[PlayerID].keycount += 8;

            switch (PlayerList[PlayerID].keycount)
            {
                case 0:
                    PlayerList[PlayerID].goalVelocityX = 0;
                    PlayerList[PlayerID].goalVelocityY = 0;
                    break;

                case 1:
                    PlayerList[PlayerID].goalVelocityY = speedLimit;
                    PlayerList[PlayerID].goalVelocityX = 0;
                    break;

                case 2:
                    PlayerList[PlayerID].goalVelocityY = -speedLimit;
                    PlayerList[PlayerID].goalVelocityX = 0;
                    break;

                case 4:
                    PlayerList[PlayerID].goalVelocityX = -speedLimit;
                    PlayerList[PlayerID].goalVelocityY = 0;
                    break;

                case 8:
                    PlayerList[PlayerID].goalVelocityX = speedLimit;
                    PlayerList[PlayerID].goalVelocityY = 0;
                    break;
                case 5:

                    PlayerList[PlayerID].goalVelocityY = speedLimit * 0.7f;

                    PlayerList[PlayerID].goalVelocityX = -speedLimit * 0.7f;

                    break;
                case 6:
                    PlayerList[PlayerID].goalVelocityY = -speedLimit * 0.7f;
                    PlayerList[PlayerID].goalVelocityX = -speedLimit * 0.7f;
                    break;
                case 9:
                    PlayerList[PlayerID].goalVelocityY = speedLimit * 0.7f;
                    PlayerList[PlayerID].goalVelocityX = speedLimit * 0.7f;
                    break;
                case 10:
                    PlayerList[PlayerID].goalVelocityY = -speedLimit * 0.7f;
                    PlayerList[PlayerID].goalVelocityX = speedLimit * 0.7f;
                    break;
            }
        }

        private void playermovement()
        {
            //if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Client)
            //{
            //    //X
            //    if (PlayerList[PlayerID].goalVelocityX > PlayerList[PlayerID].Sprite.Velocity.X + 0.1f)
            //    {
            //        PlayerList[PlayerID].CurrentXAcceleration = accelerationspeed;
            //    }

            //    else if (PlayerList[PlayerID].goalVelocityX < PlayerList[PlayerID].Sprite.Velocity.X - 0.1f)
            //    {
            //        PlayerList[PlayerID].CurrentXAcceleration = -accelerationspeed;
            //    }
            //    else
            //    {
            //        PlayerList[PlayerID].CurrentXAcceleration = 0;
            //    }

            //    //Y 
            //    if (PlayerList[PlayerID].goalVelocityY > PlayerList[PlayerID].Sprite.Velocity.Y + 0.1f)
            //    {
            //        PlayerList[PlayerID].CurrentYAcceleration = accelerationspeed;
            //    }

            //    else if (PlayerList[PlayerID].goalVelocityY < PlayerList[PlayerID].Sprite.Velocity.Y - 0.1f)
            //    {
            //        PlayerList[PlayerID].CurrentYAcceleration = -accelerationspeed;
            //    }
            //    else
            //    {
            //        PlayerList[PlayerID].CurrentYAcceleration = 0;
            //    }
            //}


            //X
            foreach (Player p in PlayerList)
            {
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
        }

        private void Rotation()
        {
            //Riktning att kolla åt
            if (PlayerList[PlayerID].mousestate.LeftButton == ButtonState.Pressed)
            {
                float worldX = GuiManager.Cursor.WorldXAt(PlayerList[PlayerID].Sprite.Z);
                float worldY = GuiManager.Cursor.WorldYAt(PlayerList[PlayerID].Sprite.Z);

                PlayerList[PlayerID].desiredRotation = (float)Math.Atan2(
                worldY - PlayerList[PlayerID].Sprite.Y, worldX - PlayerList[PlayerID].Sprite.X);

                // Sätt rotationen


            }


            else if (PlayerList[PlayerID].goalVelocityX != 0 || PlayerList[PlayerID].goalVelocityY != 0)
            {
                if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server)
                {
                    PlayerList[PlayerID].desiredRotation = (float)Math.Atan2(PlayerList[PlayerID].Sprite.Velocity.Y, PlayerList[PlayerID].Sprite.Velocity.X);
                }
                else 
                    PlayerList[PlayerID].desiredRotation = float.MinValue;
                
            }
            if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server)
            {
                PlayerList[PlayerID].Sprite.RotationZ = PlayerList[PlayerID].desiredRotation + offset;
            }




        }

        private void Spellcasting(List<GameObject> gameObjects)
        {




            if (player1.mousestate.ScrollWheelValue > scrollvalue || (player1.keystate.IsKeyDown(Keys.E) && !edown))
            {
                edown = true;
                if ((int)selectedSpell < Enum.GetNames(typeof(Spellbook)).Length - 1)
                {
                    selectedSpell++;
                }
                else selectedSpell = 0;
            }
            else if (player1.keystate.IsKeyUp(Keys.E)) edown = false;

            if (player1.mousestate.ScrollWheelValue < scrollvalue)
            {
                if ((int)selectedSpell > 0)
                {
                    selectedSpell--;
                }
                else selectedSpell = (Spellbook)Enum.GetNames(typeof(Spellbook)).Length - 1;
            }

            scrollvalue = player1.mousestate.ScrollWheelValue;

            if (player1.mousestate.LeftButton == ButtonState.Pressed && mouseup == true)
            {
                float worldX = GuiManager.Cursor.WorldXAt(PlayerList[PlayerID].Sprite.Z);
                float worldY = GuiManager.Cursor.WorldYAt(PlayerList[PlayerID].Sprite.Z);
                angle = (float)Math.Atan2(
                worldY - PlayerList[PlayerID].Sprite.Y, worldX - PlayerList[PlayerID].Sprite.X);
                mouseup = false;
               
                    
                    PlayerList[PlayerID].Castspell((int)selectedSpell, gameObjects, angle, ProjectileList);
                    sendspell = true;
                
                    

                //player1.Castspell((int)selectedSpell, gameObjects);
            }
            else if (player1.mousestate.LeftButton == ButtonState.Released)
            {
                mouseup = true;
                sendspell = false;
            }
            else sendspell = false;
        }

        private void CollisionActivity(List<GameObject> gameObjects)
        {

            foreach (Player pl in new List<Player>(Aliveplayers))
            {
                foreach (Projectile pr in new List<Projectile>(ProjectileList))
                {
                   if (pl.CircleCollidesWith(pr))
                   {
                       if (pl.Playerindex != pr.playerID)
                       {
                           SendCollision((byte)pl.Playerindex, (byte)ProjectileList.IndexOf(pr), (byte)pr.playerID);
                           pl.Kill(gameObjects);
                           pr.Kill(gameObjects);
                           ProjectileList.Remove(pr);
                           Aliveplayers.Remove(pl);
                       }
                   }
                }
            }

        }

        private void SendCollision(byte playerindex, byte projectileindex, byte spellcaster)
        {
            foreach (NetConnection Player in mAgent.Connections)
            {
                mAgent.WriteMessage((byte)MessageType.Collision);
                mAgent.WriteMessage(playerindex);
                mAgent.WriteMessage(projectileindex);
                mAgent.WriteMessage(spellcaster);
                mAgent.SendMessage(Player);
            }
        }

        private void StartServerAndClient()
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
                mAgent.Connect(ip);

                System.Threading.Thread.Sleep(500);
                
                mAgent.WriteMessage((byte)MessageType.Action);
                mAgent.WriteMessage((byte)ActionType.ServerRestart);
                mAgent.SendMessage(mAgent.Connections[0], true);
                
                
            }
        }

        private void ServerAndClientActivity(List<GameObject> gameObjects)
        {
            if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server)
            {
                ServerActivity(gameObjects);
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
                    mAgent.WriteMessage(PlayerID);
                    mAgent.WriteMessage(PlayerList[PlayerID].goalVelocityX);
                    mAgent.WriteMessage(PlayerList[PlayerID].goalVelocityY);
                    mAgent.WriteMessage(PlayerList[PlayerID].desiredRotation);
                    //mAgent.WriteMessage(PlayerList[PlayerID].DashPressed);
                    ////mAgent.WriteMessage(PlayerList[PlayerID].Speed);
                    //mAgent.WriteMessage(PlayerList[PlayerID].Sprite.RotationZ);
                    mAgent.SendMessage(mAgent.Connections[0]);

                    if (sendspell)
                    {
                        mAgent.WriteMessage((byte)MessageType.Spell);
                        mAgent.WriteMessage(PlayerID);
                        mAgent.WriteMessage((Int16)selectedSpell);
                        mAgent.WriteMessage(angle);
                        mAgent.SendMessage(mAgent.Connections[0]);
                    }

                }

                ////RECEIVE MESSAGE AND UPDATE POSITIONS
	            List<NetIncomingMessage> incomingMessages;
                incomingMessages = mAgent.CheckForMessages();
        foreach (NetIncomingMessage incomingMessage in incomingMessages)
        {
            if (incomingMessage.MessageType == NetIncomingMessageType.Data)
            {
            // server sent a data message
            byte type = incomingMessage.ReadByte();

                switch (type)
                {
                case (byte)MessageType.Player1:
                    PlayerList[(byte)MessageType.Player1].Sprite.X = incomingMessage.ReadFloat();
                    PlayerList[(byte)MessageType.Player1].Sprite.Y = incomingMessage.ReadFloat();
                    PlayerList[(byte)MessageType.Player1].Sprite.RotationZ = incomingMessage.ReadFloat();
                    //PlayerList[(byte)MessageType.Player1].CooldownCircleRadius = incomingMessage.ReadFloat();
                    //PlayerList[(byte)MessageType.Player1].DashPressed = incomingMessage.ReadBoolean();
                    break;

                case (byte)MessageType.Player2:
                    PlayerList[(byte)MessageType.Player2].Sprite.X = incomingMessage.ReadFloat();
                    PlayerList[(byte)MessageType.Player2].Sprite.Y = incomingMessage.ReadFloat();
                    PlayerList[(byte)MessageType.Player2].Sprite.RotationZ = incomingMessage.ReadFloat();
                    //PlayerBallList[(byte)MessageType.Player2].CooldownCircleRadius = incomingMessage.ReadFloat();
                    //PlayerBallList[(byte)MessageType.Player2].DashPressed = incomingMessage.ReadBoolean();

                    break;

                case (byte)MessageType.Spell:
                    PlayerList[incomingMessage.ReadByte()].Castspell(incomingMessage.ReadInt16(), gameObjects, incomingMessage.ReadFloat(), ProjectileList);
                    break;

                case (byte)MessageType.Collision:
                    byte playerindex = incomingMessage.ReadByte();
                    byte projectileindex = incomingMessage.ReadByte();
                    byte spellcaster = incomingMessage.ReadByte();
                    PlayerList[playerindex].Kill(gameObjects);
                    ProjectileList[projectileindex].Kill(gameObjects);
                    ProjectileList.RemoveAt(projectileindex);
                    Aliveplayers.Remove(PlayerList[playerindex]);
                    break;

                case (byte)MessageType.Action:
                    switch (incomingMessage.ReadByte()) 
                    {
                        case (byte)ActionType.ServerRestart:
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

                switch (type)
                {
                    case (byte)MessageType.Player2:
                        PlayerList[(byte)MessageType.Player2].goalVelocityX = incomingMessage.ReadFloat();
                        PlayerList[(byte)MessageType.Player2].goalVelocityY = incomingMessage.ReadFloat();
                        PlayerList[(byte)MessageType.Player2].desiredRotation = incomingMessage.ReadFloat(); 
                        break;
                    case (byte)MessageType.Spell:
                        PlayerList[incomingMessage.ReadByte()].Castspell(incomingMessage.ReadInt16(), gameObjects, incomingMessage.ReadFloat(), ProjectileList);
                        
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
            playermovement();
            if (PlayerList[(byte)MessageType.Player2].desiredRotation == float.MinValue) //Försvinner när koden uppdaterats till att förutsäga clientside
                PlayerList[(byte)MessageType.Player2].Sprite.RotationZ = (float)Math.Atan2(PlayerList[(byte)MessageType.Player2].Sprite.Velocity.Y, PlayerList[(byte)MessageType.Player2].Sprite.Velocity.X) + offset;
            else PlayerList[(byte)MessageType.Player2].Sprite.RotationZ = PlayerList[(byte)MessageType.Player2].desiredRotation + offset;
            CollisionActivity(gameObjects);

            //Send the message to each player (client)
            foreach (NetConnection Player in mAgent.Connections)
            {
                
                //Send a separate message for each object
                mAgent.WriteMessage((byte)MessageType.Player1);
                mAgent.WriteMessage(PlayerList[(byte)MessageType.Player1].Sprite.X);
                mAgent.WriteMessage(PlayerList[(byte)MessageType.Player1].Sprite.Y);
                mAgent.WriteMessage(PlayerList[(byte)MessageType.Player1].Sprite.RotationZ);
                //mAgent.WriteMessage(PlayerList[(byte)MessageType.Player1].CooldownCircleRadius);
                //mAgent.WriteMessage(PlayerList[(byte)MessageType.Player1].DashPressed);
                mAgent.SendMessage(Player);

                mAgent.WriteMessage((byte)MessageType.Player2);
                mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].Sprite.X);
                mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].Sprite.Y);
                mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].Sprite.RotationZ);
                //mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].CooldownCircleRadius);
                //mAgent.WriteMessage(PlayerList[(byte)MessageType.Player2].DashPressed);
                mAgent.SendMessage(Player);

                if (sendspell)
                {
                    mAgent.WriteMessage((byte)MessageType.Spell);
                    mAgent.WriteMessage(PlayerID);
                    mAgent.WriteMessage((Int16)selectedSpell);
                    mAgent.WriteMessage(angle);
                    mAgent.SendMessage(Player);
                }


                //mAgent.WriteMessage((byte)MessageType.Scores);
                //mAgent.WriteMessage((byte)ScoreHudInstance.Score1);
                //mAgent.WriteMessage((byte)ScoreHudInstance.Score2);
                //mAgent.SendMessage(player);
            }
        }

        public override void Kill(List<GameObject> gameObjects) 
        {
            mAgent.Shutdown();
        }
        
        enum Spellbook : int
        {
            shadowbolt,
            fireball,
            meadbeam,
        }
    }
}

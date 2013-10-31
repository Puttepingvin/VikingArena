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
    /// Interpolation broken!
    /// 
    /// MAYBE
    /// Disconnect
    /// Change Velocity on blink
    /// 
    /// DO NEXT:

    /// More spells, MOAR SPELLS
    /// * Spell tick
    ///
    /// ALWAYS
    /// Better lag-compensation!!!
    /// </summary>


    class Gamescreen : GameObject
    {
        NetworkAgent mAgent;
        Player tempplayer;
        const float ticktime = 0.05f;
        const float speedLimit = 400.0f;
        const float accelerationspeed = 1000.0f;
        const float breakacc = 400.0f;
        List<Player> PlayerList = new List<Player>();
        List<Player> Aliveplayers = new List<Player>();
        byte PlayerID = 0;
        byte Playercount;
        double roundstarted;
        AbilityManager abilitys;
        //bool pause;
        byte mouseover = 0;
        double LastTick = 0;
        private GameState gamestate = GameState.Picking;
        List<MenuButton> PickButtons = new List<MenuButton>();
        byte[] pickedspell = new byte[3] { 0, 0, 0 };
        byte playertexture = 0;
        List<Sprite> cosmetics = new List<Sprite>();
        private enum GameState
        {
            Playing,
            Watching,
            Picking,
            Waiting
        }
        private bool ready;

        public Gamescreen(Game game, Sprite sprite, List<GameObject> gameObjects)
            : base(game, sprite) 
        {
            StartServerAndClient(gameObjects);
            abilitys = new AbilityManager(game);
            LoadPick(gameObjects);

        }
        public Gamescreen(Game game, Sprite sprite, List<GameObject> gameObjects, NetworkAgent Agent)
            : base(game, sprite)
        {
            mAgent = Agent;
            abilitys = new AbilityManager(game);
            LoadPick(gameObjects);
        }


        /// <summary>
        /// Updates the game screen
        /// </summary>
        /// <param name="gameObjects"></param>
        public override void Update(List<GameObject> gameObjects)
        {
            if (gamestate == GameState.Playing)
            {
                InputCheck();
            }
            if (gamestate == GameState.Picking) PickUpdate();
            
            ServerAndClientActivity(gameObjects);

            if (gamestate == GameState.Playing || gamestate == GameState.Watching)
            {
                foreach (Player p in Aliveplayers)
                {
                    PlayerMovement(p);
                    Spellcasting(gameObjects, p);
                    Rotation(p);
                    SaveVariables(p);
                    Vector3 temp = PlayerList[PlayerID].Sprite.Velocity;
                }
                CollisionActivity(gameObjects);
                abilitys.Update(gameObjects, PlayerList);
                //CameraUpdate();
            }
        }

        private void LoadPick(List<GameObject> gameObjects) 
        {
            foreach (NetConnection Player in mAgent.Connections)
            {
                Player.Ready = false;
            }
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
            for (byte i = 0; i < 12; i++) 
            {
                if (i < 9) PickButtons.Add(new MenuButton(game, SpriteManager.AddSprite(Textures.spelltextures[i]), i));
                else PickButtons.Add(new MenuButton(game, SpriteManager.AddSprite(Textures.playertextures[i-9]), i));
                PickButtons[i].Sprite.Position.Y -= i%3 * 100 - 300;
                PickButtons[i].Sprite.Position.X += ((i / 3) * 100) - 150f;
                PickButtons[i].Sprite.Width = 50;
                PickButtons[i].Sprite.Height = 50;
                //for (int n = 0; n < 3; n++) 
                //{
                    
                //}
            }
            for (int i = 0; i <= 9; i += 3)
            {
                cosmetics.Add(new Sprite());
                cosmetics[cosmetics.Count - 1].Texture = FlatRedBallServices.Load<Texture2D>(Textures.PickHighlight);
                cosmetics[cosmetics.Count - 1].Position = PickButtons[i].Sprite.Position;
                cosmetics[cosmetics.Count - 1].Width = 80;
                cosmetics[cosmetics.Count - 1].Height = 80;
                SpriteManager.AddSprite(cosmetics[cosmetics.Count - 1]);
            }

            Sprite t = new Sprite();
            t.Texture = FlatRedBallServices.Load<Texture2D>(Textures.Go);
            PickButtons.Add(new MenuButton(game, t, 254));
            SpriteManager.AddSprite(t);
            t.Position = new Vector3(400, -250, 0);

        }

        private void PickUpdate()
        {
            byte oldmouseover = mouseover;
            mouseover = 255;
            foreach (MenuButton m in PickButtons) 
            {
                if (m.MouseOver())
                {
                    mouseover = m.Buttonindex;
                    break;
                }
            }
            if (mouseover != oldmouseover) 
            {
                if (mouseover < 254)
                {
                    //show new spellinfo
                    foreach (Sprite s in cosmetics)
                    {
                        if (s.Position == new Vector3(-400, -250, 0))
                        {
                            SpriteManager.RemoveSprite(s);
                        }
                    }
                    cosmetics.Add(new Sprite());
                    cosmetics[cosmetics.Count - 1].Texture = PickButtons[mouseover].Sprite.Texture;
                    cosmetics[cosmetics.Count - 1].Position = new Vector3(-400, -250, 0);
                    cosmetics[cosmetics.Count - 1].Width = 100;
                    cosmetics[cosmetics.Count - 1].Height = 100;
 
                    SpriteManager.AddSprite(cosmetics[cosmetics.Count - 1]);
                }
            }
            if (Mouse.GetState().LeftButton == ButtonState.Pressed) 
            {
                if (mouseover == 254) 
                {
                    foreach (MenuButton m in PickButtons) 
                    {
                        if (m.Buttonindex == 254) 
                        {
                            m.Sprite.Texture = FlatRedBallServices.Load<Texture2D>(Textures.Waiting);
                        }
                    }
                    if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server) 
                    {
                        ready = true;
                    }
                    else if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Client) 
                    {
                        mAgent.WriteMessage((byte)MessageType.Ready);
                        mAgent.WriteMessage(pickedspell[0]);
                        mAgent.WriteMessage(pickedspell[1]);
                        mAgent.WriteMessage(pickedspell[2]);
                        mAgent.WriteMessage(playertexture);
                        mAgent.SendMessage(mAgent.Connections[0], true);
                    }
                }
                else if (mouseover != 255)
                {
                    if (mouseover < 9)
                    {
                        pickedspell[mouseover / 3] = (byte)(mouseover % 3);
                    }
                    else if (mouseover < 12) 
                    {
                        playertexture = (byte)(mouseover - 9);
                    }
                    //%3 is temp
                    foreach (Sprite s in cosmetics)
                    {
                        if (s.Position.X == PickButtons[mouseover].Sprite.Position.X)
                        {
                            SpriteManager.RemoveSprite(s);
                        }
                    }
                    cosmetics.Add(new Sprite());
                    cosmetics[cosmetics.Count - 1].Texture = FlatRedBallServices.Load<Texture2D>(Textures.PickHighlight);
                    cosmetics[cosmetics.Count - 1].Position = PickButtons[mouseover].Sprite.Position;
                    cosmetics[cosmetics.Count - 1].Width = 80;
                    cosmetics[cosmetics.Count - 1].Height = 80;
                    SpriteManager.AddSprite(cosmetics[cosmetics.Count - 1]);
                }
            }
        }

        /// <summary>
        /// Creates all the player classes and adds them to the proper lists
        /// </summary>
        /// <param name="gameObjects"></param>
        private void CreatePlayers(List<GameObject> gameObjects)
        {
            for (byte i = 0; i < Playercount; i++)
            {
                Sprite tSprite = SpriteManager.AddManagedInvisibleSprite();
                tSprite.Texture = FlatRedBallServices.Load<Texture2D>(Textures.PlayerTexture1);
                tempplayer = new Player(game, tSprite);                
                tempplayer.Sprite.Position.X += -300 + (i % 3 * 300);
                tempplayer.Sprite.Position.Y -= -300 + ((byte)(i / 3) * 300);
                SpriteManager.AddDrawableBatch(new CustomSpriteDrawableBatch(tempplayer.Sprite, GlobalData.GlobalData.Lighting));
                PlayerList.Add(tempplayer);
                gameObjects.Add(tempplayer);
                Aliveplayers.Add(tempplayer);
                gamestate = GameState.Playing;
            }
            AssignPlayerIndices();
        }

        /// <summary>
        /// Gives all players a unique number
        /// </summary>
        private void AssignPlayerIndices()
        {
            for (byte i = 0; i < PlayerList.Count; i++)
            {
                PlayerList[i].Playerindex = i;
                if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server && i != 0)
                {
                    mAgent.WriteMessage((byte)MessageType.PlayerID);
                    mAgent.WriteMessage((byte)i);
                    mAgent.SendMessage(mAgent.Connections[i - 1], true);

                }
            }
        }
        /// <summary>
        /// Resets all variables
        /// </summary>
        /// <param name="gameObjects"></param>
        private void StartNewRound(List<GameObject> gameObjects)
        {
            foreach (MenuButton m in PickButtons) 
            {
                SpriteManager.RemoveSprite(m.Sprite);
            }
            PickButtons.Clear();
            SpriteManager.RemoveSpriteList(cosmetics);
            CreatePlayers(gameObjects);
            if (GlobalData.GlobalData.GameData.TypeOfGame == GlobalData.GameData.GameType.Server)
            {
                foreach (Player p in PlayerList)
                {
                    if (p.Playerindex == 0)
                    {
                        p.PickedSpell[0] = pickedspell[0];
                        p.PickedSpell[1] = pickedspell[1];
                        p.PickedSpell[2] = pickedspell[2];
                        p.Playerskin = playertexture;
                    }
                    else
                    {
                        p.PickedSpell[0] = mAgent.Connections[p.Playerindex - 1].pickedspell[0];
                        p.PickedSpell[1] = mAgent.Connections[p.Playerindex - 1].pickedspell[1];
                        p.PickedSpell[2] = mAgent.Connections[p.Playerindex - 1].pickedspell[2];
                        p.Playerskin = mAgent.Connections[p.Playerindex - 1].PlayerTexture;
                    }
                }
                foreach (NetConnection Player in mAgent.Connections)
                {
                    mAgent.WriteMessage((byte)MessageType.Action);
                    mAgent.WriteMessage((byte)ActionType.ServerRestart);
                    mAgent.WriteMessage((byte)mAgent.Connections.Count + 1);
                    mAgent.SendMessage(Player, true);
                    foreach (Player p in PlayerList)
                    {
                        mAgent.WriteMessage((byte)MessageType.Spellpicks);
                        mAgent.WriteMessage((byte)p.Playerindex);
                        mAgent.WriteMessage((byte)p.PickedSpell[0]);
                        mAgent.WriteMessage((byte)p.PickedSpell[1]);
                        mAgent.WriteMessage((byte)p.PickedSpell[2]);
                        mAgent.WriteMessage(p.Playerskin);
                        mAgent.SendMessage(Player, true);
                    }
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
            //if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.Q)) PlayerList[PlayerID].keycount += 16;
            if (PlayerList[PlayerID].mousestate.LeftButton == ButtonState.Pressed) PlayerList[PlayerID].keycount += 32;
            if (PlayerList[PlayerID].mousestate.RightButton == ButtonState.Pressed) PlayerList[PlayerID].keycount += 64;
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.Space)) { PlayerList[PlayerID].keycount += 128; }
            

            PlayerList[PlayerID].MouseX = GuiManager.Cursor.WorldXAt(PlayerList[PlayerID].Sprite.Z);
            PlayerList[PlayerID].MouseY = GuiManager.Cursor.WorldYAt(PlayerList[PlayerID].Sprite.Z);
        }

        private void CameraUpdate() 
        {
            if (PlayerList[PlayerID].keystate.IsKeyDown(Keys.LeftShift))
            {
            SpriteManager.Camera.Position.X = PlayerList[PlayerID].Sprite.Position.X + (PlayerList[PlayerID].MouseX - PlayerList[PlayerID].Sprite.Position.X)/2;
            SpriteManager.Camera.Position.Y = PlayerList[PlayerID].Sprite.Position.Y + (PlayerList[PlayerID].MouseY - PlayerList[PlayerID].Sprite.Position.Y) /2;
            }
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

                

                if (p.goalVelocityX > p.Sprite.Velocity.X + 10)
                {
                    p.Sprite.Acceleration.X = accelerationspeed;
                }

                else if (p.goalVelocityX < p.Sprite.Velocity.X - 10)
                {
                    p.Sprite.Acceleration.X = -accelerationspeed;
                }
                else
                {
                    p.Sprite.Acceleration.X = 0;
                    if (p.Sprite.Velocity.X < 20 && p.Sprite.Velocity.X > -20)
                    {
                        p.Sprite.Velocity.X = 0;
                    }
                }



                //Y 
                if (p.goalVelocityY > p.Sprite.Velocity.Y + 10)
                {
                    p.Sprite.Acceleration.Y = accelerationspeed;
                }

                else if (p.goalVelocityY < p.Sprite.Velocity.Y - 10)
                {
                    p.Sprite.Acceleration.Y = -accelerationspeed;
                }
                else
                {
                    p.Sprite.Acceleration.Y = 0;
                    if (p.Sprite.Velocity.Y < 20 && p.Sprite.Velocity.Y > -20)
                    {
                        p.Sprite.Velocity.Y = 0;
                    }
                }

                //if (p.InterPos != Vector3.Zero)
                //{
                //    p.Sprite.Position.X = p.InterPos.X * (ticktime * (float)TimeManager.SecondDifference);
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
                p.desiredRotation = (float)Math.Atan2(p.Sprite.Velocity.Y, p.Sprite.Velocity.X);
            }
            
            if (p.desiredRotation < 0)
            {
                p.desiredRotation += (2 * (float)Math.PI);
            }

            if (p.desiredRotation - p.Sprite.RotationZ < p.RotationSpeed * TimeManager.SecondDifference*1.50 && p.desiredRotation - p.Sprite.RotationZ > -p.RotationSpeed * TimeManager.SecondDifference*1.50)
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

        //private void Spellselection(Player p) 
        //{
        //    if ((((p.keycount & 16) == 16) && !((p.oldkeycount & 16) == 16))) //|| p.mousestate.ScrollWheelValue > p.OldMouseValue)
        //    {
        //        if ((int)p.selectedSpell > 0)
        //        {
        //            p.selectedSpell--;
        //        }
        //        else p.selectedSpell = (TechnoViking.Player.Spellbook)Enum.GetNames(typeof(TechnoViking.Player.Spellbook)).Length - 1;
        //    }
        //    if ((((p.keycount & 32) == 32) && !((p.oldkeycount & 32) == 32))) //|| p.mousestate.ScrollWheelValue > p.OldMouseValue)
        //    {
        //        if ((int)p.selectedSpell < Enum.GetNames(typeof(TechnoViking.Player.Spellbook)).Length - 1)
        //        {
        //            p.selectedSpell++;
        //        }
        //        else p.selectedSpell = 0;
        //    }
        //}
        //127.0.0.1
        /// <summary>
        /// Handles everything related to spellcasting
        /// </summary>
        /// <param name="gameObjects"></param>
        private void Spellcasting(List<GameObject> gameObjects, Player p)
        {
            if ((p.keycount & 32) == 32 && (p.oldkeycount & 32) != 32)
            {
                abilitys.CastAbility((int)p.PickedSpell[0], gameObjects, p);
            }
            if ((p.keycount & 64) == 64 && (p.oldkeycount & 64) != 64)
            {
                abilitys.CastAbility((int)p.PickedSpell[1], gameObjects, p);
            }
            if ((p.keycount & 128) == 128 && (p.oldkeycount & 128) != 128)
            {
                abilitys.CastAbility((int)p.PickedSpell[2], gameObjects, p);
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
                                List<Projectile> tlist = new List<Projectile>();
                                tlist.Add((Projectile)g);
                                
                                pl.HP -= tlist[0].damage;
                                Bounce(pl, g);   
                                g.Kill(gameObjects);
                                

                            }
                        }
                        if (g is Player && g != pl)
                        {
                            if (pl.CircleCollidesWith(g))
                            {
                                Bounce(pl, g);
                            }
                            
                        }
                    
                }
                if (pl.HP <= 0)
                {
                    pl.Kill(gameObjects);
                    Aliveplayers.Remove(pl);
                    if (pl.Playerindex == PlayerID) 
                    {
                        gamestate = GameState.Watching;
                    }
                }
            }

        }

        //private void BounceOld(GameObject pl, GameObject g, bool move) 
        //{
        //    Vector3 adjustment = g.Sprite.Velocity;

        //    g.Sprite.Velocity -= adjustment;
        //    pl.Sprite.Velocity -= adjustment;

        //    double V1 = pl.Sprite.Velocity.Length();
        //    double V2 = 0;
        //    Vector3 posdiff = pl.Sprite.Position - g.Sprite.Position;
        //    //float temp = posdiff.X / pl.Sprite.Velocity.X;
        //    float angle = (float)Math.Atan2(pl.Sprite.Velocity.Y, pl.Sprite.Velocity.X);

        //    if (move)
        //    {
        //        float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(pl.Sprite.Position.Z);
        //        pl.Sprite.Position.X -= ((((pl.Sprite.Width / pixelsPerUnit) + (g.Sprite.Width + (1/float.MinValue) / pixelsPerUnit)) - posdiff.Length()) / 2) * (float)Math.Cos(angle);
        //        pl.Sprite.Position.Y -= ((((pl.Sprite.Width / pixelsPerUnit) + (g.Sprite.Width + (1 / float.MinValue) / pixelsPerUnit)) - posdiff.Length()) / 2) * (float)Math.Sin(angle);
        //        g.Sprite.Position.X += ((((pl.Sprite.Width / pixelsPerUnit) + (g.Sprite.Width + (1 / float.MinValue) / pixelsPerUnit)) - posdiff.Length()) / 2) * (float)Math.Cos(angle);
        //        g.Sprite.Position.Y += ((((pl.Sprite.Width / pixelsPerUnit) + (g.Sprite.Width + (1 / float.MinValue) / pixelsPerUnit)) - posdiff.Length()) / 2) * (float)Math.Sin(angle);
        //        posdiff = pl.Sprite.Position - g.Sprite.Position;
        //    }
        //    double a = pl.Sprite.Velocity.Length();
        //    double b = posdiff.Length();
        //    Vector3 triangle3 = g.Sprite.Position - (pl.Sprite.Position + pl.Sprite.Velocity);
        //    double c = triangle3.Length();
        //    double theta = 0;
        //    if (a* b!=0){
        //        theta = 2 * Math.PI - Math.Acos((a * a + b * b - c * c) / (2 * a * b));
        //        if ((a * a + b * b - c * c) / (2 * a * b) > 1)
        //        {
        //            theta = 2 * Math.PI;
        //        }
        //    }

        //    double M1 = 1/pl.InvMass;
        //    double M2 = 1/g.InvMass;
        //    double A1;
        //    double A2;
            
        //    V2 = ((2 * M1 * Math.Cos(theta)) * V1) / (M1 + M2);
        //    V1 = (Math.Sqrt(M1 * M1 + M2 * M2 - 2 * M1 * M2 * Math.Cos(2 * theta)) * V1) / (M1 + M2);
        //    A1 = angle + Math.Atan2(M1 - M2 * Math.Cos(2 * theta) , M2 * Math.Sin(2 * theta));
        //    A2 = theta + angle;




        //    pl.Sprite.Velocity.X = (float)(V1 * Math.Cos(A1));
        //    pl.Sprite.Velocity.Y = (float)(V1 * Math.Sin(A1));
        //    g.Sprite.Velocity.X = (float)(V2 * Math.Cos(A2));
        //    g.Sprite.Velocity.Y = (float)(V2 * Math.Sin(A2));

        //    pl.Sprite.Velocity += adjustment;
        //    g.Sprite.Velocity += adjustment;
        //}

        private void Bounce(GameObject a, GameObject b) 
        {
            Vector3 rv = b.Sprite.Velocity - a.Sprite.Velocity;
            Vector3 normal = b.Sprite.Position - a.Sprite.Position;
            normal.Normalize();

            float velAlongNormal = Vector3.Dot( rv, normal );
 
            if(velAlongNormal > 0) return;
            
            float j = -velAlongNormal;
            j /= a.InvMass + b.InvMass;
            j *= Math.Max(a.Softness, b.Softness);
 
            Vector3 impulse = j * normal;
            a.Sprite.Velocity -= a.InvMass * impulse;
            b.Sprite.Velocity += b.InvMass * impulse;

            Vector3 posdiff = a.Sprite.Position - b.Sprite.Position;
            float lenght = posdiff.Length() - a.Sprite.Texture.Width / 2 - b.Sprite.Texture.Width / 2;
            lenght *= -1;
            const float percent = 0.05f; // usually 20% to 80%
            const float slop = 0.01f; // usually 0.01 to 0.1
            Vector3 correction = Math.Max(lenght - slop, 0.0f) / (a.InvMass + b.InvMass) * percent * normal;
            a.Sprite.Position -= a.InvMass * correction;
            b.Sprite.Position += b.InvMass * correction;
                
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
                throw new Exception("Tried to creater server as client");
                
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
                        if ((PlayerList[PlayerID].keycount & 64) == 64 || (PlayerList[PlayerID].keycount & 32) == 32 || (PlayerList[PlayerID].keycount & 128) == 128)
                        {
                            mAgent.WriteMessage(PlayerList[PlayerID].MouseX);
                            mAgent.WriteMessage(PlayerList[PlayerID].MouseY);
                        }
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
                        if ((PlayerList[i].keycount & 64) == 64 || (PlayerList[i].keycount & 32) == 32 || (PlayerList[i].keycount & 128) == 128)
                        {
                            PlayerList[i].MouseX = incomingMessage.ReadFloat();
                            PlayerList[i].MouseY = incomingMessage.ReadFloat();
                        }
                    }
                }
                switch (type)
                {
                case (byte)MessageType.Action:
                    switch (incomingMessage.ReadByte()) 
                    {
                        case (byte)ActionType.ServerRestart:
                            Playercount = incomingMessage.ReadByte();
                            StartNewRound(gameObjects);
                        break;
                    }
                    break;
                        
                    case (byte)MessageType.Spellpicks:
                    Player pl = PlayerList[incomingMessage.ReadByte()];
                    pl.PickedSpell[0] = incomingMessage.ReadByte();
                    pl.PickedSpell[1] = incomingMessage.ReadByte();
                    pl.PickedSpell[2] = incomingMessage.ReadByte();
                    pl.Playerskin = incomingMessage.ReadByte();
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
                                if ((PlayerList[i].keycount & 64) == 64 || (PlayerList[i].keycount & 32) == 32 || (PlayerList[i].keycount & 128) == 128)
                                {
                                    PlayerList[i].MouseX = incomingMessage.ReadFloat();
                                    PlayerList[i].MouseY = incomingMessage.ReadFloat();
                                }
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
                    case (byte)MessageType.Ready:
                        incomingMessage.SenderConnection.pickedspell[0] = incomingMessage.ReadByte();
                        incomingMessage.SenderConnection.pickedspell[1] = incomingMessage.ReadByte();
                        incomingMessage.SenderConnection.pickedspell[2] = incomingMessage.ReadByte();
                        incomingMessage.SenderConnection.PlayerTexture = incomingMessage.ReadByte();
                        incomingMessage.SenderConnection.Ready = true;

                        break;
                }
		    }
	        
            //Server only logic
            //if (Aliveplayers.Count < 2 && mAgent.Connections.Count > 0)
            //{
            //    LoadPick(gameObjects);
            //    foreach (NetConnection Player in mAgent.Connections)
            //    {
            //        Player.Ready = false;
            //    }
            //}
            bool clientsready = true;
            foreach (NetConnection Player in mAgent.Connections)
            {
                if (!Player.Ready) 
                {
                    clientsready = false;
                }
            }
            if (ready && clientsready && mAgent.Connections.Count > 0 && gamestate != GameState.Playing)
            {
                StartNewRound(gameObjects);
                ready = false;
                foreach (NetConnection Player in mAgent.Connections)
                {
                    Player.Ready = false;
                }
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
                        if ((PlayerList[i].keycount & 64) == 64 || (PlayerList[i].keycount & 32) == 32 || (PlayerList[i].keycount & 128) == 128)
                        {
                            mAgent.WriteMessage(PlayerList[i].MouseX);
                            mAgent.WriteMessage(PlayerList[i].MouseY);
                        }
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
                        if (pos.X <= tX + 0.1 && pos.X >= tX - 0.1 && pos.Y <= tY +0.1 && pos.Y >= tY - 0.1)
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
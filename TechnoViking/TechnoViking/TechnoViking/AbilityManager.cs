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
    
    class AbilityManager
    {
        //Ska man synka abilitymanagern när man tickar?
        float[] cooldowns = new float[3] { 2, 1, 5 };
        
        Queue<Projectile> projectileQueue = new Queue<Projectile>(); 
        Game mgame;
        Sprite tempsprite;
        Texture2D texture;
        public AbilityManager(Game game)
        {
            mgame = game;
        }

        float projectilecasttime = 0.2f;
        Projectile tempprojectile;
        //Beam tempbeam;
        //man ska inte kunna casta andra spells när man castar
        public void CastAbility(int spellindex, List<GameObject> gameObjects, float CursorX, float CursorY, Player player)
        {
            bool stop = false;
            foreach (Projectile p in projectileQueue)
            {
                if (p.playerID == player.Playerindex)
                {
                    stop = true;
                }
            }
            if (player.Lastcasted[spellindex] + cooldowns[spellindex] < TimeManager.CurrentTime && !stop)
            {
                player.RotationLocked = true;
                player.desiredRotation = (float)Math.Atan2(
                CursorY - player.Sprite.Y, CursorX - player.Sprite.X) + player.Offset;
                switch (spellindex)
                {
                    case 0:
                        tempsprite = new Sprite();
                        tempsprite.TimeCreated = TimeManager.CurrentTime;
                        texture = FlatRedBallServices.Load<Texture2D>(Game1.Fireballtexture1);
                        tempsprite.Texture = texture;
                        tempprojectile = new Projectile(mgame, tempsprite, player, spellindex, gameObjects, CursorX, CursorY);
                        projectileQueue.Enqueue(tempprojectile);
                        break;
                    case 1:
                        tempsprite = new Sprite();
                        tempsprite.TimeCreated = TimeManager.CurrentTime;
                        texture = FlatRedBallServices.Load<Texture2D>(Game1.Fireballtexture1);
                        tempsprite.Texture = texture;
                        tempprojectile = new Projectile(mgame, tempsprite, player, spellindex, gameObjects, CursorX, CursorY);
                        projectileQueue.Enqueue(tempprojectile);
                        break;
                    case 2:
                        //tempbeam = new Beam(mgame, SpriteManager.AddSprite(Game1.Beamtexture1), player, gameObjects, angle);
                        //gameObjects.Add(tempbeam);
                        break;
                }
            }
        }

        public void Update(List<GameObject> gameObjects, List<Player> PlayerList)
        {
            
            if (projectileQueue.Count != 0)
            {
                while (projectileQueue.Peek().Sprite.TimeCreated < TimeManager.CurrentTime - projectilecasttime)
                {
                    gameObjects.Add(projectileQueue.Peek());
                    projectileQueue.Peek().SetPosition(gameObjects);
                    PlayerList[projectileQueue.Peek().playerID].Lastcasted[projectileQueue.Peek().Spellindex] = TimeManager.CurrentTime;
                    PlayerList[projectileQueue.Peek().playerID].RotationLocked = false;
                    projectileQueue.Peek().Sprite.TimeCreated = TimeManager.CurrentTime;
                    SpriteManager.AddSprite(projectileQueue.Dequeue().Sprite);
                    
                    if (projectileQueue.Count == 0) 
                    {
                        break;
                    }
                }
            }
                
        } 

        //Behövs det en kill funktion?
    }
}

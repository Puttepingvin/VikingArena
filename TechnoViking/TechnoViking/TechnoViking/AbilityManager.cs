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
        float[] cooldowns = new float[3] { 0.5f, 1, 5 };
        float blinkdistance = 10.0f;

        List<Projectile> projectileList = new List<Projectile>(); 
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
        public void CastAbility(int spellindex, List<GameObject> gameObjects, Player player)
        {
            bool stop = false;
            foreach (Projectile p in projectileList)
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
                player.MouseY - player.Sprite.Y, player.MouseX - player.Sprite.X) + player.Offset;
                switch (spellindex)
                {
                    case 0:
                        tempsprite = new Sprite();
                        tempsprite.TimeCreated = TimeManager.CurrentTime;
                        texture = FlatRedBallServices.Load<Texture2D>(Game1.Fireballtexture1);
                        tempsprite.Texture = texture;
                        tempprojectile = new Projectile(mgame, tempsprite, player, spellindex, gameObjects, player.MouseX, player.MouseY);
                        projectileList.Add(tempprojectile);
                        break;
                    case 1:
                        tempsprite = new Sprite();
                        tempsprite.TimeCreated = TimeManager.CurrentTime;
                        texture = FlatRedBallServices.Load<Texture2D>(Game1.Fireballtexture1);
                        tempsprite.Texture = texture;
                        tempprojectile = new Projectile(mgame, tempsprite, player, spellindex, gameObjects, player.MouseX, player.MouseY);
                        projectileList.Add(tempprojectile);
                        break;
                    case 2:
                        float angle = 
                        player.Sprite.Position.X += blinkdistance * (float)Math.Cos(player.desiredRotation - player.Offset);
                        player.Sprite.Position.Y += blinkdistance * (float)Math.Sin(player.desiredRotation - player.Offset);
                        player.Lastcasted[spellindex] = TimeManager.CurrentTime;
                        player.RotationLocked = false; 
                        break;
                }
            }
        }

        public void Update(List<GameObject> gameObjects, List<Player> PlayerList)
        {
            
            if (projectileList.Count != 0)
            {
                foreach(Projectile pr in new List<Projectile>(projectileList)){
                    if (pr.Sprite.TimeCreated < TimeManager.CurrentTime - projectilecasttime)
                {
                    gameObjects.Add(pr);
                    pr.SetPosition(gameObjects);
                    PlayerList[pr.playerID].Lastcasted[pr.Spellindex] = TimeManager.CurrentTime;
                    PlayerList[pr.playerID].RotationLocked = false;
                    pr.Sprite.TimeCreated = TimeManager.CurrentTime;
                    SpriteManager.AddSprite(pr.Sprite);
                    projectileList.Remove(pr);
                }
                }

            }
                
        } 

        //Behövs det en kill funktion?
    }
}

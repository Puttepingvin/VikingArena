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
    abstract class Actor : GameObject
    {
         public Actor(Game game, Sprite sprite)
            : base(game, sprite)
         {



            if (sprite != null)
                {
                   
                    float texturePixelWidth = sprite.Texture.Width;
                    float texturePixelHeight = sprite.Texture.Height;

                    //Now, we need to find out how many pixels per unit there are in our view at the Sprite's Z position:
                    float pixelsPerUnit = SpriteManager.Camera.PixelsPerUnitAt(sprite.Z);

                    //Now, we just have to use those two values to set the scale.
                    sprite.ScaleX = .5f * texturePixelWidth / pixelsPerUnit;
                    sprite.ScaleY = .5f * texturePixelHeight / pixelsPerUnit;
                }
            


        }

        // Net variables.
        enum ENetRole
        {
            ROLE_None,              // No role at all.
            ROLE_SimulatedProxy,    // Locally simulated proxy of this actor.
            ROLE_AutonomousProxy,   // Locally autonomous proxy of this actor.
            ROLE_Authority,         // Authoritative control over the actor.
        };
        
        ENetRole RemoteRole, Role;
         

    }
}

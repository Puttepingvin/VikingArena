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
    class CustomSpriteDrawableBatch : IDrawableBatch
    {
        Effect mEffect;
        Sprite mSprite;

        #region Properties

        // Add any custom properties you'll need for rendering here

        public bool UpdateEveryFrame
        {
            get { return true; }
        }

        public float X
        {
            get
            {
                return mSprite.X;
            }
            set
            {
                mSprite.X = value;
            }
        }

        public float Y
        {
            get
            {
                return mSprite.Y;
            }
            set
            {
                mSprite.Y = value;
            }
        }

        public float Z
        {
            get
            {
                return mSprite.Z;
            }
            set
            {
                mSprite.Z = value;
            }
        }

        #endregion

        public CustomSpriteDrawableBatch(Sprite spriteToDraw, Effect effect)
        {
            mSprite = spriteToDraw;
            mEffect = effect;
        }

        public void Draw(FlatRedBall.Camera camera)
        {
            // This is needed to update the Sprite's VerticesForDrawing
            SpriteManager.ManualUpdate(mSprite);

            // Set graphics states
            FlatRedBallServices.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            // Set the vertex declaration
            VertexDeclaration vd = Renderer.PositionColorTextureVertexDeclaration;
            //Vector3 lightDirection = (mSprite.Position-new Vector3(GuiManager.Cursor.WorldXAt(0), GuiManager.Cursor.WorldYAt(0), 500));
            //lightDirection.Normalize();
            // Set Parameters here:
            mEffect.Parameters["ColorMap"].SetValue(mSprite.Texture);
            mEffect.Parameters["View"].SetValue(camera.View);
            mEffect.Parameters["Projection"].SetValue(camera.Projection);
            mEffect.Parameters["lightPos"].SetValue(new Vector3(GuiManager.Cursor.WorldXAt(0), GuiManager.Cursor.WorldYAt(0), 20));
            mEffect.Parameters["NormalMap"].SetValue(FlatRedBallServices.Load<Texture2D>(Textures.PlayerTexture2Normal));
            mEffect.Parameters["spritePos"].SetValue(mSprite.Position);
            mEffect.Parameters["spriteWidth"].SetValue(mSprite.Texture.Width);
            mEffect.Parameters["spriteHeight"].SetValue(mSprite.Texture.Height);
            mEffect.Parameters["spriteRotation"].SetValue(mSprite.RotationZ);


            //swapping vertexes to draw sprite correctly
            VertexPositionColorTexture[] vpct = mSprite.VerticesForDrawing;
            VertexPositionColorTexture v2 = vpct[2];
            vpct[2] = vpct[3];
            vpct[3] = v2;

            foreach (EffectPass pass in mEffect.CurrentTechnique.Passes)
            {
                // Start each pass
                pass.Apply();

                // Do this to avoid magic numbers:
                const int numberOfTrianglesInASprite = 2;

                // Draw the shape
                FlatRedBallServices.GraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(
                    PrimitiveType.TriangleStrip, vpct , 0, numberOfTrianglesInASprite);
            }
        }

        public void Update() { }

        public void Destroy() { }
    }
    
}


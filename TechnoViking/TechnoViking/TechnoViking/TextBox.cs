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
    
    class TextBox : IKeyboardSubscriber
    {
        public delegate void TextBoxEvent(TextBox sender);
        SpriteFont mfont;
        
        FlatRedBall.Graphics.Text text;
        

        public TextBox(int x, int y, int width) 
        {
            text = FlatRedBall.Graphics.TextManager.AddText("Enter Server IP");
            text.SetColor(66, 66, 66);
            mfont = GlobalData.GlobalData.Font;
            X = x;
            Y = y;
            Width = width;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; private set; }


        public bool Highlighted { get; set; }



        string mtext = "";

        
        public String Text
        {
            get
            {
                return mtext;
            }


            set
            {
                mtext = value;
                if (mtext == null)
                    mtext = "";

                if (mtext != "")
                {
                    //if you attempt to display a character that is not in your font
                    //you will get an exception, so we filter the characters
                    String filtered = "";
                    foreach (char c in value)
                    {
                        //if (mfont.Characters.Contains(c))
                            filtered += c;
                    }

                    mtext = filtered;

                    //if (mfont.MeasureString(mtext).X > Width)
                    //{
                    //    //recursion to ensure that text cannot be larger than the box
                    //    Text = mtext.Substring(0, mtext.Length - 1);
                    //}
                }
            }
        }

        public void Update(List<GameObject> gameObjects)
        {
            text.DisplayText = Text; 
        }

        public void Kill(List<GameObject> gameObjects) 
        {
            FlatRedBall.Graphics.TextManager.RemoveText(text);
        }

        

    public void RecieveTextInput(char inputChar)
    {
        Text = Text + inputChar;
    }
    public void RecieveTextInput(string text)
    {
        Text = Text + text;
    }
    public void RecieveCommandInput(char command)
    {
        switch (command)
        {
            case '\b': //backspace
                if (Text.Length > 0)
                    Text = Text.Substring(0, Text.Length - 1);
                break;
            case '\r': //return
                if (OnEnterPressed != null)
                    OnEnterPressed(this);
                break;
            case '\t': //tab
                if (OnTabPressed != null)
                    OnTabPressed(this);
                break;
            default:
                break;
        }
    }
    public void RecieveSpecialInput(Keys key)
    {

    }

    public event TextBoxEvent OnEnterPressed;
    public event TextBoxEvent OnTabPressed;

    public bool Selected
    {
        get;
        set;
    }


    }
}

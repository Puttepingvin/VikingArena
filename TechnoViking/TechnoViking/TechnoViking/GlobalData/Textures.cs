using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TechnoViking.GlobalData
{
    class Textures
    {
        /// <summary>
        /// Saves all the references to external content
        /// </summary>
        public const string PlayerTexture1 = "/Content/viking1.png";
        public const string Shadowbolttexture1 = "/Content/viking1.png";
        public const string Fireballtexture1 = "/Content/fireball.png";
        public const string Beamtexture1 = "/Content/Beam.png";
        public const string Castingtexture1 = "/Content/fireball.png";
        public const string Pixeltexture = "/Content/pixel.png";
        public const string MenuButtonTexture1 = "/Content/menubutton1.png";
        public const string MenuButtonTexture2 = "/Content/menubutton2.png";
        public const string MenuButtonTexture3 = "/Content/menubutton3.png";
        public const string GameFont = "/Content/GameFont.xml";
        public const string PickHighlight = "/Content/Highlight.png";
        public const string Go = "/Content/Go.png";
        public const string Waiting = "/Content/Waiting.png";
        public const string PlayerTexture2 = "/Content/MANNEN1.png";
        public const string PlayerTexture3 = "/Content/KVINNAN1.png";
        public const string PlayerTexture2Normal = "/Content/MANNEN1NORMAL.png";
        public static string[] spelltextures = new string[9]
        {
            Fireballtexture1,
            Shadowbolttexture1,
            Beamtexture1,
            Beamtexture1,
            Fireballtexture1,
            Shadowbolttexture1,
            Shadowbolttexture1,
            Beamtexture1,
            Fireballtexture1,

        };
        public static string[] playertextures = new string[3]
        {
            PlayerTexture1,
            PlayerTexture2,
            PlayerTexture3,
        };
    }
}

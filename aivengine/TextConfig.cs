using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Aiv.Fast2D;
using OpenTK;

namespace Aiv.Engine
{
    public class TextConfig
    {
        public TextConfig(
            Asset fontAsset, Dictionary<char, Tuple<Vector2, Vector2>> charToSprite,
            Color fontBaseColor = default(Color), float spaceWidth = 44f, float spaceHeight = 31f,
            float padding = float.MinValue, Func<float, float> paddingFunc = null, bool staticColor = true)
        {
            if (fontBaseColor == default(Color))
                fontBaseColor = Color.Black;
            if (paddingFunc == null && padding == float.MinValue)
                padding = 0;
            FontAsset = fontAsset;
            FontTexture = new Texture(fontAsset.FileName);
            CharToSprite = charToSprite;
            FontBaseColor = fontBaseColor;
            SpaceWidth = spaceWidth;
            SpaceHeight = spaceHeight;
            Padding = padding;
            PaddingFunc = paddingFunc;
            StaticColor = staticColor;

            if (Default == null)
                Default = this;
        }

        public static TextConfig Default { get; set; }

        public float Padding { get; set; }

        public float SpaceHeight { get; set; }

        public float SpaceWidth { get; set; }

        public Func<float, float> PaddingFunc { get; set; }

        public bool StaticColor { get; set; }

        public Color FontBaseColor { get; set; }

        public Dictionary<char, Tuple<Vector2, Vector2>> CharToSprite { get; set; }

        public Asset FontAsset { get; set; }

        public Texture FontTexture { get; set; }
    }
}
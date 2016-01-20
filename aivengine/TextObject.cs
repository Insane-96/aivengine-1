/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;


namespace Aiv.Engine
{
    public class TextObject : GameObject
    {
        private Fast2D.TextObject textRaw;

        public TextObject(
            Vector2 scale, Color color, string fontFile = "font.png",
            Dictionary<char, Tuple<Vector2, Vector2>> charToSprite = null, float alpha = 1f, 
            Color fontBaseColor = default(Color), float spaceWidth = 44f, float spaceHeight = 31f,
            float padding = float.MinValue, Func<float, float> paddingFunc = null)
        {
            // default for ff-fonts.. this should be the font file sprite's color
            //  which will be changed if you choose a color, do not use if you have  a font with multiple
            //  colors, shadows and such.
            //if (fontBaseColor == default(Color))
            //    fontBaseColor = Color.FromArgb(238, 242, 238);
            // if the textobject has been initialized without a padding and a paddingFunc use default one
            if (paddingFunc == null && padding == float.MinValue)
                paddingFunc = (float width) =>
                {
                    float result = width;
                    if (width > 15f && width < 44f)
                        result *= -0.25f;
                    else if (width < 15f)
                        result *= -0.1f;
                    return result;
                };
            textRaw = new Fast2D.TextObject
            {
                FontBaseColor = fontBaseColor,
                Alpha = (int) (alpha*255),
                SpaceWidth = spaceWidth,
                SpaceHeight = spaceHeight,
            };
            if (padding != float.MinValue)
                textRaw.Padding = padding;
            else
                textRaw.PaddingFunc = paddingFunc;

            // ff workaround... to remove b4 gamejam
            //if (color == Color.White)
            //    color = Color.FromArgb(238, 242, 238);

            FontFile = new Asset(fontFile).FileName;
            if (charToSprite == null)
                charToSprite = DefaultCharToSprite;
            textRaw.CharToSprite = charToSprite;
            Scale = scale;
            Color = color;

            IgnoreCamera = true;
        }

        public TextObject(
            float uniformScale, Color color, string fontFile = "font.png",
            Dictionary<char, Tuple<Vector2, Vector2>> charToSprite = null, float alpha = 1f) :
                this(new Vector2(uniformScale, uniformScale), color, fontFile, charToSprite, alpha)
        {
        }

        public string Text
        {
            get { return textRaw.Text; }
            set { textRaw.Text = value.ToUpper(); }
        }

        public string FontFile
        {
            get { return textRaw.FontFile; }
            set { textRaw.FontFile = value; }
        }

        public Color Color
        {
            get { return Color; }
            set { textRaw.Color = value; }
        }

        public override Vector2 Scale
        {
            get { return textRaw.Scale; }
            set { textRaw.Scale = value; }
        }

        // change this once in your game to have your own fonts, if you have multiple fonts you
        //  have to put this dictionary every time you create a new textobject
        public static Dictionary<char, Tuple<Vector2, Vector2>> DefaultCharToSprite { get; set; } =
            new Dictionary<char, Tuple<Vector2, Vector2>>
            {
                {'0', Tuple.Create(new Vector2(0f, 0f), new Vector2(44f, 31f))},
                {'1', Tuple.Create(new Vector2(45f, 0f), new Vector2(22f, 31f))},
                {'2', Tuple.Create(new Vector2(66f, 0f), new Vector2(44f, 31f))},
                {'3', Tuple.Create(new Vector2(109f, 0f), new Vector2(44f, 31f))},
                {'4', Tuple.Create(new Vector2(152f, 0f), new Vector2(44f, 31f))},
                {'5', Tuple.Create(new Vector2(195f, 0f), new Vector2(44f, 31f))},
                {'6', Tuple.Create(new Vector2(239f, 0f), new Vector2(44f, 31f))},
                {'7', Tuple.Create(new Vector2(281f, 0f), new Vector2(44f, 31f))},
                {'8', Tuple.Create(new Vector2(325f, 0f), new Vector2(44f, 31f))},
                {'9', Tuple.Create(new Vector2(369f, 0f), new Vector2(44f, 31f))},
                {'A', Tuple.Create(new Vector2(411f, 0f), new Vector2(51f, 31f))},
                {'B', Tuple.Create(new Vector2(462f, 0f), new Vector2(46f, 31f))},
                {'C', Tuple.Create(new Vector2(0f, 31f), new Vector2(44f, 31f))},
                {'D', Tuple.Create(new Vector2(44f, 31f), new Vector2(44f, 31f))},
                {'E', Tuple.Create(new Vector2(88f, 31f), new Vector2(44f, 31f))},
                {'F', Tuple.Create(new Vector2(132f, 31f), new Vector2(44f, 31f))},
                {'G', Tuple.Create(new Vector2(175f, 31f), new Vector2(44f, 31f))},
                {'H', Tuple.Create(new Vector2(219f, 31f), new Vector2(44f, 31f))},
                {'I', Tuple.Create(new Vector2(262f, 31f), new Vector2(15f, 31f))},
                {'J', Tuple.Create(new Vector2(275f, 31f), new Vector2(44f, 31f))},
                {'K', Tuple.Create(new Vector2(319f, 31f), new Vector2(44f, 31f))},
                {'L', Tuple.Create(new Vector2(362f, 31f), new Vector2(44f, 31f))},
                {'M', Tuple.Create(new Vector2(404f, 31f), new Vector2(44f, 31f))},
                {'N', Tuple.Create(new Vector2(450f, 31f), new Vector2(44f, 31f))},
                {'O', Tuple.Create(new Vector2(0f, 62f), new Vector2(44f, 31f))},
                {'P', Tuple.Create(new Vector2(44f, 62f), new Vector2(44f, 31f))},
                {'Q', Tuple.Create(new Vector2(88f, 62f), new Vector2(44f, 31f))},
                {'R', Tuple.Create(new Vector2(131f, 62f), new Vector2(44f, 31f))},
                {'S', Tuple.Create(new Vector2(175f, 62f), new Vector2(44f, 31f))},
                {'T', Tuple.Create(new Vector2(218f, 62f), new Vector2(44f, 31f))},
                {'U', Tuple.Create(new Vector2(262f, 62f), new Vector2(44f, 31f))},
                {'V', Tuple.Create(new Vector2(306f, 62f), new Vector2(44f, 31f))},
                {'W', Tuple.Create(new Vector2(350f, 62f), new Vector2(44f, 31f))},
                {'X', Tuple.Create(new Vector2(395f, 62f), new Vector2(44f, 31f))},
                {'Y', Tuple.Create(new Vector2(439f, 62f), new Vector2(44f, 31f))},
                {'Z', Tuple.Create(new Vector2(0f, 93f), new Vector2(44f, 31f))},
                {'%', Tuple.Create(new Vector2(44f, 93f), new Vector2(44f, 31f))},
                {'!', Tuple.Create(new Vector2(87f, 93f), new Vector2(13f, 31f))},
                {'?', Tuple.Create(new Vector2(100f, 93f), new Vector2(44f, 31f))},
                {'+', Tuple.Create(new Vector2(142f, 93f), new Vector2(36f, 31f))},
                {'-', Tuple.Create(new Vector2(179f, 93f), new Vector2(30f, 31f))},
                {'*', Tuple.Create(new Vector2(209f, 93f), new Vector2(30f, 31f))},
                {'/', Tuple.Create(new Vector2(238f, 93f), new Vector2(34f, 31f))},
                {':', Tuple.Create(new Vector2(296f, 93f), new Vector2(13f, 31f))},
                {'.', Tuple.Create(new Vector2(272f, 93f), new Vector2(13f, 31f))},
                {',', Tuple.Create(new Vector2(272f, 93f), new Vector2(13f, 31f))},
                {'\'', Tuple.Create(new Vector2(285f, 93f), new Vector2(13f, 31f))}
            };

        public Vector2 Measure()
        {
            var result = textRaw.Measure();
            return result;
        }

        public override void Draw()
        {
            base.Draw();
            textRaw.Position = new Vector2(DrawX, DrawY);
            textRaw.Draw();
        }

        public override GameObject Clone()
        {
            var go = (TextObject) base.Clone();
            go.textRaw = textRaw.Clone();
            return go;
        }
    }
}
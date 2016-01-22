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
            Vector2 scale, Color color, float alpha = 1f, TextConfig textConfig = null)
        {
            if (textConfig == null)
                textConfig = TextConfig.Default;
            textRaw = new Fast2D.TextObject
            {
                FontTexture = textConfig.FontTexture.Clone(),
                FontBaseColor = textConfig.FontBaseColor,
                StaticColor = textConfig.StaticColor, 
                Alpha = (int) (alpha*255),
                SpaceWidth = textConfig.SpaceWidth,
                SpaceHeight = textConfig.SpaceHeight
            };
            if (textConfig.Padding != float.MinValue)
                textRaw.Padding = textConfig.Padding;
            else
                textRaw.PaddingFunc = textConfig.PaddingFunc;

            // ff workaround... to remove b4 gamejam
            //if (color == Color.White)
            //    color = Color.FromArgb(238, 242, 238);

            textRaw.CharToSprite = textConfig.CharToSprite;
            Scale = scale;
            Color = color;

            IgnoreCamera = true;
        }

        public TextObject(
            float uniformScale, Color color, float alpha = 1f, TextConfig textConfig = null) 
            : this(new Vector2(uniformScale, uniformScale), color, alpha, textConfig)
        {
        }

        public string Text
        {
            get { return textRaw.Text; }
            set { textRaw.Text = value.ToUpper(); }
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
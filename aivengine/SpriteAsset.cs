/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System;
using System.Collections.Generic;
using Aiv.Fast2D;
using OpenTK;

namespace Aiv.Engine
{
    public class SpriteAsset : Asset
    {
        private static Dictionary<Tuple<string, bool, bool, bool>, Texture> textures;

        public SpriteAsset(
            string fileName, int x = 0, int y = 0, int width = 0, int height = 0,
            bool linear = false, bool repeatx = false, bool repeaty = false) : base(fileName)
        {
            Texture = GetTexture(FileName, linear, repeatx, repeaty);
            X = x;
            Y = y;
            Width = width == 0 ? Texture.Width : width;
            Height = height == 0 ? Texture.Height : height;
        }

        public Texture Texture { get; }

        public int X { get; set; }
        public int Y { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        private static Texture GetTexture(string fileName, bool linear, bool repeatx, bool repeaty)
        {
            if (textures == null)
                textures = new Dictionary<Tuple<string, bool, bool, bool>, Texture>();
            var key = Tuple.Create(fileName, linear, repeatx, repeaty);
            if (!textures.ContainsKey(key))
                textures[key] = new Texture(fileName, linear, repeatx, repeaty);
            return textures[key];
        }

        // x, y, width, height, minalpha, minpixelcount
        private Tuple<int, int, int, int, int, int> lastHitBoxCalculationInfo;
        private Tuple<Vector2, Vector2> cachedHitBoxInfo;
        public Tuple<Vector2, Vector2> CalculateRealHitBox(int minAlpha = 25, int minPixelCount = 2)
        {
            var key = Tuple.Create(X, Y, Width, Height, minAlpha, minPixelCount);
            if (lastHitBoxCalculationInfo != null && lastHitBoxCalculationInfo.Equals(key))
                return cachedHitBoxInfo;
            // minPixelCount: if less than n pixels in a row/col then the row/col is considered empty
            var offSetDone = false;
            // CALCULATE Y
            Vector2 offset = Vector2.Zero;
            Vector2 size = new Vector2(Width, Height);
            for (var posY = 0; posY < Height; posY++)
            {
                var emptyRow = true;
                var pixelCount = 0;
                for (var posX = 0; posX < Width; posX++)
                {
                    if (Texture.Bitmap[(posY + Y) * Texture.Width * 4 + (posX + X) * 4 + 3] > minAlpha)
                    {
                        pixelCount++;
                        if (pixelCount >= minPixelCount) { 
                            emptyRow = false;
                            break;
                        }
                    }

                }
                if (emptyRow && !offSetDone)
                {
                    offset = new Vector2(offset.X, posY);
                }
                else if (!emptyRow)
                {
                    offSetDone = true;
                    size = new Vector2(size.X, posY + 1);
                }
            }
            // CALCULATE X
            offSetDone = false;
            for (var posX = 0; posX < Width; posX++)
            {
                var emptyCol = true;
                var pixelCount = 0;
                for (var posY = 0; posY < Height; posY++)
                {
                    if (Texture.Bitmap[(posY + Y) * Texture.Width * 4 + (posX + X) * 4 + 3] > minAlpha)
                    {
                        pixelCount++;
                        if (pixelCount >= minPixelCount)
                        {
                            emptyCol = false;
                            break;
                        }
                    }
                }
                if (emptyCol && !offSetDone)
                {
                    offset = new Vector2(posX, offset.Y);
                }
                else if (!emptyCol)
                {
                    offSetDone = true;
                    size = new Vector2(posX + 1, size.Y);
                }
            }

            size = new Vector2(size.X - offset.X, size.Y - offset.Y);
            //offset = new Vector2(offset.X - X, offset.Y - Y);
            lastHitBoxCalculationInfo = key;
            cachedHitBoxInfo = Tuple.Create(offset, size);

            return cachedHitBoxInfo;
        }

        public SpriteAsset Clone()
        {
            var go = new SpriteAsset(BaseFileName, X, Y, Width, Height);
            return go;
        }
    }
}
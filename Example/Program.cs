using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aiv.Engine;
using OpenTK;

namespace Example
{
    class Program
    {
        private static Dictionary<char, Tuple<Vector2, Vector2>> charToSprite = new Dictionary<char, Tuple<Vector2, Vector2>>()
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

        static void Main(string[] args)
        {
            Engine engine = new Engine("test", 1024, 768, 60, false, false);
            engine.debugCollisions = true;

            // set default directory for assets, will be appened to all assets's path
            Asset.BasePath = "..\\..\\Assets";

            // load repeating texture
            var repeatingGoblins = new SpriteAsset("goblins.png", 100, 100, 150, 150, repeatx: true, repeaty: true);
            // auto hitbox and spriteasset with repeatx or/and repeaty are NOT compatible
            var obj = new SpriteObject(repeatingGoblins.Width + 20, repeatingGoblins.Height + 100);
            obj.CurrentSprite = repeatingGoblins;

            obj.OnUpdate += sender =>
            {
                var s = (SpriteObject)sender;
                s.SpriteOffset += new Vector2(15f, 15f) * s.DeltaTime;
            };

            // text
            TextConfig.Default = new TextConfig(new Asset("font.png"), charToSprite);
            var semiTransparentText = new TextObject(0.66f, Color.Red, 0.8f);
            semiTransparentText.Text = "SEMI TRANSPARENT";
            semiTransparentText.Y = obj.Height;
            var bigText = new TextObject(1.1f, Color.CadetBlue);
            bigText.Text = "BIG TEXT";
            var semiTransparentTextMeasure = semiTransparentText.Measure();
            bigText.Y = semiTransparentTextMeasure.Y + semiTransparentText.Y;
            var bigTextMeasure = bigText.Measure();

            // hitboxes
            var spriteSheet = new SpriteAsset("rob.png");
            var tileWidth = spriteSheet.Width/22;
            var tileHeight = spriteSheet.Height/1;
            var spriteAsset = new SpriteAsset("rob.png", 0, 0, tileWidth, tileHeight);
            var spriteH = new SpriteObject(spriteAsset.Width, spriteAsset.Height, true);
            spriteH.Y = bigText.Y + bigTextMeasure.Y;
            spriteH.CurrentSprite = spriteAsset;
            spriteH.Scale = new Vector2(5f, 5f);

            // spawn gameobjects
            engine.SpawnObject("obj", obj);

            engine.SpawnObject("semiTransparentText", semiTransparentText);
            engine.SpawnObject("bigText", bigText);

            engine.SpawnObject("spriteH", spriteH);

            engine.Run();
        }
    }
}

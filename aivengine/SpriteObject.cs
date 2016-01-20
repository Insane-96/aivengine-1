/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System.Collections.Generic;
using OpenTK;

namespace Aiv.Engine
{
    public class SpriteObject : GameObject
    {
        private SpriteAsset currentSprite;
        private float width;
        private float height;

        public Dictionary<string, Animation> Animations { get; set; }

        public string CurrentAnimation { get; set; }

        public SpriteAsset CurrentSprite
        {
            get { return currentSprite; }
            set
            {
                if (value == null)
                {
                    Width = 0;
                    Height = 0;
                }
                else
                {
                    Width = value.Sprite.Width;
                    Height = value.Sprite.Height;
                }
                currentSprite = value;
            }
        }

        public float Width
        {
            get { return width * Scale.X; }
            set { width = value; }
        }

        public float Height
        {
            get { return height * Scale.Y; }
            set { height = value; }
        }

        public float BaseWidth => width;
        public float BaseHeight => height;

        private void Animate(string animationName)
        {
            var animation = Animations[animationName];
            var neededTime = 1f/animation.Fps;

            if (Time - animation.LastTick >= neededTime)
            {
                animation.LastTick = Time;
                animation.currentFrame++;
                // end of the animation ?
                var lastFrame = animation.Sprites.Count - 1;
                if (animation.currentFrame > lastFrame)
                {
                    if (animation.Loop)
                    {
                        animation.currentFrame = 0;
                    }
                    else if (animation.OneShot)
                    {
                        // disable drawing
                        animation.owner.CurrentAnimation = null;
                        return;
                    }
                    else
                    {
                        // block to the last frame
                        animation.currentFrame = lastFrame;
                    }
                }
            }
            // simply draw the current frame
            var spriteAssetToDraw = animation.Sprites[animation.currentFrame];
            var spriteToDraw = spriteAssetToDraw.Sprite;
            Width = spriteToDraw.Width;
            Height = spriteToDraw.Height;
            var cameraX = IgnoreCamera ? 0 : Engine.Camera.X;
            var cameraY = IgnoreCamera ? 0 : Engine.Camera.Y;

            spriteAssetToDraw.Sprite.position.X = X - cameraX;
            spriteAssetToDraw.Sprite.position.Y = Y - cameraY;
            spriteAssetToDraw.Sprite.scale = Scale;
            spriteAssetToDraw.Draw();
        }

        public override void Draw()
        {
            base.Draw();
            if (!CanDraw)
                return;
            if (CurrentAnimation != null)
            {
                Animate(CurrentAnimation);
                return;
            }
            if (CurrentSprite != null)
            {
                var cameraX = IgnoreCamera ? 0 : Engine.Camera.X;
                var cameraY = IgnoreCamera ? 0 : Engine.Camera.Y;
                CurrentSprite.Sprite.position.X = X - cameraX;
                CurrentSprite.Sprite.position.Y = Y - cameraY;
                CurrentSprite.Sprite.scale = Scale;
                CurrentSprite.Draw();
            }
        }

        // optional engine param to add animations before spawning the SpriteObject
        public Animation AddAnimation(string name, IEnumerable<string> assets, int fps, Engine engine = null)
        {
            if (engine == null)
                engine = Engine;
            // allocate animations dictionary on demand
            if (Animations == null)
            {
                Animations = new Dictionary<string, Animation>();
            }
            var animation = new Animation();
            animation.Fps = fps;
            animation.Sprites = new List<SpriteAsset>();
            foreach (var asset in assets)
            {
                animation.Sprites.Add((SpriteAsset) engine.GetAsset(asset));
            }
            animation.currentFrame = 0;
            // force the first frame to be drawn
            animation.LastTick = 0;
            animation.Loop = true;
            animation.OneShot = false;
            animation.owner = this;
            Animations[name] = animation;
            return animation;
        }

        public override GameObject Clone()
        {
            var go = new SpriteObject();
            go.Name = Name;
            go.X = X;
            go.Y = Y;
            go.CurrentSprite = CurrentSprite.Clone();
            if (Animations != null)
            {
                go.Animations = new Dictionary<string, Animation>();
                foreach (var animKey in Animations.Keys)
                {
                    go.Animations[animKey] = Animations[animKey].Clone();
                    go.Animations[animKey].owner = go;
                }
            }
            go.CurrentAnimation = CurrentAnimation;
            return go;
        }

        public class Animation
        {
            public int currentFrame;
            public SpriteObject owner;
            public float Fps { get; set; }
            public List<SpriteAsset> Sprites { get; internal set; }
            public float LastTick { get; internal set; }
            public bool Loop { get; set; }
            public bool OneShot { get; set; }

            public Animation Clone()
            {
                var anim = new Animation();
                anim.Fps = Fps;
                if (Sprites != null)
                {
                    anim.Sprites = new List<SpriteAsset>();
                    foreach (var spriteAsset in Sprites)
                    {
                        anim.Sprites.Add(spriteAsset.Clone());
                    }
                }
                anim.Loop = Loop;
                anim.OneShot = OneShot;
                return anim;
            }
        }
    }
}
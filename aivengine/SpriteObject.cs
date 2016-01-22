/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro

*/

using System.Collections.Generic;
using Aiv.Fast2D;
using OpenTK;

namespace Aiv.Engine
{
    public class SpriteObject : GameObject
    {
        private SpriteAsset currentSprite;

        public Dictionary<string, Animation> Animations { get; set; }

        public string CurrentAnimation { get; set; }

        public Sprite Sprite { get; set; }

        public SpriteAsset CurrentSprite
        {
            get { return currentSprite; }
            set
            {
                currentSprite = value;
            }
        }

        public Vector2 Pivot
        {
            get { return Sprite.pivot; }
            set { Sprite.pivot = value; }
        }

        public float Rotation
        {
            get { return Sprite.Rotation; }
            set { Sprite.Rotation = value; }
        }

        public float EulerRotation
        {
            get { return Sprite.EulerRotation; }
            set { Sprite.EulerRotation = value; }
        }

        public float Opacity { get; set; } = 1f;

        public float Width => Sprite.Width * Scale.X;

        public float Height => Sprite.Height * Scale.Y;

        public override Vector2 Scale
        {
            get { return base.Scale; }
            set
            {
                base.Scale = value;
                Sprite.scale = value;
            }
        }

        public Vector2 SpriteOffset { get; set; }
        public float BaseWidth => Sprite.Width;
        public float BaseHeight => Sprite.Height;

        public bool AutomaticHitBox { get; }

        public SpriteObject(int width, int height, bool automaticHitBox = false)
        {
            Sprite = new Sprite(width, height);
            AutomaticHitBox = automaticHitBox;
            if (automaticHitBox)
                AddHitBox("auto", 0, 0, 1, 1);
        }


        private void Animate(string animationName)
        {
            var animation = Animations[animationName];
            var neededTime = 1f/animation.Fps;

            if (Time - animation.LastTick >= neededTime && !animation.Locked)
            {
                animation.LastTick = Time;
                animation.CurrentFrame++;
                // end of the animation ?
                var lastFrame = animation.Sprites.Count - 1;
                if (animation.CurrentFrame > lastFrame)
                {
                    if (animation.Loop)
                    {
                        animation.CurrentFrame = 0;
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
                        animation.CurrentFrame = lastFrame;
                    }
                }
            }
            // simply draw the current frame
            var spriteAssetToDraw = animation.Sprites[animation.CurrentFrame];

            DrawSprite(spriteAssetToDraw);
        }

        private void DrawSprite(SpriteAsset sprite)
        {
            Sprite.position.X = DrawX;
            Sprite.position.Y = DrawY;
            sprite.Texture.SetOpacity(Opacity);
            Sprite.DrawTexture(
                sprite.Texture,
                (int) (sprite.X + SpriteOffset.X), (int) (sprite.Y + SpriteOffset.Y), 
                sprite.Width, sprite.Height);
            UpdateAutomaticHitBox(sprite);
        }

        public virtual void UpdateAutomaticHitBox(SpriteAsset sprite)
        {
            if (AutomaticHitBox)
            {
                if (HitBoxes == null || !HitBoxes.ContainsKey("auto"))
                    AddHitBox("auto", 0, 0, 1, 1);
                var hitBoxInfo = sprite.CalculateRealHitBox();
                HitBoxes["auto"].X = hitBoxInfo.Item1.X;
                HitBoxes["auto"].Y = hitBoxInfo.Item1.Y;
                HitBoxes["auto"].Width = (int)hitBoxInfo.Item2.X;
                HitBoxes["auto"].Height = (int)hitBoxInfo.Item2.Y;
            }
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
                DrawSprite(CurrentSprite);
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            Sprite.Dispose();
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
            animation.CurrentFrame = 0;
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
            var go = new SpriteObject((int) Width, (int) Height);
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
            public int CurrentFrame { get; set; }
            public SpriteObject owner;
            public float Fps { get; set; }
            public List<SpriteAsset> Sprites { get; internal set; }
            public float LastTick { get; internal set; }
            public bool Loop { get; set; }
            public bool OneShot { get; set; }
            public bool Locked { get; set; }

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
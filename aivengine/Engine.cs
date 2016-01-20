/*

Copyright 2015 20tab S.r.l.
Copyright 2015 Aiv S.r.l.
Forked by Luciano Ferraro 

*/

#define FAST2D

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using Aiv.Fast2D;
using Aiv.Vorbis;
using OpenTK.Audio;

namespace Aiv.Engine
{
    public class Engine
    {
        public delegate void AfterUpdateEventHandler(object sender);

        public delegate void BeforeUpdateEventHandler(object sender);

        public const float MaxDeltaTime = 0.33f;
        
        public bool debugCollisions;
        private Dictionary<GameObject.HitBox, RectangleObject> debugCollisionsBoxes;

        // objects that need to be added (1) or removed (0) from sortedObjects
        // objects that have changed order (2)
        private Dictionary<GameObject, int> dirtyObjects;

        private int totalObjCount;

        private readonly Window Window;

        protected Engine()
        {
        }

        public Engine(string windowName, int width, int height, int fps, bool fullscreen)
        {
            Initialize();

            // FPS?
            Window = new Window(width, height, windowName, fullscreen);
        }

        public TimerManager Timer { get; private set; }

        public Dictionary<string, Asset> Assets { get; private set; }

        public Camera Camera { get; set; }

        public bool ClearEveryFrame { get; set; } = true;

        public int Height => Window.Height;

        public bool IsGameRunning { get; set; }

        public Joystick[] Joysticks { get; private set; }

        public int MouseX => Window.mouseX;

        public int MouseY => Window.mouseY;

        public bool MouseLeft => Window.mouseLeft;

        public bool MouseRight => Window.mouseRight;

        public bool MouseMiddle => Window.mouseMiddle;

        public Dictionary<string, GameObject> Objects { get; private set; }

        public SortedSet<GameObject> SortedObjects { get; private set; }
        public float UnchangedTime { get; private set; }
        public float Time { get; private set; }

        // time modifier for deltaTime and deltaTicks
        public float TimeModifier { get; set; } = 1f;

        public int Width => Window.Width;

        public float DeltaTime { get; private set; }
        public float UnchangedDeltaTime { get; private set; }

        public event AfterUpdateEventHandler OnAfterUpdate;

        public event BeforeUpdateEventHandler OnBeforeUpdate;


        // put in fast2d?
        // rewrite / use aiv-vorbis
        //private object PlaySoundThread(string assetName, bool loop, float volume = 1f)
        //{
        //    var fileName = GetAsset(assetName).FileName;
        //    var ext = fileName.Substring(fileName.LastIndexOf(@".", StringComparison.Ordinal) + 1);

        //    if (ext != "ogg")
        //        throw new NotImplementedException($"Support for audio extension '{ext}' is not implemented.");
        //    try
        //    {
        //        var stream = new OggStream(fileName);
        //        stream.IsLooped = loop;
        //        stream.Volume = volume;
        //        stream.Prepare();
        //        stream.Play();
        //        return stream;
        //    }
        //    catch 
        //    {
        //        Console.WriteLine("AUDIO PLAY ERROR.");
        //    }
        //    return null;
        //}

        protected void GameUpdate()
        {
            Window.Update();

            UnchangedDeltaTime = Window.deltaTime;
            // having a deltatime higher than 0.33 means either that the game is having only 3fps or that something is loading
            // in both cases having a fixed maximum deltatime would make things smoother
            if (UnchangedDeltaTime > MaxDeltaTime)
                UnchangedDeltaTime = MaxDeltaTime;
            DeltaTime = UnchangedDeltaTime*TimeModifier;
            UnchangedTime += UnchangedDeltaTime;
            Time += DeltaTime;

            Timer.Update();

            OnBeforeUpdate?.Invoke(this);

            foreach (var obj in SortedObjects)
            {
                obj.UnchangedTime = UnchangedTime;
                obj.Time = Time;
                obj.UnchangedDeltaTime = UnchangedDeltaTime;
                obj.DeltaTime = DeltaTime;
                if (!obj.Enabled)
                    continue;
                obj.Draw();
                if (debugCollisions && obj.HitBoxes != null)
                {
                    foreach (var hitBox in obj.HitBoxes.Values)
                    {
                        if (hitBox.Width <= 0 || hitBox.Height <= 0)
                            continue;
                        if (!debugCollisionsBoxes.ContainsKey(hitBox) ||
                            hitBox.X != debugCollisionsBoxes[hitBox].X || hitBox.Y != debugCollisionsBoxes[hitBox].Y ||
                            hitBox.Width != debugCollisionsBoxes[hitBox].Width || hitBox.Height != debugCollisionsBoxes[hitBox].Height)
                            debugCollisionsBoxes[hitBox] = new RectangleObject(hitBox.Width, hitBox.Height)
                            {
                                Color = Color.Green
                            };
                        var rectangle = debugCollisionsBoxes[hitBox];
                        rectangle.X = obj.DrawX + hitBox.X;
                        rectangle.Y = obj.DrawY + hitBox.Y;
                        rectangle.Draw();
                    }
                }
            }

            OnAfterUpdate?.Invoke(this);

            if (dirtyObjects.Count > 0)
            {
                var newObjects = dirtyObjects.ToArray();
                dirtyObjects.Clear();
                foreach (var pair in newObjects)
                {
                    if (pair.Value == 1)
                    {
                        pair.Key.Time = Time;
                        SortedObjects.Add(pair.Key);
                    }
                    else if (pair.Value == 0)
                        SortedObjects.Remove(pair.Key);
                    else if (pair.Value == 2) // order changed
                    {
                        SortedObjects.Remove(pair.Key);
                        SortedObjects.Add(pair.Key);
                    }
                }
            }
        }


        protected void Initialize()
        {
            Camera = new Camera();

            // create dictionaries
            Objects = new Dictionary<string, GameObject>();
            SortedObjects = new SortedSet<GameObject>(new GameObjectComparer());
            Assets = new Dictionary<string, Asset>();

            dirtyObjects = new Dictionary<GameObject, int>();

            Joysticks = new Joystick[8];

            debugCollisionsBoxes = new Dictionary<GameObject.HitBox, RectangleObject>();

            Timer = new TimerManager(this);
        }


        public void DestroyAllObjects()
        {
            foreach (var obj in Objects.Values)
            {
                obj.Destroy();
            }
            // redundant now, could be useful in the future
            Objects.Clear();
            SortedObjects.Clear();
        }

        public Asset GetAsset(string name)
        {
            return Assets[name];
        }

        // keyboard management
        public bool IsKeyDown(KeyCode key)
        {
            return Window.GetKey(key);
        }

        /*
		 * 
		 * Asset's management
		 * 
		 */

        public void LoadAsset(string name, Asset asset)
        {
            asset.Engine = this;
            Assets[name] = asset;
        }

        //public virtual void PlaySound(string assetName, float volume = 1f, bool loop = false)
        //{
        //    AudioAsset audioAsset = (AudioAsset) GetAsset(assetName);
        //    audioSource.Volume = volume; // same volume for everything? :( more audio sources? effect - soundtrack - etc.
        //    audioSource.Play(audioAsset.Clip, loop);
        //}

        //public virtual void PlaySoundLoop(string assetName, float volume = 1f)
        //{
        //    PlaySound(assetName, volume, true);
        //}

        public void RemoveObject(GameObject obj)
        {
            if (debugCollisions && obj.HitBoxes != null)
                foreach (var hitBox in obj.HitBoxes.Values)
                    if (debugCollisionsBoxes.ContainsKey(hitBox))
                        debugCollisionsBoxes.Remove(hitBox);

            Objects.Remove(obj.Name);
            dirtyObjects[obj] = 0;
        }

        public void Run()
        {

            IsGameRunning = true;

            // compute update frequency
            //int freq = 1000 / this.fps;

            while (IsGameRunning && Window.opened)
            {
                GameUpdate();
                if (Window.opened == false)
                    IsGameRunning = false;
            }
            IsGameRunning = false;
            // check if we need to slowdown
            //if (endTick - startTick < freq)
            //{
            //    Thread.Sleep(freq - (endTick - startTick));
            //}
        }

        public void SpawnObject(GameObject obj)
        {
            SpawnObject(obj.Name, obj);
        }

        /* 
		 * 
		 * GameObject's management
		 * 
		 */

        public void SpawnObject(string name, GameObject obj)
        {
            obj.Name = name;
            obj.Engine = this;
            obj.Enabled = true;
            obj.Id = totalObjCount++;
            Objects[name] = obj;
            dirtyObjects[obj] = 1;
            obj.Initialize();
        }

        public void UpdatedObjectOrder(GameObject gameObject)
        {
            dirtyObjects[gameObject] = 2;
        }

        public class Joystick
        {
            public bool[] buttons;
            public long id;
            public int index;
            public string name;
            public int x;
            public int y;

            public Joystick()
            {
                // max 20 buttons
                buttons = new bool[20];
            }

            public bool anyButton()
            {
                foreach (var pressed in buttons)
                {
                    if (pressed)
                        return true;
                }
                return false;
            }

            public bool AnyButton()
            {
                return anyButton();
            }

            public int GetAxis(int axisIndex)
            {
                if (axisIndex < 0 || axisIndex > 1)
                    throw new ArgumentOutOfRangeException($"Wrong axis index '{axisIndex}', only 0 and 1 are supported.");
                return axisIndex == 0 ? x : y;
            }

            public bool GetButton(int buttonIndex)
            {
                return buttons[buttonIndex];
            }
        }
    }
}
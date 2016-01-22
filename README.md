Simple engine that uses aiv-fast2d.

Check out Example project (uses TextObject and repeating sprite):
https://github.com/luxkun/aivengine-1/blob/master/Example/Program.cs

Check out Futuridium for example project:
https://github.com/luxkun/Futuridium

Example: 
```cs
Engine engine = new Engine("window's title", 1024, 768, fps_int, fullscreen_bool);

GameObject object = new ClassThatInheritGameObject(); 
engine.SpawnObject("name_of_object", object);

// can do also: new SpriteAsset("filename.png") if the file doesn't have tiles 
SpriteAsset spriteAsset = new SpriteAsset("fileName.png", offset_x, offset_y, width, height);
engine.LoadAsset("name_of_asset", spriteAsset);

SpriteObject sprite = new ClassThatInheritSprite(); 
sprite.CurrentSprite = Engine.GetAsset("name_of_asset"); 
engine.SpawnObject("name_of_spriteobject", sprite);

engine.Run();
```

Userful variables: 
```cs
// ENGINE
float Engine.DeltaTime 
float Engine.UnchangedDeltaTime
float Engine.Time 
float Engine.TimeModifier //(change the real time to this modifier, if set to 0 the DeltaTime of all items won't change)
int Engine.Width
int Engine.Height
Engine.IsKeyDown(KeyCode) //-> returns true if KeyCode is being pressed
Engine.LoadAsset("asset_name", Asset) //-> adds Asset with name "asset_name" to engine's assets
Engine.GetAsset("asset_name") //-> returns Asset with name "asset_name"
Engine.Run() //-> runs the game
TimerManager Engine.Timer //-> same as GameObject.Timer

// GAMEOBJECT
Engine GameObject.Engine
float GameObject.X
float GameObject.Y
float GameObject.DrawX //(used when you need the camera) 
float GameObject.DrawY 
float GameObject.DeltaTime 
float GameObject.Time //(total time since the gameobject has been spawned) 
float GameObject.UnchangedTime //(userful when the Engine.TimeModifier is changed, ex.: pause) 
float GameObject.IgnoreCamera //(ignores camera position, userful for hud etc.) 
int GameObject.Order //(order of the gameobject in the draw, if object A has lower order than object B then object B is going to be drawn after object A) 
AudioSource GameObject.AudioSource //(aiv-vorbis audiosource, can play with "AudioSource.Play(AudioClip);") 
bool GameObject.Enabled //(if false the object won't be drawn) 
GameObject.AddHitBox("nameOfHitBox", offset_x, offset_y, width, height) //-> adds an hitbox to the gameobject
GameObject.CheckCollisions() //-> returns a List of hitsboxes that this gameobject is colliding
TimerManager GameObject.Timer // check class
GameObject.Destroy() //-> removes the gameobject from the engine
//Methods to override:
// - Update
// - Start
//If you want to handle Destroy you can do this:
OnDestroy += DestroyEvent;
public void DestroyEvent(object sender) {
    // do stuff
}

//TIMER
TimerManager.Set("name_of_key", float_expire_time)
TimerManager.Get("name_of_key") -> returns true if "float_expire_time" time has passed, else returns false

// SPRITEOBJECT -> INHERIT GAMEOBJECT
SpriteObject(int width, int height, bool automaticHitBox)
bool AutomaticHitBox
// if automatichitbox is true then an hitbox "auto" is added to the sprite, which is automatically updated every time the sprite
//  is changed, works even with animations
SpriteAsset CurrentSprite //-> the current drawn SpriteAsset
string CurrentAnimation //-> if using animations this should be changed to the wanted animation key
float Width/Height // Width and Height, if the sprite is scaled the width and height are also scaled
float BaseWidth/BaseHeight // returns the unscaled width and height, userful only if the sprite is scaled
float Opacity // set to change opacity  modifier on all alpha channels of the sprite's textures, heavy
bool RepeatX // set repeatX on all textures
bool RepeatY // set repeatY on all textures
float Rotation
float EulerRotation
AddAnimation("animation_key", List<string> AssetsKeys, int fps)

// SPRITEASSET -> INHERIT ASSET
X // the x offset in the texture, basically the x you put in DrawTexture in aiv-fast2d
Y // the y offset in the texture, basically the y you put in DrawTexture in aiv-fast2d
Width // the width of the sprite
Height // ...
// if you want to use fast2d's feature repeatx/y just put repeatx: true, repeaty: true when initializating the spriteasset
CalculateRealHitBox() //-> returns two Vector2, one have the real offsets, ignoring trasparency in the sprite, the other one have sizes
// Example of usage: 
/*
var tuple = spriteasset.CalculateRealHitBox()
AddHitBox("mass", tuple.Item1.X, tuple.Item1.Y, tuple.Item2.X, tuple.Item2.Y)
*/
```

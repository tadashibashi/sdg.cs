using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDG;

public abstract partial class Scene
{
    /// <summary>
    /// Scene management, start / stop scenes, etc.
    /// This is initialized by the Scene.Manager
    /// </summary>
    protected Manager Scenes { get; private set; }

    /// <summary>
    /// Content manager for this scene.
    /// This is initialized by the Scene.Manager
    /// </summary>
    protected SdgContentManager Content { get; private set; }
    
    /// <summary>
    /// This event occurs on scene start - Scenes.CurrentScene during this callback will be the last scene,
    /// or null if this is the first one.
    /// </summary>
    protected abstract void Start();
    
    /// <summary>
    /// This event occurs once every update tick, which should occur at ~60fps on most modern systems.
    /// `Draw` comes after this event.
    /// </summary>
    /// <param name="time"></param>
    protected abstract void Update(GameTime time);
    
    /// <summary>
    /// This scene event occurs once every update tick, after the `Update` event.
    /// </summary>
    /// <param name="time">time struct containing delta time and total time info</param>
    /// <param name="batch">sprite batch to draw with</param>
    protected abstract void Draw(GameTime time, SpriteBatch batch);
    
    /// <summary>
    /// This event occurs when this scene ends as the result of calling `StopCurrentScene` while this one is running.
    /// </summary>
    protected abstract void End();
    
    /// <summary>
    /// This event occurs on scene pause. Another has taken its place, but has not called for it to be
    /// removed/ended. Once that scene ends, this one will resume. Scene.CurrentScene will be this scene.
    /// </summary>
    protected abstract void Paused();
    
    /// <summary>
    /// This event occurs when this scene is resumed, when the scene on top of this one has stopped.
    /// Scene.CurrentScene will be the last scene.
    /// </summary>
    protected abstract void Resumed();
}
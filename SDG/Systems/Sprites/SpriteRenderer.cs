using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using SDG.Systems.Sprites;

namespace SDG.Systems;

public readonly record struct Sprite
{
    private readonly SpriteFrame[] _frames;
    private readonly int[] _reel;
    public float FrameRate { get; init; }

    public Sprite() : this(Array.Empty<SpriteFrame>(), Array.Empty<int>(), 0)
    {
    }
    
    /// <summary>
    /// Create a Sprite with a custom animation reel that selects frame indices for each frame
    /// </summary>
    /// <param name="frames">Sprite frames</param>
    /// <param name="reel">Indices of sprite frames to display</param>
    /// <param name="frameRate">Frames per second to play reel</param>
    public Sprite(SpriteFrame[] frames, int[] reel, float frameRate = 5.0f)
    {
        _frames = frames;
        _reel = reel;
        FrameRate = frameRate;
    }

    /// <summary>
    /// Create a Sprite from a list of frames
    /// </summary>
    /// <param name="frames">Sprite frames in animation order</param>
    /// <param name="frameRate">Frames per second to play animation</param>
    public Sprite(SpriteFrame[] frames, float frameRate = 5.0f) : this(frames, new int[frames.Length], frameRate)
    {
        // setup reel to default
        for (var i = 0; i < frames.Length; ++i)
        {
            _reel[i] = i;
        }
    }
    
    public Sprite(SpriteFrame frame) : this(new[] { frame }, new[] { 0 }, 0)
    {
    }

    public int Length => _reel.Length;
    public SpriteFrame this[int index] => _frames[_reel[index]];
}

/// <summary>
/// Static image useful for images that don't get updated as frequently or need an automatic animator
/// </summary>
public record StaticSpriteComponent
{
    public SpriteFrame Frame { get; set; }
    public Color Color { get; set; } = Color.White;
}

public record SpriteComponent
{
    /// <summary>
    /// Sprite to display
    /// </summary>
    public Sprite Sprite { get; set; }
    
    /// <summary>
    /// Color tint
    /// </summary>
    public Color Color { get; set; } = Color.White;
    
    /// <summary>
    /// Current frame index state in the Sprite
    /// </summary>
    public float Index { get; set; } = 0;
    
    /// <summary>
    /// Multiplier on framerate, default is 1
    /// </summary>
    public float Speed { get; set; } = 1.0f;
    
    /// <summary>
    /// Whether the sprite animation should be paused
    /// </summary>
    public bool Paused { get; set; } = false;

    public SpriteFrame CurrentFrame => Sprite[(int)Index];
}


public class SpriteRenderer
{
    private EntityGroup<Entity, SpriteComponent, Transform2D> _group;
        
    public void Initialize(EntityContext<Entity> entities)
    {
        _group = new(entities);
    }

    public void Close()
    {
        _group = null;
    }
    
    public void Update(GameTime time)
    { 
        var deltaSeconds = (float)time.ElapsedGameTime.TotalSeconds;
        foreach (var (e, sprite, transform) in _group)
        {
            sprite.Index = Mathf.Modf(sprite.Index + sprite.Speed * deltaSeconds * sprite.Sprite.FrameRate, sprite.Sprite.Length);
            
            
        }
    }

    public void Draw(GameTime time, SpriteBatch batch)
    {
        foreach (var (e, sprite, transform) in _group)
        {
            if (sprite.Sprite.Length == 0) continue;
            var frame = sprite.CurrentFrame;
            
            batch.Draw(frame.Texture, transform.Position, frame.Frame, sprite.Color, 
                MathHelper.ToRadians(transform.Rotation + (frame.Rotated ? -90 : 0)), 
                frame.Pivot, transform.Scale,
                SpriteEffects.None, transform.Depth);
        }
    }
}
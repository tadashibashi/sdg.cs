using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDG;

public class Viewport2D
{
    public Viewport Viewport
    {
        get => _viewport;
        set
        {
            _viewport = value;
            Camera.ScreenBounds = value.Bounds;
            ViewportChanged?.Invoke(value);
        }
    }
    
    public Camera2D Camera { get; }
    
    private Viewport _viewport;

    public event Action<Viewport> ViewportChanged;

    public Viewport2D(Viewport viewport)
    {
        _viewport = viewport;
        Camera = new Camera2D
        {
            ScreenBounds = _viewport.Bounds,
        };
    }
}
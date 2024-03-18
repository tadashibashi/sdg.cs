using System;
using Microsoft.Xna.Framework;

namespace SDG;

/// <summary>
/// 2-Dimensional camera that supports translation, zoom/scale, and rotation
/// </summary>
public class Camera2D
{
    private Matrix _matrix = Matrix.Identity;
    private Matrix _invert;
    private bool _isDirty = true;
    private RectangleF? _worldBounds;
    
    /// <summary>
    /// Position of the camera
    /// </summary>
    public Vector2 Position {
        set
        {
            if (value != _position)
            {
                _isDirty = true;
                _position = value;
            }
        }
        get => _position;
    }
    private Vector2 _position = Vector2.Zero;
    
    /// <summary>
    /// Point in normalized view coordinates upon which the camera rotates and zooms in-out of.
    /// This is also the point in the view of where the position is set.
    /// The point is calculated as: Origin x Bounds.Size.
    /// Default: {X:0.5f, Y: 0.5f}
    /// </summary>
    public Vector2 Origin {
        set
        {
            if (value != _origin)
            {
                _isDirty = true;
                _origin = value;
            }
        }
        get => _origin;
    }
    private Vector2 _origin = new Vector2(0.5f, 0.5f);
    
    /// <summary>
    /// Screen bounds of the camera view. This is set automatically if this camera is owned by a Viewport2D.
    /// </summary>
    public Rectangle ScreenBounds { get; set; }
    
    public RectangleF WorldBounds
    {
        get
        {
            if (_worldBounds == null)
            {
                var pos = ViewToWorld(Vector2.Zero);
                var size = ViewToWorld(new Vector2(ScreenBounds.Width, ScreenBounds.Height));
                _worldBounds = new RectangleF(pos, size);

            }

            return _worldBounds.Value;
        }
    }

    public Vector2 Zoom {
        set
        {
            if (value != _zoom)
            {
                _isDirty = true;
                _zoom = value;
            }
        }
        get => _zoom;
    }
    private Vector2 _zoom = Vector2.One;

    public float Rotation {
        set
        {
            if (Math.Abs(value - _rotation) > float.Epsilon)
            {
                _isDirty = true;
                _rotation = value;
            }
        }
        get => _rotation;
    }
    private float _rotation = 0;

    /// <summary>
    /// Relatively move the camera
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="degrees"></param>
    public void Move(float distance, float degrees)
    {
        degrees -= _rotation;
        const double constant = Math.PI / 180.0;
        Position += new Vector2(
            (float)Math.Cos(degrees * constant),
            (float)Math.Sin(degrees * constant)
        );
    }
    
    /// <summary>
    /// Center the view on a position in world space. Sets the origin to center {X=0.5f, Y=0.5f}
    /// </summary>
    /// <param name="position">Position in world space</param>
    public void LookAt(Vector2 position)
    {
        Origin = new Vector2(0.5f, 0.5f);
        Position = position;
    }

    public Vector2 ViewToWorld(Vector2 screen)
    {
        ApplyChanges();
        return Vector2.Transform(screen, _matrix);
    }

    public Vector2 WorldToView(Vector2 world)
    {
        ApplyChanges();
        return Vector2.Transform(world, _invert);
    }

    public Matrix Matrix
    {
        get
        {
            ApplyChanges();
            return _matrix;
        }
    }

    private void ApplyChanges()
    {
        if (!_isDirty) return;

        var matrix = Matrix.CreateTranslation(new Vector3(-_position, 0)) *
                     Matrix.CreateScale(new Vector3(_zoom, 1)) *
                     Matrix.CreateRotationZ(_rotation) *
                     Matrix.CreateTranslation(new Vector3(_origin.X * ScreenBounds.Width, _origin.Y * ScreenBounds.Height, 0));

        _matrix = matrix;
        _invert = Matrix.Invert(matrix);
        _isDirty = false;
        _worldBounds = null;
    }
}
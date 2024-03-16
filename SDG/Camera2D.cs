using System;
using Microsoft.Xna.Framework;

namespace SDG;

public class Camera2D
{
    private Matrix _matrix = Matrix.Identity;
    private Matrix _invert;
    private bool _isDirty = true;

    /// <summary>
    /// The minimum X and Y position that the camera can view
    /// </summary>
    public Vector2 BoundsMin { set; get; } = new();
    
    /// <summary>
    /// The maximum X and Y position that the camera can view.
    /// If less than the view port or screen, than the camera will prefer the greater value.
    /// </summary>
    public Vector2 BoundsMax { set; get; } = new();
    
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
    
    public Vector2 Size {
        set
        {
            if (value != _size)
            {
                _isDirty = true;
                _size = value;
            }
        }
        get => _size;
    }
    private Vector2 _size = new Vector2(640, 480); // FIXME: auto set this to a viewport's size, once class setup
    
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
    /// Center the view on a position in world space
    /// </summary>
    /// <param name="position">Position in world space</param>
    public void LookAt(Vector2 position)
    {
        Position = (_size * 0.5f) - (_size * _origin) + position;
    }

    public Vector2 ScreenToWorld(Vector2 screen)
    {
        ApplyChanges();
        return Vector2.Transform(screen, _matrix);
    }

    public Vector2 WorldToScreen(Vector2 world)
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
                     Matrix.CreateTranslation(new Vector3(_origin * _size, 0));

        _matrix = matrix;
        _invert = Matrix.Invert(matrix);
        _isDirty = false;
    }
}
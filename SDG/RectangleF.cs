using Microsoft.Xna.Framework;

namespace SDG;

/// <summary>
/// Axis-aligned floating-point rectangle, that matches the API for Xna's Rectangle, with a few extensions.
/// </summary>
/// <param name="X"></param>
/// <param name="Y"></param>
/// <param name="Width"></param>
/// <param name="Height"></param>
public record struct RectangleF(float X, float Y, float Width, float Height)
{
    public RectangleF(Vector2 position, Vector2 size) : this(position.X, position.Y, size.X, size.Y)
    {
    }

    public RectangleF(float x, float y, Vector2 size) : this(x, y, size.X, size.Y)
    {
    }

    public RectangleF(Vector2 position, float width, float height) : this(position.X, position.Y, width, height)
    {
    }

    public static explicit operator RectangleF(Rectangle other)
    {
        return new RectangleF
        {
            X = other.X,
            Y = other.Y,
            Width = other.Width,
            Height = other.Height,
        };
    }

    public bool Intersects(RectangleF other)
    {
        return !(Area == 0 || other.Area == 0 || 
                 Right < other.Left || Bottom < other.Top || Left > other.Right || Top > other.Bottom);
    }

    public bool Intersects(Rectangle other)
    {
        return !(other.Width * other.Height == 0 || Area == 0 ||
                 Right < other.Left || Bottom < other.Top || Left > other.Right || Top > other.Bottom);
    }

    public bool Contains(Vector2 point)
    {
        return !(Area == 0 || point.X < Left || point.X > Right || point.Y < Top || point.Y > Bottom);
    }

    public bool Contains(Point point)
    {
        return !(Area == 0 || point.X < Left || point.X > Right || point.Y < Top || point.Y > Bottom);
    }

    public float Area => Width * Height;

    public Vector2 Size
    {
        get => new Vector2(Width, Height);
        set
        {
            Width = value.X;
            Height = value.Y;
        }
    }

    public Vector2 Location
    {
        get => new Vector2(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public float Left
    {
        get => X;
        set => X = value;
    }

    public float Top
    {
        get => Y;
        set => Y = value;
    }

    public float Right
    {
        get => X + Width;
        set
        {
            if (value < X)
            {
                X = value;
                Width = 0;
            }
            else
            {
                Width = value - X;
            }
        }
    }

    public float Bottom
    {
        get => Y + Height;
        set
        {
            if (value < Y)
            {
                Y = value;
                Height = 0;
            }
            else
            {
                Height = value - Y;
            }
        }
    }

    public Vector2 Center
    {
        get => new Vector2(Width * 0.5f + X, Height * 0.5f + Y);
        set
        {
            X = value.X - Width * 0.5f;
            Y = value.Y - Height * 0.5f;
        }
    }

    public Vector2 TopLeft
    {
        get => Location;
        set => Location = value;
    }

    public Vector2 TopRight
    {
        get => new Vector2(X + Width, Y);
        set
        {
            Right = value.X;
            Top = value.Y;
        }
    }

    public Vector2 BottomLeft
    {
        get => new Vector2(X, Y + Height);
        set
        {
            Left = value.X;
            Bottom = value.Y;
        }
    }

    public Vector2 BottomRight
    {
        get => new Vector2(X + Width, X + Height);
        set
        {
            Right = value.X;
            Left = value.Y;
        }
    }
}

public static class XnaRectangleExtensions
{
    public static RectangleF ToRectangleF(this Rectangle rect)
    {
        return new RectangleF(rect.X, rect.Y, rect.Width, rect.Height);
    }
}
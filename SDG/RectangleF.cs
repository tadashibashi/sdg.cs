using Microsoft.Xna.Framework;

namespace SDG;

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

    public Vector2 Size
    {
        get => new Vector2(Width, Height);
        set
        {
            Width = value.X;
            Height = value.Y;
        }
    }

    public Vector2 Position
    {
        get => new Vector2(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
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
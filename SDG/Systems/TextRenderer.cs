using System.Security.Cryptography.X509Certificates;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDG.Systems;


public record Transform2D
{
    public Vector2 Position { set; get; } = Vector2.Zero;
    public Vector2 Scale { set; get; } = Vector2.One;
    public float Rotation { set; get; } = 0;
    public float Depth { get; set; } = 0;
}

public record TextComponent
{
    public string Text
    {
        get => _text;
        set
        {
            if (value == _text) return;
            _size = null;
            _text = value;
        }
    }
    private string _text;

    public int CharSpacing { get; set; } = 0;
    public int LineSpacing { get; set; } = 0;

    public Color Color
    {
        get => Colors.Length == 0 ? Color.White : Colors[0];
        set
        {
            if (Colors.Length == 1)
            {
                Colors[0] = value;
            }
            else
            {
                Colors = new[] { value };
            }
        }
    }
    public Color[] Colors { get; set; } = { Color.White };

    public Vector2 Origin { get; set; } = new(0.5f, 0.5f);

    public SpriteFontBase Font
    {
        get => _font;
        set
        {
            if (value == _font) return;
            _size = null;
            _font = value;
        }
    }

    public int StrokeWidth { get; set; } = 0;

    private SpriteFontBase _font;

    /// <summary>
    /// Size of the text without transformations applied
    /// </summary>
    public Vector2 Size
    {
        get
        {
            if (_size == null)
            {
                if (_font == null)
                    return Vector2.Zero;
                _size = _font.MeasureString(_text, null, 0, 0, FontSystemEffect.Stroked, StrokeWidth);
            }

            return _size.Value;
        }
    }
    private Vector2? _size;
}

public class TextRenderer
{
    private readonly EntityGroup<Entity, TextComponent, Transform2D> _renderables; 
    
    public TextRenderer(EntityContext entities)
    {
        _renderables = new(entities);
    }
    
    public void Draw(SpriteBatch batch)
    {
        foreach (var (e, text, transform) in _renderables)
        {
            batch.DrawString(text.Font, text.Text, transform.Position, text.Colors,
                MathHelper.ToRadians(transform.Rotation), text.Size * text.Origin, transform.Scale, transform.Depth, 
                text.CharSpacing, text.LineSpacing, TextStyle.None, 
                FontSystemEffect.Stroked, text.StrokeWidth);
        }
    }
}
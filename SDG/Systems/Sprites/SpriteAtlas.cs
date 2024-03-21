using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;

namespace SDG.Systems.Sprites;

internal record struct SpriteRect
{
    public int X { get; set; }
    public int Y { get; set; }
    public int W { get; set; }
    public int H { get; set; }
    
    public static implicit operator Rectangle(SpriteRect rect)
    {
        return new Rectangle(rect.X, rect.Y, rect.W, rect.H);
    }

    public Point Location => new Point(X, Y);
    public Point Size => new Point(W, H);
}

internal record struct SpriteSize
{
    public int W { get; set; }
    public int H { get; set; }
}

internal record struct SpriteVec2
{
    public float X { get; set; }
    public float Y { get; set; }
}

/// <summary>
/// Metadata included in TexturePacker JSON output files
/// </summary>
internal record struct SpriteMeta
{
    /// <summary>
    /// Link to application website: "https://www.codeandweb.com/texturepacker"
    /// </summary>
    public string App { get; set; }
    /// <summary>
    /// Version string of this JSON format
    /// </summary>
    public string Version { get; set; }
    /// <summary>
    /// Relative path to sprite sheet image
    /// </summary>
    public string Image { get; set; }
    /// <summary>
    /// Image format string: e.g. "RGBA8888"
    /// </summary>
    public string Format { get; set; }
    /// <summary>
    /// Size of the source image in pixels
    /// </summary>
    public SpriteSize Size { get; set; }
    /// <summary>
    /// Image scale (unused)
    /// </summary>
    public string Scale { get; set; }
    /// <summary>
    /// TexturePacker update functionality
    /// </summary>
    public string SmartUpdate { get; set; }
}

internal record struct TexturePackerArrayFile
{
    internal record struct SpriteFrame
    {
        public string Filename { get; set; }
        public SpriteRect Frame { get; set; }
        public bool Rotated { get; set; }
        public bool Trimmed { get; set; }
        public SpriteRect SpriteSourceSize { get; set; }
        public SpriteSize SourceSize { get; set; }
        public SpriteVec2 Pivot { get; set; }
    }

    public SpriteFrame[] Frames { get; set; }
    public SpriteMeta Meta { get; set; }
}

internal record struct TexturePackerHashFile
{
    internal record struct SpriteFrame
    {
        public SpriteRect Frame { get; set; }
        public bool Rotated { get; set; }
        public bool Trimmed {get; set; }
        public SpriteRect SpriteSourceSize {get; set; }
        public SpriteSize SourceSize {get; set; }
        public SpriteVec2 Pivot {get; set; }
    }

    public Dictionary<string, SpriteFrame> Frames { get; }
    public SpriteMeta Meta { get; }
}

/// <summary>
/// Frame from a SpriteAtlas representing a frame sub-image
/// </summary>
/// <param name="Texture">Texture image this frame is a subimage of</param>
/// <param name="Frame">Location and dimensions of subframe in pixels</param>
/// <param name="Original">
///     Original location info, subtrack X and Y from the frame to get the original frame.
///     Width and height represent the original image's width and height.
/// </param>
/// <param name="Rotated">Whether the frame is rotated + 90 degrees</param>
/// <param name="Trimmed">
///     Whether the frame has been trimmed from its original dimensions. See `Original` to get the
///     original X and Y.
/// </param>
/// <param name="Pivot">
///     Pivot / origin point, from where position will be set, and where image will be rotated / scaled
/// </param>
public readonly record struct SpriteFrame(
    Texture2D Texture,
    Rectangle Frame,
    Rectangle Original,
    bool Rotated,
    bool Trimmed,
    Vector2 Pivot);


public class SpriteAtlas
{
    private Dictionary<string, SpriteFrame> _frames = new();
    private Texture2D _texture;

    public bool FromTexturePackerJson(GraphicsDevice graphics, string filepath)
    {
        try
        {
            Dictionary<string, SpriteFrame> dict = new();
            Texture2D texture;
            
            // load json info
            using (var fileStream = TitleContainer.OpenStream(filepath))
            {
                using var fileReader = new StreamReader(fileStream);
                var fileString = fileReader.ReadToEnd();
                
                // check json object fields
                var json = JObject.Parse(fileString);
                var jsonFrames = json["frames"];
                if (jsonFrames == null)
                {
                    Console.Error.WriteLine("Invalid TexturePacker JSON: missing `frames` field");
                    return false;
                }
                
                var jsonMeta = json["meta"];
                if (jsonMeta == null)
                {
                    Console.Error.WriteLine("Invalid TexturePacker JSON: missing `meta` field");
                    return false;
                }

                var meta = jsonMeta.ToObject<SpriteMeta>();
                
                // load texture
                var parentDirectory = Path.GetDirectoryName(filepath) ?? "";
                var imagePath = Path.Combine(parentDirectory, meta.Image);
            
                using (var imageFileStream = TitleContainer.OpenStream(imagePath))
                {
                    texture = Texture2D.FromStream(graphics, imageFileStream);
                }
                
                // get frames
                if (jsonFrames.Type == JTokenType.Object) // TexturePacker hash export file
                {
                    var tempFrameDict = jsonFrames.ToObject<Dictionary<string, TexturePackerHashFile.SpriteFrame>>();

                    foreach (var (key, frame) in tempFrameDict)
                    {
                        var frameRect = frame.Rotated
                            ? new Rectangle(frame.Frame.X, frame.Frame.Y, frame.Frame.H, frame.Frame.W)
                            : new Rectangle(frame.Frame.X, frame.Frame.Y, frame.Frame.W, frame.Frame.H);
                        var pivot = frame.Rotated
                            ? new Vector2(frameRect.Width + frame.SpriteSourceSize.Y - frame.Pivot.Y * frame.SourceSize.H, frame.Pivot.X * frame.SourceSize.W - frame.SpriteSourceSize.X)
                            : new Vector2(frame.Pivot.X * frame.SourceSize.W - frame.SpriteSourceSize.X, frame.Pivot.Y * frame.SourceSize.H - frame.SpriteSourceSize.Y);
                            
                        dict.TryAdd(key, 
                            new SpriteFrame(texture, frameRect,
                                new Rectangle(frame.SpriteSourceSize.X, frame.SpriteSourceSize.Y, 
                                    frame.SourceSize.W, frame.SourceSize.H),
                                    frame.Rotated,
                                    frame.Trimmed,
                                    pivot)
                        );
                    }
                }
                else if (jsonFrames.Type == JTokenType.Array) // TexturePacker array export file
                {
                    var tempFrameArr = jsonFrames.ToObject<TexturePackerArrayFile.SpriteFrame[]>();
                    foreach (var frame in tempFrameArr)
                    {
                        var frameRect = frame.Rotated
                            ? new Rectangle(frame.Frame.X, frame.Frame.Y, frame.Frame.H, frame.Frame.W)
                            : new Rectangle(frame.Frame.X, frame.Frame.Y, frame.Frame.W, frame.Frame.H);
                        var pivot = frame.Rotated
                            ? new Vector2(frameRect.Width + frame.SpriteSourceSize.Y - frame.Pivot.Y * frame.SourceSize.H, frame.Pivot.X * frame.SourceSize.W - frame.SpriteSourceSize.X)
                            : new Vector2(frame.Pivot.X * frame.SourceSize.W - frame.SpriteSourceSize.X, frame.Pivot.Y * frame.SourceSize.H - frame.SpriteSourceSize.Y);
                        
                        dict.TryAdd(frame.Filename, 
                            new SpriteFrame(texture, frameRect,
                                new Rectangle(frame.SpriteSourceSize.X, frame.SpriteSourceSize.Y, frame.SourceSize.W,
                                    frame.SourceSize.H),
                                frame.Rotated, 
                                frame.Trimmed,
                                pivot)
                        );
                    }
                }
                else
                {
                    Console.Error.WriteLine("Invalid TexturePacker JSON: invalid type for `frames` field");
                    return false;
                }
            }
            
            Dispose();
            _frames = dict;
            _texture = texture;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e);
            return false;
        }
        
        return true;
    }

    public SpriteFrame this[string key] => _frames[key];

    public void Dispose()
    {
        if (_texture != null)
        {
            _texture.Dispose();
            _texture = null;
        }

        _frames.Clear();
    }
}
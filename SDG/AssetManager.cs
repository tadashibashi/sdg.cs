using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Text;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SDG;

public class AssetManager
{
    private readonly GraphicsDevice _graphics;
    private readonly Dictionary<string, Texture2D> _textures = new();
    private readonly Dictionary<string, FontSystem> _fontSystems = new();
    private readonly Dictionary<string, SpriteFontBase> _bitmapFonts = new();

    public AssetManager(GraphicsDevice graphics)
    {
        _graphics = graphics;
    }
    
    public string RootDirectory { get; set; } = "Assets/";

    protected string MakeAssetPath(string filepath)
    {
        return Path.Combine(RootDirectory, filepath);
    }

    public Texture2D LoadTexture(string filepath)
    {
        var fullpath = MakeAssetPath(filepath);
        
        // try to get cached texture first
        if (_textures.TryGetValue(fullpath, out var outTexture))
        {
            return outTexture;
        }
        
        // load and cache texture
        using var stream = TitleContainer.OpenStream(fullpath);

        return _textures[fullpath] = Texture2D.FromStream(_graphics, stream);
    }

    public bool UnloadTexture(string filepath)
    {
        var fullpath = MakeAssetPath(filepath);
        if (_textures.TryGetValue(fullpath, out var texture))
        {
            texture.Dispose();
            _textures.Remove(fullpath);
            return true;
        }

        return false;
    }

    public SpriteFontBase LoadBitmapFont(string filepath)
    {
        var fullpath = MakeAssetPath(filepath);
        if (_bitmapFonts.TryGetValue(fullpath, out var outFont))
        {
            return outFont;
        }

        using var stream = TitleContainer.OpenStream(fullpath);
        using var reader = new StreamReader(stream);
        var fontData = reader.ReadToEnd();

        var font = StaticSpriteFont.FromBMFont(fontData,
            fileName => File.OpenRead(
                Path.Combine( Path.GetDirectoryName(fullpath) ?? "", fileName)), 
            _graphics);

        return _bitmapFonts[fullpath] = font;
    }
}
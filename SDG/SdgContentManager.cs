using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SDG;

public class SdgContentManager : ContentManager
{
    private GraphicsDevice GraphicsDevice
    {
        get
        {
            if (_graphics == null)
            {
                var service = (ServiceProvider.GetService(typeof(IGraphicsDeviceService)));
                if (service is IGraphicsDeviceService graphicsDeviceService)
                {
                    _graphics = graphicsDeviceService.GraphicsDevice;
                }
            }

            return _graphics;
        }   
    }
    private GraphicsDevice _graphics;
    
    private Dictionary<string, object> _loadedAssets;
    private Dictionary<string, object> LoadedAssets {
        get {
            if (_loadedAssets == null)
            {
                FieldInfo info; 
                var type = typeof(ContentManager);
                do
                {
                    info = type.GetField("loadedAssets",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    type = type.BaseType;
                } while (info == null && type != null);
                
                _loadedAssets = info!.GetValue(this) as Dictionary<string, object>;
            }

            return _loadedAssets;
        }
    }

    public SdgContentManager(Core game, string rootDirectory = "Content") 
        : base(game.Services, rootDirectory)
    {

    }

    private string MakeAssetPath(string filepath)
    {
        return Path.Combine(RootDirectory, filepath);
    }

    public Texture2D LoadTexture(string filepath)
    {
        var fullpath = MakeAssetPath(filepath);
        
        // try to get cached texture first
        if (LoadedAssets.TryGetValue(fullpath, out var outTexture))
        {
            return outTexture as Texture2D;
        }
        
        // load and cache texture
        using var stream = TitleContainer.OpenStream(fullpath);
        return (LoadedAssets[fullpath] = Texture2D.FromStream(GraphicsDevice, stream)) as Texture2D;
    }

    /// <summary>
    /// Unload a single content item
    /// </summary>
    /// <param name="filepath"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool UnloadAsset<T>(string filepath) where T : class, IDisposable
    {
        var fullpath = MakeAssetPath(filepath);
        if (LoadedAssets.TryGetValue(fullpath, out var asset))
        {
            if (asset is T disposable)
            {
                disposable.Dispose();
                LoadedAssets.Remove(fullpath);
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Unload all loaded asset files currently stored in this manager
    /// </summary>
    public override void Unload()
    {
        foreach (var (key, value) in LoadedAssets)
        {
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        LoadedAssets.Clear();
        base.Unload();
    }

    public SpriteFontBase LoadBitmapFont(string filepath)
    {
        var fullpath = MakeAssetPath(filepath);
        if (LoadedAssets.TryGetValue(fullpath, out var outFont))
        {
            if (outFont is SpriteFontBase spriteFontBase)
                return spriteFontBase;
            return null;
        }

        using var stream = TitleContainer.OpenStream(fullpath);
        using var reader = new StreamReader(stream);
        var fontData = reader.ReadToEnd();

        var font = StaticSpriteFont.FromBMFont(fontData,
            fileName => TitleContainer.OpenStream(
                Path.Combine( Path.GetDirectoryName(fullpath) ?? "", fileName)), 
            GraphicsDevice);

        LoadedAssets[fullpath] = font;
        return font;
    }
}
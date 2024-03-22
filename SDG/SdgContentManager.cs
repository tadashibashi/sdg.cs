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

    private SdgContentManager _parent;

    /// <summary>
    /// Times an asset is referenced by children / self
    /// </summary>
    private readonly Dictionary<string, int> _assetRefCount;
    
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

    public SdgContentManager(Core game, SdgContentManager parent = null, string rootDirectory = "Content") 
        : base(game.Services, rootDirectory)
    {
        _assetRefCount = new Dictionary<string, int>();
        _parent = parent;
    }

    private string MakeAssetPath(string filepath)
    {
        return Path.Combine(RootDirectory, filepath);
    }

    public Texture2D LoadTexture(string filepath)
    {
        var fullpath = MakeAssetPath(filepath);
        
        // check our cache first
        if (LoadedAssets.TryGetValue(fullpath, out var outAsset))
        {
            if (outAsset is Texture2D outTexture)
            {
                _assetRefCount[fullpath] += 1;
                return outTexture;
            }
        }
        
        Texture2D texture;
        
        // get from parent if we have one
        if (_parent != null)
        {
            texture = _parent.LoadTexture(filepath); // not fullpath
        }
        else
        {
            // no parent, and we haven't cached it yet, load texture ourselves
            using var stream = TitleContainer.OpenStream(fullpath);
            texture = Texture2D.FromStream(GraphicsDevice, stream);
        }
        
        // cache and return texture
        LoadedAssets[fullpath] = texture;
        _assetRefCount[fullpath] = 1;
        return texture;
    }
    
    public SpriteFontBase LoadBitmapFont(string filepath)
    {
        var fullpath = MakeAssetPath(filepath);
        
        // check our cache first
        if (LoadedAssets.TryGetValue(fullpath, out var outFont))
        {
            if (outFont is SpriteFontBase spriteFontBase)
            {
                _assetRefCount[fullpath] += 1;
                return spriteFontBase;
            }
        }

        SpriteFontBase font;
        
        // get from parent if we have one
        if (_parent != null)
        {
            font = _parent.LoadBitmapFont(filepath); // not fullpath
        }
        else
        {
            // no parent, load it ourselves
            using var stream = TitleContainer.OpenStream(fullpath);
            using var reader = new StreamReader(stream);
            var fontData = reader.ReadToEnd();

            font = StaticSpriteFont.FromBMFont(fontData,
                fileName => TitleContainer.OpenStream(
                    Path.Combine( Path.GetDirectoryName(fullpath) ?? "", fileName)), 
                GraphicsDevice);
        }
        
        // cache and return font
        LoadedAssets[fullpath] = font;
        _assetRefCount[fullpath] = 1;
        return font;
    }

    /// <summary>
    /// Unload a single content item
    /// </summary>
    /// <param name="filepath"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool UnloadAsset(string filepath)
    {
        var fullpath = MakeAssetPath(filepath);

        var dealtWith = false;
        
        // Let parent deal with it
        if (_parent != null)
        {
            dealtWith = _parent.UnloadAsset(filepath);
        }
        
        // Check if we own a reference
        if (LoadedAssets.TryGetValue(fullpath, out var asset))
        {
            // we do, remove one
            _assetRefCount[fullpath] -= 1;

            // check if we've hit zero references
            if (_assetRefCount[fullpath] <= 0)
            {
                // no parent has dealt with this unload, and we have a zero-reference count, we'll dispose of it
                if (!dealtWith)
                {
                    if (asset is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }

                LoadedAssets.Remove(fullpath);
                _assetRefCount.Remove(fullpath);
            }
            
            return true;
        }
        
        return dealtWith;
    }
    
    /// <summary>
    /// Unload all loaded asset files currently stored in this manager
    /// </summary>
    public override void Unload()
    {
        foreach (var key in LoadedAssets.Keys)
        {
            UnloadAsset(key);
        }
        
        // (we may still have references left over from children, don't fully clean up by clearing the LoadedAssets)
        
        base.Unload();
    }
}
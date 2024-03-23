using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

    private readonly SdgContentManager _parent;

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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="engine">Engine to receive services from</param>
    /// <param name="parent">Parent content manager to cache with, or null if none</param>
    /// <param name="rootDirectory">
    /// Root content directory. All load functions will prepend this directory to each filename.
    /// </param>
    public SdgContentManager(Core engine, SdgContentManager parent = null, string rootDirectory = "Content") 
        : base(engine.Services, rootDirectory)
    {
        _assetRefCount = new Dictionary<string, int>();
        _parent = parent;
    }

    private string MakeAssetPath(string filepath)
    {
        return Path.Combine(RootDirectory, filepath);
    }

    /// <summary>
    /// Get the number of times an SdgContentManager has referenced a particular asset between itself and its children.
    /// This is mainly for debugging.
    /// </summary>
    /// <param name="filepath">path to the asset</param>
    /// <returns>Number of times asset at the path was referenced by an SdgContentManager</returns>
    public int RefCountOf(string filepath)
    {
        var fullpath = MakeAssetPath(filepath);
        return _assetRefCount.GetValueOrDefault(fullpath, 0);
    }

    private T LoadAsset<T>(string filepath, Func<SdgContentManager, string, T> loadAssetFromPath) where T : class
    {
        var fullpath = MakeAssetPath(filepath);
        
        // check cache first
        if (LoadedAssets.TryGetValue(fullpath, out var outAsset)
            && outAsset is T outT)
        {
            _assetRefCount[fullpath] += 1;
            return outT;
        }
        
        // get asset from parent if we have one, otherwise load it ourselves
        var asset = (_parent == null) ? 
            loadAssetFromPath(this, fullpath) : 
            _parent.LoadAsset(filepath, loadAssetFromPath);
        
        // cache and return
        LoadedAssets[fullpath] = asset;
        _assetRefCount[fullpath] = 1;

        return asset;
    }

    /// <summary>
    /// Load a texture from an image file
    /// </summary>
    /// <param name="filepath">Path to the image file</param>
    /// <returns>Texture2D object with image data</returns>
    public Texture2D LoadTexture(string filepath)
    {
        return LoadAsset(filepath, LoadTextureImpl);
    }

    private static Texture2D LoadTextureImpl(SdgContentManager content, string fullpath)
    {
        using var stream = TitleContainer.OpenStream(fullpath);
        return Texture2D.FromStream(content.GraphicsDevice, stream);
    }
    
    public SpriteFontBase LoadBitmapFont(string filepath)
    {
        return LoadAsset(filepath, LoadBitmapFontImpl);
    }

    private static SpriteFontBase LoadBitmapFontImpl(SdgContentManager content, string fullpath)
    {
        // Open stream, convert to text string
        using var stream = TitleContainer.OpenStream(fullpath);
        using var reader = new StreamReader(stream);
        
        var fontData = reader.ReadToEnd();
        
        // Load the font
        return StaticSpriteFont.FromBMFont(fontData,
            fileName => TitleContainer.OpenStream(
                Path.Combine( Path.GetDirectoryName(fullpath) ?? "", fileName)), 
            content.GraphicsDevice);
    }

    public bool UnloadAsset(string filepath)
    {
        return UnloadAssetFullpath(MakeAssetPath(filepath));
    }

    /// <summary>
    /// Unload a single content item
    /// </summary>
    /// <param name="fullpath"></param>
    /// <returns>
    /// Whether or not asset was dealt with. False means this manager doesn't own content at that path
    /// </returns>
    private bool UnloadAssetFullpath(string fullpath)
    {
        // Check if we own a reference
        if (LoadedAssets.TryGetValue(fullpath, out var asset))
        {
            // we do, remove one
            _assetRefCount[fullpath] -= 1;
            
            var dealtWith = false;
        
            // Let parent deal with it
            if (_parent != null)
            {
                dealtWith = _parent.UnloadAssetFullpath(fullpath);
            }

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

        return false;
    }
    
    /// <summary>
    /// Unload all loaded asset files currently stored in this manager
    /// </summary>
    public override void Unload()
    {
        foreach (var key in LoadedAssets.Keys)
        {
            UnloadAssetFullpath(key);
        }
    }
}
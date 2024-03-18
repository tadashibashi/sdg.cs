using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SDG.TestScenes;

public class Camera2DTest : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFontBase _font;
    private Vector2 _textSize;
    private Camera2D _camera;

    public Camera2DTest()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// Load font relative to Content/SDG/Fonts/
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    private SpriteFontBase LoadBmpFont(string filepath)
    {
        var fontData = File.ReadAllText("Content/SDG/Fonts/" + filepath);
        return  StaticSpriteFont.FromBMFont(fontData,
            fileName => File.OpenRead("Content/SDG/Fonts/" + fileName),
            _graphics.GraphicsDevice);
    }
    
    private Texture2D LoadTexture(string filepath)
    {
        using var stream = TitleContainer.OpenStream(filepath);
        return Texture2D.FromStream(GraphicsDevice, stream);
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _camera = new Camera2D
        {
            Origin = Vector2.Zero,
            ScreenBounds = GraphicsDevice.Viewport.Bounds,
            Position = Vector2.Zero
        };

        _font = LoadBmpFont("DefaultFont.fnt");
    }



    private Vector2 _camPosition;
        
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float x = 0, y = 0;
        const float speed = 2.0f;
        if (Keyboard.GetState().IsKeyDown(Keys.Left))
        {
            x -= speed;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Right))
        {
            x += speed;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Up))
        {
            y -= speed;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Down))
        {
            y += speed;
        }
        _camPosition += new Vector2(x, y);
        _camera.LookAt(_camPosition);

        var zoom = 1.0f;
        if (Keyboard.GetState().IsKeyDown(Keys.X))
        {
            zoom *= 1.1f;
        }

        if (Keyboard.GetState().IsKeyDown(Keys.Z))
        {
            zoom *= 0.9f;
        }
        _camera.Zoom *= new Vector2(zoom, zoom);

        float rotate = 0;
        if (Keyboard.GetState().IsKeyDown(Keys.R))
        {
            rotate += MathHelper.ToRadians(2.0f);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.E))
        {
            rotate -= MathHelper.ToRadians(2.0f);
        }
        _camera.Rotation += rotate;
            
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // Set the transform matrix here
        _spriteBatch.Begin(SpriteSortMode.BackToFront, 
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.Default, 
            RasterizerState.CullNone, 
            null, 
            _camera.Matrix);
        _spriteBatch.DrawString(_font, "Hello Camera2D", 
            new Vector2(
                _graphics.PreferredBackBufferWidth/2.0f - _textSize.X/2.0f, 
                _graphics.PreferredBackBufferHeight/2.0f - _textSize.Y/2.0f),
            Color.Gainsboro);
        _spriteBatch.End();
        
        _spriteBatch.Begin(SpriteSortMode.BackToFront, 
            BlendState.AlphaBlend, 
            SamplerState.PointClamp, 
            DepthStencilState.Default,
            RasterizerState.CullNone);
        var mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        _spriteBatch.DrawString(_font, "Mouse Position: " + mousePosition.ToString(),
            new Vector2(10, 10), Color.Black);
        _spriteBatch.DrawString(_font, "World Position: " + _camera.ViewToWorld(mousePosition).ToString(),
            new Vector2(10, 32), Color.Black);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
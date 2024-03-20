using System;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDG.Systems;

namespace SDG.TestScenes;

public class Camera2DTest : Core
{
    private SpriteBatch _spriteBatch;
    private SpriteFontBase _font;
    private Vector2 _textSize;
    private Camera2D _camera;

    private EntityContext<Entity> _entities;
    private TextRenderer _textRenderer; 

    public Camera2DTest()
    {
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
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

        _entities = new EntityContext<Entity>(256, (ctx) => new Entity(ctx));
        _font = Content.LoadBitmapFont("SDG/Fonts/DefaultFont.fnt");

        {
            var e = _entities.CreateEntity();
            e.AddComponent(new TextComponent
            {
                CharSpacing = 0,
                Colors = new[] {Color.Green, Color.Black, Color.Orange, Color.Blue, Color.YellowGreen},
                Font = _font,
                LineSpacing = 0,
                Origin = new Vector2(0.5f, 0.5f),
                Text = "Hello!!!"
            });
            e.AddComponent(new Transform2D
            {
                Depth = 0,
                Position = new Vector2(100, 100),
                Rotation = 90,
                Scale = new Vector2(2.0f, 1.0f)
            });
        }

        _textRenderer = new TextRenderer(_entities);
        _entities.ApplyChanges();
    }



    private Vector2 _camPosition;
    private Vector2 _quakeOffset;
    
    private Random _rand = new();
        
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        _quakeOffset = new Vector2(_rand.NextSingle() * 2.0f, _rand.NextSingle() * 2.0f);

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
        _camera.LookAt(_camPosition + _quakeOffset);

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
            RasterizerState.CullCounterClockwise, 
            null, 
            _camera.Matrix);
        _spriteBatch.DrawString(_font, "Hello Camera2D", 
            new Vector2(
                Window.ClientBounds.Width/2.0f - _textSize.X/2.0f, 
                Window.ClientBounds.Height/2.0f - _textSize.Y/2.0f),
            Color.Gainsboro);
        _textRenderer.Draw(_spriteBatch);
        _spriteBatch.End();
        
        _spriteBatch.Begin(SpriteSortMode.BackToFront, 
            BlendState.AlphaBlend, 
            SamplerState.PointClamp, 
            DepthStencilState.Default,
            RasterizerState.CullCounterClockwise);
        var mousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        _spriteBatch.DrawString(_font, "Mouse Position: " + mousePosition.ToString(),
            new Vector2(10, 10), Color.Black);
        _spriteBatch.DrawString(_font, "World Position: " + _camera.ViewToWorld(mousePosition).ToString(),
            new Vector2(10, 32), Color.Black);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
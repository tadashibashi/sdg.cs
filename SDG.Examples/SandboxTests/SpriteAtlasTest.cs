using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SDG.Systems;
using SDG.Systems.Sprites;

namespace SDG.Examples;

public class SpriteAtlasTest : Core
{
    private readonly SpriteAtlas _atlas = new();
    private readonly SpriteRenderer _spriteRenderer = new();
    private readonly EntityContext _entities = new();
    private SpriteBatch _spriteBatch;
    
    protected override void Initialize()
    {
        _spriteRenderer.Initialize(_entities);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _atlas.FromTexturePackerJson(GraphicsDevice, "Content/SDG/Sprites/sprite_sheet.json");
        var sprite = new Sprite(new[]
        {
            _atlas["minecart/horizontal-1/1.png"], _atlas["minecart/horizontal-1/2.png"], 
            _atlas["minecart/horizontal-1/3.png"], _atlas["minecart/horizontal-1/4.png"]
        }, 5.0f);
        var rand = new Random();
        {
            for (var i = 0; i < 1000; ++i)
            {
                _entities.CreateEntity()
                    .AddComponent(new Transform2D
                    {
                        Position = new Vector2(rand.NextSingle() * (float)Window.ClientBounds.Width,
                            rand.NextSingle() * (float)Window.ClientBounds.Height
                        ),
                        Scale = Vector2.One
                    })
                    .AddComponent(new SpriteComponent
                    {
                        Sprite = sprite,
                        Color = new Color(rand.NextSingle(), rand.NextSingle(), rand.NextSingle())
                    });
        
            }
        
        }
        
        Console.WriteLine(_atlas["minecart/horizontal-1/1.png"].Pivot);

        _entities.CreateEntity()
            .AddComponent(new Transform2D())
            .AddComponent(new SpriteComponent
            {
                Sprite = sprite
            });
    }

    protected override void Update(GameTime time)
    {
        _spriteRenderer.Update(time);
        var keyboard = Keyboard.GetState();
        foreach (var (e, tf) in _entities.Find<Transform2D>())
        {
            if (keyboard.IsKeyDown(Keys.Left))
                tf.Position += new Vector2(-2, 0);
            if (keyboard.IsKeyDown(Keys.Right))
                tf.Position += new Vector2(2, 0);
            if (keyboard.IsKeyDown(Keys.Down))
                tf.Position += new Vector2(0, 2);
            if (keyboard.IsKeyDown(Keys.Up))
                tf.Position += new Vector2(0, -2);
            if (keyboard.IsKeyDown(Keys.X))
                tf.Scale += new Vector2(0.1f, 0);
            if (keyboard.IsKeyDown(Keys.Z))
                tf.Scale -= new Vector2(0.1f, 0);
        }
        
        _entities.ApplyChanges();
    }

    protected override void Draw(GameTime time)
    {
        GraphicsDevice.Clear(Color.Coral);
        _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp,
            DepthStencilState.Default, RasterizerState.CullCounterClockwise, null);
        _spriteRenderer.Draw(time, _spriteBatch);
        _spriteBatch.End();
    }
}
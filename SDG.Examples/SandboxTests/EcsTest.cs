using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SDG.Examples
{
    /// <summary>
    /// FIXME: Create a scene class to subclass and run this in the main Core class
    /// </summary>
    public class EcsTest : Core
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public EcsTest()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        
        private record Position
        {
            public int X { get; set; } = 0;
            public int Y { get; set; } = 0;
        }

        private record Transform
        {
            public Vector2 Scale { get; set; } = new();
            public float Degrees { get; set; } = 0;
        }

        private record Sprite
        {
            public Rectangle SubOrigin { get; set; } = new();
            public string AtlasName { get; set; } = "";
        }

        private record Tag
        {
            public string Name { get; set; } = "";
            public string Group { get; set; } = "";
        }
        
        private Stopwatch sw = new();
        private EntityContext<Entity> ctx;

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            allEntities = new List<Entity>();
            ctx = new EntityContext<Entity>(256, ctx => new Entity(ctx));
            for (var i = 0; i < 10000; ++i)
            {
                {
                    allEntities.Add(ctx.CreateEntity()
                        .AddComponent(new Position { X = 50, Y = 100 })
                        .AddComponent(new Tag { Name = "Camera", Group = "Controller" }));
                }
                {
                    allEntities.Add(ctx.CreateEntity()
                        .AddComponent(new Position { X = 10, Y = 20 })
                        .AddComponent(new Tag { Name = "Joe", Group = "NPC" })
                        .AddComponent(new Sprite
                            { AtlasName = "Main", SubOrigin = new Rectangle { X = 10, Y = 20, Width = 16, Height = 16 } }));
                }
                {
                    allEntities.Add(ctx.CreateEntity()
                        .AddComponent(new Position { X = 20, Y = 40 })
                        .AddComponent(new Tag { Name = "Bob", Group = "NPC" })
                        .AddComponent(new Sprite
                            { AtlasName = "Main", SubOrigin = new Rectangle { X = 40, Y = 40, Width = 16, Height = 16 } })
                        .AddComponent(new Transform { Degrees = 20, Scale = new Vector2(2.4f, 1.2f) }));
                }
                {
                    allEntities.Add(ctx.CreateEntity()
                        .AddComponent(new Position { X = 50, Y = 50 })
                        .AddComponent(new Tag { Name = "Bush1", Group = "Prop" })
                        .AddComponent(new Sprite { AtlasName = "Main", SubOrigin = new Rectangle(100, 200, 32, 32) }));
                }
            }
            ctx.ApplyChanges();
            
            entities = ctx.Find<Tag, Position, Sprite>().ToList();
            group = new EntityGroup<Entity, Tag, Position, Sprite>(ctx);
        }

        private EntityGroup<Entity, Tag, Position, Sprite> group;
        private List<(Entity, Tag, Position, Sprite)> entities;
        private List<Entity> allEntities;
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            sw.Reset();
            sw.Start();
            var count = 0;
            foreach (var (e, tag, position, sprite) in ctx.Find<Tag, Position, Sprite>())
            {
                ++count;
                position.X += count;
            }
            sw.Stop();
            Console.WriteLine("Ms: " + sw.Elapsed.ToString());
            Console.WriteLine(count);
            sw.Reset();
            count = 0;
            sw.Start();
            foreach (var (e, tag, position, sprite) in entities)
            {
                ++count;
                position.X -= count;
            }
            sw.Stop();
            Console.WriteLine("Static list Ms: " + sw.Elapsed.ToString());
            Console.WriteLine(count);
            sw.Reset();
            count = 0;
            sw.Start();
            foreach (var (e, tag, position, sprite) in group)
            {
                ++count;
                position.X += count;
            }
            sw.Stop();
            Console.WriteLine("Group Ms: " + sw.Elapsed.ToString());
            Console.WriteLine(count);
            sw.Reset();

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                for (var i = 0; i < 100; ++i)
                {
                    if (allEntities.Count > 0)
                    {
                        allEntities[^1].Destroy();
                        allEntities.RemoveAt(allEntities.Count-1);
                    }

                }
            }

            ctx.ApplyChanges();
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}

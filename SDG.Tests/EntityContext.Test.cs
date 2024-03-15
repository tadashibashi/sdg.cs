using System.Drawing;
using System.Numerics;
using Xunit;

namespace SDG.Tests;

public class EntityContext
{
    [Fact]
    public void CanCreateEntities()
    {
        EntityContext<Entity> context = new(256, (context) => new Entity(context));

        var entity = context.CreateEntity();
        context.FlushCommands();
        
        Assert.Equal(1, context.AliveEntityCount);
        Assert.True(entity.IsAlive);
    }
    
    // Helper to create a default entity context
    private static EntityContext<Entity> MakeContext()
    {
        return new EntityContext<Entity>(256, (context) => new Entity(context));
    }
    
    private class Position
    {
        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
    }

    private class Transform
    {
        public Vector2 Scale { get; set; } = new();
        public float Degrees { get; set; } = 0;
    }

    private class Sprite
    {
        public Rectangle SubOrigin { get; set; } = new();
        public string AtlasName { get; set; } = "";
    }

    private class Tag
    {
        public string Name { get; set; } = "";
        public string Group { get; set; } = "";
    }

    [Fact]
    public void CanAddComponentTypes()
    {
        var ctx = MakeContext();
        var e = ctx.CreateEntity();

        e.AddComponent(new Position
        {
            X = 10,
            Y = 10,
        });
        ctx.FlushCommands();
        Assert.Equal(1, ctx.ComponentTypeCount);
        
        e.AddComponent(new Tag
        {
            Name = "Player",
            Group = "Actor"
        });
        ctx.FlushCommands();
        Assert.Equal(2, ctx.ComponentTypeCount);
    }
    
    [Fact]
    public void CanGetComponents()
    {
        var ctx = MakeContext();
        var e = ctx.CreateEntity();

        e.AddComponent(new Position
        {
            X = 10,
            Y = 20,
        });
        
        e.AddComponent(new Tag
        {
            Name = "Player",
            Group = "Actor"
        });

        var position = e.GetComponent<Position>();
        Assert.NotNull(position);
        Assert.Equal(10, position.X);
        Assert.Equal(20, position.Y);

        var tag = e.GetComponent<Tag>();
        Assert.NotNull(tag);
        Assert.Equal("Player", tag.Name);
        Assert.Equal("Actor", tag.Group);
    }

    [Fact]
    public void CanFindAllComponents()
    {
        var ctx = MakeContext();
        {
            ctx.CreateEntity()
                .AddComponent(new Position { X = 50, Y = 100 })
                .AddComponent(new Tag { Name = "Camera", Group = "Controller" });
        }
        {
            ctx.CreateEntity()
                .AddComponent(new Position { X=10, Y=20})
                .AddComponent(new Tag { Name="Joe", Group="NPC" });
        }
        {
            ctx.CreateEntity()
                .AddComponent(new Position { X=20, Y=40})
                .AddComponent(new Tag { Name="Bob", Group="NPC" });
        }

        ctx.FlushCommands();
        
        Assert.Equal(3, ctx.AliveEntityCount);

        var count = 0;
        foreach (var (e, tag, position) in ctx.Find<Tag, Position>())
        {
            switch (tag.Name)
            {
                case "Bob":
                    Assert.Equal("NPC", tag.Group);
                    break;
                case "Camera":
                    Assert.Equal(50, position.X);
                    break;
                case "Joe":
                    Assert.Equal(20, position.Y);
                    Assert.Equal("NPC", tag.Group);
                    break;
            }

            Assert.True(e.IsAlive);
            ++count;
        }
        
        Assert.Equal(3, count);
    }
    
    [Fact]
    public void CanFindPatternsOfComponents()
    {
        var ctx = MakeContext();
        {
            ctx.CreateEntity()
                .AddComponent(new Position { X = 50, Y = 100 })
                .AddComponent(new Tag { Name = "Camera", Group = "Controller" });
        }
        {
            ctx.CreateEntity()
                .AddComponent(new Position { X = 10, Y = 20 })
                .AddComponent(new Tag { Name = "Joe", Group = "NPC" })
                .AddComponent(new Sprite
                    { AtlasName = "Main", SubOrigin = new Rectangle { X = 10, Y = 20, Width = 16, Height = 16 } });
        }
        {
            ctx.CreateEntity()
                .AddComponent(new Position { X = 20, Y = 40 })
                .AddComponent(new Tag { Name = "Bob", Group = "NPC" })
                .AddComponent(new Sprite
                    { AtlasName = "Main", SubOrigin = new Rectangle { X = 40, Y = 40, Width = 16, Height = 16 } })
                .AddComponent(new Transform { Degrees = 20, Scale = new Vector2(2.4f, 1.2f) });
        }
        {
            ctx.CreateEntity()
                .AddComponent(new Position { X = 50, Y = 50 })
                .AddComponent(new Tag { Name = "Bush1", Group = "Prop" })
                .AddComponent(new Sprite { AtlasName = "Main", SubOrigin = new Rectangle(100, 200, 32, 32) });
        }

        ctx.FlushCommands();
        Assert.Equal(4, ctx.AliveEntityCount);

        var count = 0;
        foreach (var (e, tag, position, sprite) in ctx.Find<Tag, Position, Sprite>())
        {
            switch (tag.Name)
            {
                case "Bob":
                    Assert.Equal("NPC", tag.Group);
                    Assert.Equal("Main", sprite.AtlasName);
                    Assert.Equal(new Rectangle(40, 40, 16, 16), sprite.SubOrigin);
                    break;
                case "Camera":
                    Assert.Equal(50, position.X);
                    break;
                case "Bush1":
                    Assert.Equal(50, position.Y);
                    Assert.Equal("Prop", tag.Group);
                    break;
            }

            Assert.True(e.IsAlive);
            ++count;
        }
        
        Assert.Equal(3, count);

        count = 0;
        foreach (var (_, tag, tf) in ctx.Find<Tag, Transform>())
        {
            if (tag.Name == "Bob")
            {
                Assert.Equal(20.0f, tf.Degrees);
                Assert.Equal(new Vector2(2.4f, 1.2f), tf.Scale);
            }
            ++count;
        }
        
        Assert.Equal(1, count);
    }
}
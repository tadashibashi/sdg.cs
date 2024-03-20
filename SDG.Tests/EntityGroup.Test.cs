using Xunit;

namespace SDG.Tests;

public class EntityGroupTest
{
    private record Component1
    {
        public string Data { get; set; } = "";
    }

    private record Component2
    {
        public string Data { get; set; } = "";
    }

    private record Component3
    {
        public string Data { get; set; } = "";
    }

    private record Component4
    {
        public string Data { get; set; } = "";
    }

    private record Component5
    {
        public string Data { get; set; } = "";
    }
    
    private record Component6
    {
        public string Data { get; set; } = "";
    }
    
    private record Component7
    {
        public string Data { get; set; } = "";
    }
    
    private record Component8
    {
        public string Data { get; set; } = "";
    }
    
    private record Component9
    {
        public string Data { get; set; } = "";
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(1)]
    [InlineData(10000)]
    [InlineData(1000000)]
    public void GroupWith1TypeContainsCorrectEntities(int count)
    {
        EntityContext<Entity> context = new(256, ctx => new Entity(ctx));
        EntityGroup<Entity, Component1> group = new(context);
        Assert.Equal(0, group.Count);

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "12345"});
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component2 { Data = "67890"});
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component3 { Data = "ABCDEF"});
        }

        context.ApplyChanges();
        
        Assert.Equal(count, group.Count);

        foreach (var (e, c1) in group)
        {
            Assert.Equal("12345", c1.Data);
        }
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(1)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void GroupWith2TypesContainsCorrectEntities(int count)
    {
        EntityContext<Entity> context = new(count * 3, ctx => new Entity(ctx));
        EntityGroup<Entity, Component1, Component2> group = new(context);
        Assert.Equal(0, group.Count);

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "12345" })
                .AddComponent(new Component2 { Data = "54321" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component2 { Data = "67890" }) // contains one correct type, missing one
                .AddComponent(new Component3 { Data = "ABCDEF" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "67890" }) // contains one correct type, missing one
                .AddComponent(new Component3 { Data = "ABCDEF" });
        }

        context.ApplyChanges();
        
        Assert.Equal(count, group.Count);

        foreach (var (e, c1, c2) in group)
        {
            Assert.Equal("12345", c1.Data);
            Assert.Equal("54321", c2.Data);
        }
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(1)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void GroupWith3TypesContainsCorrectEntities(int count)
    {
        EntityContext<Entity> context = new(count * 3, ctx => new Entity(ctx));
        EntityGroup<Entity, Component1, Component2, Component3> group = new(context);
        Assert.Equal(0, group.Count);

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity() // matching pattern, but missing one
                .AddComponent(new Component1 { Data = "12345" }) 
                .AddComponent(new Component2 { Data = "54321" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "1234"}) // matching pattern with other components still works
                .AddComponent(new Component2 { Data = "67890" })
                .AddComponent(new Component3 { Data = "ABCDEF" })
                .AddComponent(new Component4 { Data = "ABCDEF" })
                .AddComponent(new Component5 { Data = "ABCDEF" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "67890" }) // contains two correct types, but one incorrect
                .AddComponent(new Component3 { Data = "ABCDEF" })
                .AddComponent(new Component4 { Data = "Pizzaaa!" });
        }

        context.ApplyChanges();
        
        Assert.Equal(count, group.Count);

        foreach (var (e, c1, c2, c3) in group)
        {
            Assert.Equal("1234", c1.Data);
            Assert.Equal("67890", c2.Data);
            Assert.Equal("ABCDEF", c3.Data);
        }
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(1)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void GroupWith4TypesContainsCorrectEntities(int count)
    {
        EntityContext<Entity> context = new(count * 3, ctx => new Entity(ctx));
        EntityGroup<Entity, Component1, Component2, Component3, Component4> group = new(context);
        Assert.Equal(0, group.Count);

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity() // matching pattern, but missing one
                .AddComponent(new Component1 { Data = "12345" }) 
                .AddComponent(new Component2 { Data = "54321" })
                .AddComponent(new Component3 { Data = "54321" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "1234" }) // matching pattern with other components still works
                .AddComponent(new Component2 { Data = "67890" })
                .AddComponent(new Component3 { Data = "ABCDEF" })
                .AddComponent(new Component4 { Data = "GHIJKL" })
                .AddComponent(new Component5 { Data = "ABCDEF" })
                .AddComponent(new Component6 { Data = "ABCDEF" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "67890" }) // contains all but one correct type
                .AddComponent(new Component2 { Data = "ABCDEF" })
                .AddComponent(new Component3 { Data = "Pizzaaa!" })
                .AddComponent(new Component5 { Data = "Pizzaaa!" });
        }

        context.ApplyChanges();
        
        Assert.Equal(count, group.Count);

        foreach (var (e, c1, c2, c3, c4) in group)
        {
            Assert.Equal("1234", c1.Data);
            Assert.Equal("67890", c2.Data);
            Assert.Equal("ABCDEF", c3.Data);
            Assert.Equal("GHIJKL", c4.Data);
        }
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(1)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void GroupWith5TypesContainsCorrectEntities(int count)
    {
        EntityContext<Entity> context = new(count * 3, ctx => new Entity(ctx));
        EntityGroup<Entity, Component1, Component2, Component3, Component4, Component5> group = new(context);
        Assert.Equal(0, group.Count);

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity() // matching pattern, but missing one
                .AddComponent(new Component1 { Data = "12345" }) 
                .AddComponent(new Component2 { Data = "54321" })
                .AddComponent(new Component3 { Data = "54321" })
                .AddComponent(new Component4 { Data = "54321" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "1234" }) // matching pattern with other components still works
                .AddComponent(new Component2 { Data = "67890" })
                .AddComponent(new Component3 { Data = "ABCDEF" })
                .AddComponent(new Component4 { Data = "GHIJKL" })
                .AddComponent(new Component5 { Data = "ABCDEF" })
                .AddComponent(new Component6 { Data = "Pizzaaa!!!" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "67890" }) // contains all but one correct type
                .AddComponent(new Component2 { Data = "ABCDEF" })
                .AddComponent(new Component3 { Data = "Pizzaaa!" })
                .AddComponent(new Component4 { Data = "Onion" })
                .AddComponent(new Component6 { Data = "Taco" });
        }

        context.ApplyChanges();
        
        Assert.Equal(count, group.Count);

        foreach (var (e, c1, c2, c3, c4, c5) in group)
        {
            Assert.Equal("1234", c1.Data);
            Assert.Equal("67890", c2.Data);
            Assert.Equal("ABCDEF", c3.Data);
            Assert.Equal("GHIJKL", c4.Data);
            Assert.Equal("ABCDEF", c5.Data);
        }
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(1)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void GroupWith6TypesContainsCorrectEntities(int count)
    {
        EntityContext<Entity> context = new(count * 3, ctx => new Entity(ctx));
        EntityGroup<Entity, Component1, Component2, Component3, Component4, Component5, Component6> group = new(context);
        Assert.Equal(0, group.Count);

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity() // matching pattern, but missing one
                .AddComponent(new Component1 { Data = "12345" })
                .AddComponent(new Component2 { Data = "54321" })
                .AddComponent(new Component3 { Data = "54321" })
                .AddComponent(new Component4 { Data = "54321" })
                .AddComponent(new Component5 { Data = "54321" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "1234" }) // matching pattern with other components still works
                .AddComponent(new Component2 { Data = "67890" })
                .AddComponent(new Component3 { Data = "ABCDEF" })
                .AddComponent(new Component4 { Data = "GHIJKL" })
                .AddComponent(new Component5 { Data = "ABCDEF" })
                .AddComponent(new Component6 { Data = "Pizzaaa!!!" })
                .AddComponent(new Component7 { Data = "MorePizza" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "67890" }) // contains all but one correct type
                .AddComponent(new Component2 { Data = "ABCDEF" })
                .AddComponent(new Component3 { Data = "Pizzaaa!" })
                .AddComponent(new Component4 { Data = "Onion" })
                .AddComponent(new Component5 { Data = "Onion" })
                .AddComponent(new Component7 { Data = "Taco" });
        }

        context.ApplyChanges();
        
        Assert.Equal(count, group.Count);

        foreach (var (e, c1, c2, c3, c4, c5, c6) in group)
        {
            Assert.Equal("1234", c1.Data);
            Assert.Equal("67890", c2.Data);
            Assert.Equal("ABCDEF", c3.Data);
            Assert.Equal("GHIJKL", c4.Data);
            Assert.Equal("ABCDEF", c5.Data);
            Assert.Equal("Pizzaaa!!!", c6.Data);
        }
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(1)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void GroupWith7TypesContainsCorrectEntities(int count)
    {
        EntityContext<Entity> context = new(count * 3, ctx => new Entity(ctx));
        EntityGroup<Entity, Component1, Component2, Component3, Component4, Component5, Component6, Component7> group = new(context);
        Assert.Equal(0, group.Count);

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity() // matching pattern, but missing one
                .AddComponent(new Component1 { Data = "12345" }) 
                .AddComponent(new Component2 { Data = "54321" })
                .AddComponent(new Component3 { Data = "54321" })
                .AddComponent(new Component4 { Data = "54321" })
                .AddComponent(new Component5 { Data = "54321" })
                .AddComponent(new Component6 { Data = "54321" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "1234" }) // matching pattern with other components still works
                .AddComponent(new Component2 { Data = "67890" })
                .AddComponent(new Component3 { Data = "ABCDEF" })
                .AddComponent(new Component4 { Data = "GHIJKL" })
                .AddComponent(new Component5 { Data = "ABCDEF" })
                .AddComponent(new Component6 { Data = "Pizzaaa!!!" })
                .AddComponent(new Component7 { Data = "MorePizza" })
                .AddComponent(new Component8 { Data = "MostPizza" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "67890" })  // contains all but one correct type
                .AddComponent(new Component2 { Data = "ABCDEF" })
                .AddComponent(new Component3 { Data = "Pizzaaa!" })
                .AddComponent(new Component4 { Data = "Onion" })
                .AddComponent(new Component5 { Data = "Onion" })
                .AddComponent(new Component6 { Data = "Onion" })
                .AddComponent(new Component8 { Data = "Taco" });
        }

        context.ApplyChanges();
        
        Assert.Equal(count, group.Count);

        foreach (var (e, c1, c2, c3, c4, c5, c6, c7) in group)
        {
            Assert.Equal("1234", c1.Data);
            Assert.Equal("67890", c2.Data);
            Assert.Equal("ABCDEF", c3.Data);
            Assert.Equal("GHIJKL", c4.Data);
            Assert.Equal("ABCDEF", c5.Data);
            Assert.Equal("Pizzaaa!!!", c6.Data);
            Assert.Equal("MorePizza", c7.Data);
        }
    }
    
    [Theory]
    [InlineData(1000)]
    [InlineData(1)]
    [InlineData(10000)]
    [InlineData(100000)]
    public void GroupWith8TypesContainsCorrectEntities(int count)
    {
        EntityContext<Entity> context = new(count * 3, ctx => new Entity(ctx));
        EntityGroup<Entity, Component1, Component2, Component3, Component4, Component5, Component6, Component7, Component8> group = new(context);
        Assert.Equal(0, group.Count);

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity() // matching pattern, but missing one
                .AddComponent(new Component1 { Data = "12345" }) 
                .AddComponent(new Component2 { Data = "54321" })
                .AddComponent(new Component3 { Data = "54321" })
                .AddComponent(new Component4 { Data = "54321" })
                .AddComponent(new Component5 { Data = "54321" })
                .AddComponent(new Component6 { Data = "54321" })
                .AddComponent(new Component7 { Data = "54321" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "1234" }) // matching pattern with other components still works
                .AddComponent(new Component2 { Data = "67890" })
                .AddComponent(new Component3 { Data = "ABCDEF" })
                .AddComponent(new Component4 { Data = "GHIJKL" })
                .AddComponent(new Component5 { Data = "ABCDEF" })
                .AddComponent(new Component6 { Data = "Pizzaaa!!!" })
                .AddComponent(new Component7 { Data = "MorePizza" })
                .AddComponent(new Component8 { Data = "MostPizza" })
                .AddComponent(new Component9 { Data = "EssenceOfPizza" });
        }

        for (var i = 0; i < count; ++i)
        {
            context.CreateEntity()
                .AddComponent(new Component1 { Data = "67890" })  // contains all but one correct type
                .AddComponent(new Component2 { Data = "ABCDEF" })
                .AddComponent(new Component3 { Data = "Pizzaaa!" })
                .AddComponent(new Component4 { Data = "Onion" })
                .AddComponent(new Component5 { Data = "Onion" })
                .AddComponent(new Component6 { Data = "Onion" })
                .AddComponent(new Component7 { Data = "Onion" })
                .AddComponent(new Component9 { Data = "Taco" });
        }

        context.ApplyChanges();
        
        Assert.Equal(count, group.Count);

        foreach (var (e, c1, c2, c3, c4, c5, c6, c7, c8) in group)
        {
            Assert.Equal("1234", c1.Data);
            Assert.Equal("67890", c2.Data);
            Assert.Equal("ABCDEF", c3.Data);
            Assert.Equal("GHIJKL", c4.Data);
            Assert.Equal("ABCDEF", c5.Data);
            Assert.Equal("Pizzaaa!!!", c6.Data);
            Assert.Equal("MorePizza", c7.Data);
        }
    }
}
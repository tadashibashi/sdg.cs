using Xunit;
namespace SDG.Tests;

public class Pool
{
    private class TestEntity : IPoolable
    {
        public Id Id { get; set; }

        public string Name { get; set; }

        public TestEntity(string name = "") 
        {
            Name = name;
        }
    }

    [Theory]
    [InlineData(256)]
    [InlineData(1024)]
    [InlineData(0)]
    [InlineData(50)]
    public void Constructor_SetsInitialSize(int initSize) 
    {
        Pool<TestEntity> pool = new(initSize, () => new TestEntity());

        Assert.Equal(initSize, pool.Count);
    }

    [Theory]
    [InlineData("Joe")]
    [InlineData("")]
    [InlineData("abcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()")]
    public void Constructor_SetMemberViaFactory(string name)
    {
        Pool<TestEntity> pool = new(256, () => new TestEntity(name));

        Assert.Equal(name, pool.CreateEntity().Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(256)]
    [InlineData(1024)]
    public void ExpandsAfterCreatingAllEntities(int initSize)
    {
        Pool<TestEntity> pool = new(initSize, () => new TestEntity());

        for (var i = 0; i < pool.Count; ++i)
        {
            pool.CreateEntity();
        }

        Assert.Equal(pool.Count, initSize);
        pool.CreateEntity();
        Assert.True(pool.Count > initSize);
    }

    [Fact]
    public void InvalidatesDiscardedEntityIds()
    {
        const int initSize = 256;
        Pool<TestEntity> pool = new(initSize, () => new TestEntity());

        var entity = pool.CreateEntity();
        Assert.True(pool.CheckValid(entity.Id));

        pool.Discard(entity.Id);
        Assert.False(pool.CheckValid(entity.Id));
    }

    [Fact]
    public void GeneratesUniqueIds()
    {
        const int initSize = 256;
        Pool<TestEntity> pool = new(initSize, () => new TestEntity());
        HashSet<ulong> set = new(initSize);

        for (var i = 0; i < initSize; ++i)
        {
            var entity = pool.CreateEntity();
            set.Add(entity.Id.InnerId);
        }

        Assert.Equal(initSize, set.Count);

        pool.DiscardAll();
        for (var i = 0; i < initSize; ++i)
        {
            var entity = pool.CreateEntity();
            set.Add(entity.Id.InnerId);
        }

        Assert.Equal(initSize * 2, set.Count);
    }
}
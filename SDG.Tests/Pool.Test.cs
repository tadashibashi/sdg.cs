using Xunit;

namespace SDG.Tests
{
    public class PoolTests
    {
        public class TestEntity : IPoolable
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
        public void Pool_ExpandsAfterCreatingAllEntities(int initSize)
        {
            Pool<TestEntity> pool = new(initSize, () => new TestEntity());

            for (int i = 0; i < pool.Count; ++i)
            {
                pool.CreateEntity();
            }

            Assert.Equal(pool.Count, initSize);
            pool.CreateEntity();
            Assert.True(pool.Count > initSize);
        }

        [Fact]
        public void Pool_InvalidatesDiscardedEntityIds()
        {
            const int initSize = 256;
            Pool<TestEntity> pool = new(initSize, () => new TestEntity());

            var entity = pool.CreateEntity();
            Assert.True(pool.CheckValid(entity.Id));

            pool.Discard(entity.Id);
            Assert.False(pool.CheckValid(entity.Id));
        }

        [Fact]
        public void Pool_GeneratesUniqueIds()
        {
            const int initSize = 256;
            Pool<TestEntity> pool = new(initSize, () => new TestEntity());
            HashSet<ulong> set = new(initSize);

            for (int i = 0; i < initSize; ++i)
            {
                var entity = pool.CreateEntity();
                set.Add(entity.Id.id);
            }

            Assert.Equal(initSize, set.Count);

            pool.DiscardAll();
            for (int i = 0; i < initSize; ++i)
            {
                var entity = pool.CreateEntity();
                set.Add(entity.Id.id);
            }

            Assert.Equal(initSize * 2, set.Count);
        }
    }

}


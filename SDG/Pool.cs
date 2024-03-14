using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDG
{
    /// <summary>
    /// Identifier, designed for Entity pools
    /// </summary>
    public struct Id
    {
        public int index;
        public int nextFreeIndex;
        public ulong id;

        public static readonly int NULL_INDEX = int.MaxValue;
        public static readonly ulong NULL_ID = ulong.MaxValue;

        public Id()
        {
            this.index = Id.NULL_INDEX;
            this.nextFreeIndex = Id.NULL_INDEX;
            id = Id.NULL_ID;
        }

        public static bool operator ==(Id left, Id right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Id left, Id right)
        {
            return !left.Equals(right);
        }

        public readonly override int GetHashCode()
        {
            return (int)id;
        }

        public readonly bool Equals(Id other)
        {
            return other.id == id;
        }

        public override readonly bool Equals(object other)
        {
            return other is Id id && Equals(id);
        }

        public readonly bool IsNull
        {
            get { return id == Id.NULL_ID; }
        }
    }

    public interface IPoolable
    {
        public Id Id { get; set; }
    }

    /// <summary>
    /// Class that stores a pool of IPoolable, an interface containing an Id field.
    /// It does not keep track of "alive" entities, for which the responsibility is handed to the user.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Pool<T> where T : IPoolable
    {
        readonly List<T> _pool;
        int _nextFreeIndex;
        ulong _idCounter;
        readonly Func<T> _entityFactory;
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initSize"></param>
        /// <param name="entityFactory">return a newly constructed entity, no need to set the Id value</param>
        public Pool(int initSize, Func<T> entityFactory)
        {
            _pool = new List<T>(initSize);

            _nextFreeIndex = initSize == 0 ? Id.NULL_INDEX : 0;
            _idCounter = 0;
            _entityFactory = entityFactory;

            Resize(initSize);
        }

        public int Count { get => _pool.Count; }

        private void Resize(int newSize)
        {
            var currentSize = _pool.Count;
            if (newSize <= currentSize) return;

            _pool.EnsureCapacity(newSize);

            for (var i = currentSize; i < newSize; ++i)
            {
                var t = _entityFactory();
                t.Id = new Id { index = i, nextFreeIndex = i + 1, id = Id.NULL_ID };

                _pool.Add(t);
            }

            // Set the last next free id to point to NULL_ID
            _pool[^1].Id = new Id
            {
                    index = newSize - 1,
                    id = _idCounter - 1,
                    nextFreeIndex = Id.NULL_INDEX
            };
        }

        public T GetEntityById(Id id)
        {
            if (!CheckValid(id))
                return default;
            return _pool[id.index];
        }

        /// <summary>
        /// Get a fresh entity from the pool
        /// </summary>
        /// <returns></returns>
        public T CreateEntity()
        {
            if (_nextFreeIndex == Id.NULL_INDEX) // no more room, expand pool
            {
                var currentSize = _pool.Count;
                Resize(currentSize * 2 + 1);

                _nextFreeIndex = currentSize;

            }

            // retrieve next free entity, update its inner id
            var freeIndex = _nextFreeIndex;
            var nextFreeIndex = _pool[freeIndex].Id.nextFreeIndex;

            _pool[freeIndex].Id = new Id()
            {
                index = _pool[freeIndex].Id.index,
                nextFreeIndex = Id.NULL_INDEX, // discard this value since we've cached it
                id = _idCounter++,
            };

            _nextFreeIndex = nextFreeIndex;

            return _pool[freeIndex];
        }

        public bool CheckValid(Id id)
        {
            return !id.IsNull && _pool[id.index].Id == id;
        }

        /// <summary>
        /// Discard an entity back into the pool for reuse
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Whether entity was discarded, if it was valid</returns>
        public bool Discard(Id id)
        {
            if (!CheckValid(id)) return false;

            _pool[id.index].Id = new Id()
            {
                id = Id.NULL_ID,
                nextFreeIndex = _nextFreeIndex,
                index = id.index,
            };

            _nextFreeIndex = id.index;
            return true;
        }

        /// <summary>
        /// Discard all entities back to the pool, invalidating all current ids
        /// </summary>
        public void DiscardAll()
        {
            if (_pool.Count == 0) return;

            for (int i = 0; i < _pool.Count; ++i)
            {
                _pool[i].Id = new Id()
                {
                    id = Id.NULL_ID,
                    nextFreeIndex = i + 1,
                    index = i
                };
            }

            // alter last index to point to null next index
            _pool[^1].Id = new Id()
            {
                id = Id.NULL_ID,
                nextFreeIndex = Id.NULL_INDEX,
                index = _pool.Count - 1,
            };
        }
    }

}

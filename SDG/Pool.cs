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
        public int Index;
        public int NextFreeIndex;
        public ulong InnerId;

        public const int NullIndex = int.MaxValue;
        public const ulong NullId = ulong.MaxValue;

        public Id()
        {
            Index = Id.NullIndex;
            NextFreeIndex = Id.NullIndex;
            InnerId = Id.NullId;
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
            return (int)InnerId;
        }

        private readonly bool Equals(Id other)
        {
            return other.InnerId == InnerId;
        }

        public readonly override bool Equals(object other)
        {
            return other is Id id && Equals(id);
        }

        public readonly bool IsNull => InnerId == Id.NullId;
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
    public class Pool<T> where T : class, IPoolable
    {
        private readonly List<T> _pool;
        private int _nextFreeIndex;
        private ulong _idCounter;
        private readonly Func<T> _entityFactory;
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initSize"></param>
        /// <param name="entityFactory">return a newly constructed entity, no need to set the Id value</param>
        public Pool(int initSize, Func<T> entityFactory)
        {
            _pool = new List<T>(initSize);

            _nextFreeIndex = initSize == 0 ? Id.NullIndex : 0;
            _idCounter = 0;
            _entityFactory = entityFactory;

            Resize(initSize);
        }

        public int Count => _pool.Count;

        private void Resize(int newSize)
        {
            var currentSize = _pool.Count;
            if (newSize <= currentSize) return;

            _pool.EnsureCapacity(newSize);

            for (var i = currentSize; i < newSize; ++i)
            {
                var t = _entityFactory();
                t.Id = new Id { Index = i, NextFreeIndex = i + 1, InnerId = Id.NullId };

                _pool.Add(t);
            }

            // Set the last next free id to point to NULL_ID
            _pool[^1].Id = new Id
            {
                    Index = newSize - 1,
                    InnerId = _idCounter - 1,
                    NextFreeIndex = Id.NullIndex
            };
        }

        public T GetEntityById(Id id)
        {
            return CheckValid(id) ? _pool[id.Index] : default;
        }

        /// <summary>
        /// Get a fresh entity from the pool
        /// </summary>
        /// <returns></returns>
        public T CreateEntity()
        {
            if (_nextFreeIndex == Id.NullIndex) // no more room, expand pool
            {
                var currentSize = _pool.Count;
                Resize(currentSize * 2 + 1);

                _nextFreeIndex = currentSize;

            }

            // retrieve next free entity, update its inner id
            var freeIndex = _nextFreeIndex;
            var nextFreeIndex = _pool[freeIndex].Id.NextFreeIndex;

            _pool[freeIndex].Id = new Id()
            {
                Index = _pool[freeIndex].Id.Index,
                NextFreeIndex = Id.NullIndex, // discard this value since we've cached it
                InnerId = _idCounter++,
            };

            _nextFreeIndex = nextFreeIndex;

            return _pool[freeIndex];
        }

        public bool CheckValid(Id id)
        {
            return !id.IsNull && _pool[id.Index].Id == id;
        }

        /// <summary>
        /// Discard an entity back into the pool for reuse
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Whether entity was discarded, if it was valid</returns>
        public bool Discard(Id id)
        {
            if (!CheckValid(id)) return false;

            _pool[id.Index].Id = new Id()
            {
                InnerId = Id.NullId,
                NextFreeIndex = _nextFreeIndex,
                Index = id.Index,
            };

            _nextFreeIndex = id.Index;
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
                    InnerId = Id.NullId,
                    NextFreeIndex = i + 1,
                    Index = i
                };
            }

            // alter last index to point to null next index
            _pool[^1].Id = new Id()
            {
                InnerId = Id.NullId,
                NextFreeIndex = Id.NullIndex,
                Index = _pool.Count - 1,
            };
        }
    }

}

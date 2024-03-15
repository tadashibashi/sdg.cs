using System;
using System.Collections.Generic;

namespace SDG;

public delegate bool EntityTupleGetter<TEntity, T>(Id id, out T tup)
    where TEntity : class, IPoolable;


public class BaseEntityGroup<TEntity, T> where TEntity : class, IPoolable
{
    protected readonly Dictionary<Id, T> _entities;
    
    private Action<Id, Type> _handleComponentAdded;
    private Action<Id, Type> _handleComponentRemoved;

    private readonly EntityContext<TEntity> _context;
    
    // Shouldn't be called directly, context will create this
    protected BaseEntityGroup(
        EntityContext<TEntity> context,
        EntityTupleGetter<TEntity, T> getTuple,
        IReadOnlySet<Type> types)
    {
        _entities = new Dictionary<Id, T>();
        _context = context;

        _handleComponentAdded = (id, type) =>
        {
            if (types.Contains(type) && getTuple(id, out var tup))
            {
                _entities.Add(id, tup);
            }
        };

        _handleComponentRemoved = (id, type) =>
        {
            if (types.Contains(type))
            {
                _entities.Remove(id);
            }
        };

        _context.ComponentAdded += _handleComponentAdded;
        _context.ComponentRemoved += _handleComponentRemoved;
    }

    public void Dispose()
    {
        if (_handleComponentAdded != null)
        {
            _context.ComponentAdded -= _handleComponentAdded;
            _context.ComponentRemoved -= _handleComponentRemoved;
            _handleComponentAdded = null;
            _handleComponentRemoved = null;
        }
    }

    public bool Contains(TEntity entity)
    {
        return Contains(entity.Id);
    }
    
    private bool Contains(Id id)
    {
        return _entities.ContainsKey(id);
    }

    public IEnumerable<T> AsEnumerable()
    {
        return _entities.Values;
    }

    public Dictionary<Id, T>.ValueCollection.Enumerator GetEnumerator()
    {
        return _entities.Values.GetEnumerator();
    }
}

public class EntityGroup<TEntity, T1> : 
    BaseEntityGroup<TEntity, (TEntity, T1)> 
    where TEntity : class, IPoolable
    where T1 : class 
{
    public EntityGroup(EntityContext<TEntity> context) : 
        base(context, context.FindById, new HashSet<Type> {typeof(T1)})
    {
        foreach (var tup in context.Find<T1>())
        {
            _entities.Add(tup.Item1.Id, tup);
        }
    }
}

public class EntityGroup<TEntity, T1, T2> : 
    BaseEntityGroup<TEntity, (TEntity, T1, T2)> 
    where TEntity : class, IPoolable
    where T1 : class
    where T2 : class
{
    public EntityGroup(EntityContext<TEntity> context) : 
        base(context, context.FindById, new HashSet<Type> {typeof(T1), typeof(T2)})
    {
        foreach (var tup in context.Find<T1, T2>())
        {
            _entities.Add(tup.Item1.Id, tup);
        }
    }
}

public class EntityGroup<TEntity, T1, T2, T3> : 
    BaseEntityGroup<TEntity, (TEntity, T1, T2, T3)> 
    where TEntity : class, IPoolable
    where T1 : class 
    where T2 : class 
    where T3 : class
{
    public EntityGroup(EntityContext<TEntity> context) :
        base(context, context.FindById,
            new HashSet<Type> { typeof(T1), typeof(T2), typeof(T3) })
    {
        foreach (var tup in context.Find<T1, T2, T3>())
        {
            _entities.Add(tup.Item1.Id, tup);
        }
    }
}

public class EntityGroup<TEntity, T1, T2, T3, T4> : 
    BaseEntityGroup<TEntity, (TEntity, T1, T2, T3, T4)> 
    where TEntity : class, IPoolable
    where T1 : class 
    where T2 : class 
    where T3 : class
    where T4 : class
{
    public EntityGroup(EntityContext<TEntity> context) :
        base(context, context.FindById,
            new HashSet<Type> { typeof(T1), typeof(T2), typeof(T3), typeof(T4) })
    {
        foreach (var tup in context.Find<T1, T2, T3, T4>())
        {
            _entities.Add(tup.Item1.Id, tup);
        }
    }
}

public class EntityGroup<TEntity, T1, T2, T3, T4, T5> : 
    BaseEntityGroup<TEntity, (TEntity, T1, T2, T3, T4, T5)> 
    where TEntity : class, IPoolable
    where T1 : class 
    where T2 : class 
    where T3 : class
    where T4 : class
    where T5 : class
{
    public EntityGroup(EntityContext<TEntity> context) :
        base(context, context.FindById,
            new HashSet<Type> { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) })
    {
        foreach (var tup in context.Find<T1, T2, T3, T4, T5>())
        {
            _entities.Add(tup.Item1.Id, tup);
        }
    }
}

public class EntityGroup<TEntity, T1, T2, T3, T4, T5, T6> : 
    BaseEntityGroup<TEntity, (TEntity, T1, T2, T3, T4, T5, T6)> 
    where TEntity : class, IPoolable
    where T1 : class 
    where T2 : class 
    where T3 : class
    where T4 : class
    where T5 : class
    where T6 : class
{
    public EntityGroup(EntityContext<TEntity> context) :
        base(context, context.FindById,
            new HashSet<Type> { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) })
    {
        foreach (var tup in context.Find<T1, T2, T3, T4, T5, T6>())
        {
            _entities.Add(tup.Item1.Id, tup);
        }
    }
}

public class EntityGroup<TEntity, T1, T2, T3, T4, T5, T6, T7> : 
    BaseEntityGroup<TEntity, (TEntity, T1, T2, T3, T4, T5, T6, T7)> 
    where TEntity : class, IPoolable
    where T1 : class 
    where T2 : class 
    where T3 : class
    where T4 : class
    where T5 : class
    where T6 : class
    where T7 : class
{
    public EntityGroup(EntityContext<TEntity> context) :
        base(context, context.FindById,
            new HashSet<Type>
            {
                typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)
            })
    {
        foreach (var tup in context.Find<T1, T2, T3, T4, T5, T6, T7>())
        {
            _entities.Add(tup.Item1.Id, tup);
        }
    }
}

public class EntityGroup<TEntity, T1, T2, T3, T4, T5, T6, T7, T8> : 
    BaseEntityGroup<TEntity, (TEntity, T1, T2, T3, T4, T5, T6, T7, T8)> 
    where TEntity : class, IPoolable
    where T1 : class 
    where T2 : class 
    where T3 : class
    where T4 : class
    where T5 : class
    where T6 : class
    where T7 : class
    where T8 : class
{
    public EntityGroup(EntityContext<TEntity> context) :
        base(context, context.FindById,
            new HashSet<Type>
            {
                typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)
            })
    {
        foreach (var tup in context.Find<T1, T2, T3, T4, T5, T6, T7, T8>())
        {
            _entities.Add(tup.Item1.Id, tup);
        }
    }
}

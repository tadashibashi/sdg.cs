using System;
using System.Collections.Generic;

namespace SDG;

public delegate bool EntityTupleGetter<TEntity, T>(Id id, out T tup)
    where TEntity : class, IPoolable;


public class BaseEntityGroup<TEntity, T> where TEntity : class, IPoolable
{
    protected readonly Dictionary<Id, T> Entities;
    
    private Action<Id, Type> _handleComponentAdded;
    private Action<Id, Type> _handleComponentRemoved;

    private readonly EntityContext<TEntity> _context;
    
    // Shouldn't be called directly, context will create this
    protected BaseEntityGroup(
        EntityContext<TEntity> context,
        EntityTupleGetter<TEntity, T> getTuple,
        IReadOnlySet<Type> types)
    {
        Entities = new Dictionary<Id, T>();
        _context = context;

        _handleComponentAdded = (id, type) =>
        {
            if (types.Contains(type) && getTuple(id, out var tup))
            {
                Entities.TryAdd(id, tup);
            }
        };

        _handleComponentRemoved = (id, type) =>
        {
            if (types.Contains(type))
            {
                Entities.Remove(id);
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

    public int Count => Entities.Count;

    public bool Contains(TEntity entity)
    {
        return Contains(entity.Id);
    }
    
    private bool Contains(Id id)
    {
        return Entities.ContainsKey(id);
    }

    public IEnumerable<T> AsEnumerable()
    {
        return Entities.Values;
    }

    public Dictionary<Id, T>.ValueCollection.Enumerator GetEnumerator()
    {
        return Entities.Values.GetEnumerator();
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
            Entities.Add(tup.Item1.Id, tup);
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
            Entities.Add(tup.Item1.Id, tup);
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
            Entities.Add(tup.Item1.Id, tup);
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
            Entities.Add(tup.Item1.Id, tup);
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
            Entities.Add(tup.Item1.Id, tup);
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
            Entities.Add(tup.Item1.Id, tup);
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
            Entities.Add(tup.Item1.Id, tup);
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
            Entities.Add(tup.Item1.Id, tup);
        }
    }
}

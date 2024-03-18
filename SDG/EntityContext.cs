using System;
using System.Collections.Generic;

namespace SDG;

/// <summary>
/// A function that receives an EntityContext, and creates an entity from it
/// </summary>
/// <typeparam name="TEntity">type of entity: must implement IPoolable and be a ref type</typeparam>
public delegate TEntity EntityFactory<TEntity>(EntityContext<TEntity> context) where TEntity : class, IPoolable;

public partial class EntityContext<TEntity> where TEntity : class, IPoolable
{
    // ----- Sub-types -----
    private enum CommandType { CreateEntity, DestroyEntity, NotifyAddedComponent, NotifyRemovedComponent }
    private readonly record struct Command(
        CommandType Type, Id Id, object Component, int TypeIndex);
        
    // ----- Members -----
    /// Component registry
    private readonly Dictionary<Type, int> _registry;
    private readonly List<Type> _indexes;
        
    /// All components of all entities in this context
    private readonly List<List<object>> _components;
        
    /// Entity pool
    private readonly Pool<TEntity> _entities;
    /// Alive entities
    private readonly HashSet<Id> _alive;
    /// List of current commands, call FlushCommands to execute them
    private readonly List<Command> _commandQueue;
        
    // ----- Callbacks -----
    /// Broadcasted when a component is added / deleted, e.g. entity destroyed and has its components removed.
    /// To informs all relevant groups to update their internal state the next time user accesses it.
    public event Action<Id, Type> ComponentAdded;

    public event Action<Id, Type> ComponentRemoved;
        
    /// Called right after an Entity is created
    public event Action<Id> EntityCreated;
        
    /// Called right before the entity has all its components discarded
    public event Action<Id> EntityDestroy;

    // ----- Getters -----
    public IReadOnlyCollection<Id> AliveEntities => _alive;
        
    /// Check if an entity is alive by Id
    public bool IsAlive(Id id)
    {
        return _entities.CheckValid(id) && _alive.Contains(id);
    }

    public EntityContext(int initPoolSize, EntityFactory<TEntity> entityFactory)
    {
        _entities = new Pool<TEntity>(initPoolSize, () => entityFactory(this));
        _registry = new Dictionary<Type, int>();
        _indexes = new List<Type>();
        _components = new List<List<object>>();
        _alive = new HashSet<Id>();

        _entities.PoolResized += HandlePoolResized;

        _commandQueue = new List<Command>();
    }
    
    /// <summary>
    /// Create an entity - it will be active starting next frame
    /// </summary>
    /// <returns></returns>
    public TEntity CreateEntity()
    {
        var e = _entities.CreateEntity();
        
        // defer command until post update
        _commandQueue.Add(new Command
        {
            Id = e.Id,
            Type = CommandType.CreateEntity
        });
            
        return e;
    }

    private void CreateEntityImpl(Id id)
    {
        if (_alive.Add(id))
        {
            EntityCreated?.Invoke(id);
        }
    }

    public bool DestroyEntity(Id id)
    {
        if (!_entities.CheckValid(id)) return false;
            
        // defer command until post update
        _commandQueue.Add(new Command
        {
            Id = id,
            Type = CommandType.DestroyEntity
        });

        return true;
    }

    private void DestroyEntityImpl(Id id)
    {
        if (_alive.Remove(id))
        {
            EntityDestroy?.Invoke(id);

            var i = 0;
            foreach (var list in _components)
            {
                list[id.Index] = null;
                ComponentRemoved?.Invoke(id, _indexes[i]);
                ++i;
            }
            _entities.Discard(id);
        }
    }
        
    
    /// <summary>
    /// Perform deferred command queue, e.g. create/destroy entity, notifications, etc.
    /// This should be called sometime post update
    /// </summary>
    public void FlushCommands()
    {
        if (_commandQueue.Count == 0) return;
            
        for (var i = 0; i < _commandQueue.Count; ++i)
        {
            var command = _commandQueue[i];
            switch (command.Type)
            {
                case CommandType.CreateEntity:
                    CreateEntityImpl(command.Id);
                    break;
                case CommandType.DestroyEntity:
                    DestroyEntityImpl(command.Id);
                    break;
                case CommandType.NotifyAddedComponent:
                    NotifyAddedComponentImpl(command.Id, command.Component, command.TypeIndex);
                    break;
                case CommandType.NotifyRemovedComponent:
                    NotifyRemovedComponentImpl(command.Id, command.TypeIndex);
                    break;
            }
        }

        _commandQueue.Clear();
    }

    /// <summary>
    /// Immediately clear all registered components, callbacks, entities, etc.
    /// Note: this invalidates all groups created with EntityContext.Group and
    /// should only be called when shutting down all ECS systems.
    /// </summary>
    public void Clear()
    {
        _registry.Clear();
        _indexes.Clear();
        ComponentAdded = null;
        _entities.Clear();
        _components.ForEach(c => c.Clear());
        _components.Clear();
        _alive.Clear();
    }
    
    /// <summary>
    /// Get a component of a sepcified type from an entity
    /// </summary>
    /// <param name="id">id of the entity to get the component from</param>
    /// <typeparam name="TComponent">type of component to get</typeparam>
    /// <returns>Component or `null`, if it doesn't exist on the entity</returns>
    public TComponent GetComponent<TComponent>(Id id) where TComponent : class
    {
        return _entities.CheckValid(id) && _registry.TryGetValue(typeof(TComponent), out var index) ?
            _components[index][id.Index] as TComponent : null;
    }
        
    /// <summary>
    /// Add a component to an entity. Will return false and fail to add component if the entity is not valid,
    /// or if a component of the same type is already attached to the entity.
    /// </summary>
    /// <param name="id">id of the entity to add the component to</param>
    /// <param name="component">component to add to the entity</param>
    /// <typeparam name="TComponent">type of component to add, must be a reference type/class</typeparam>
    /// <returns>
    /// Whether component add succeeded, it may fail if entity is not valid or if a component of the same
    /// type was already added to the entity. This is to prevent accidentally overwriting components, as each
    /// entity can only store one component of each unique class.
    /// </returns>
    public bool AddComponent<TComponent>(Id id, TComponent component) where TComponent : class
    {
        if (!_entities.CheckValid(id)) return false; // Don't check IsAlive now, check during impl because entity might have just been created
            
        // Get index of component list
        int index;
        if (_registry.TryGetValue(typeof(TComponent), out var outval))
        {
            index = outval;
        }
        else
        {
            index = _indexes.Count;
                
            // Register component before it gets added
            _registry.Add(typeof(TComponent), index);
            _indexes.Add(typeof(TComponent));
                
            // add an array in the components array
            var count = _components.Count > 0 ? _components[0].Count : _entities.Count;
            List<object> components = new(count);
            for (var i = 0; i < count; ++i)
            {
                components.Add(null);
            }
                
            _components.Add(components);
        }

        if (_components[index][id.Index] != null)
        {
            return false;
        }
        
        _components[index][id.Index] = component;
            
        _commandQueue.Add(new Command
        {
            Id = id,
            Component = component,
            TypeIndex = index,
            Type = CommandType.NotifyAddedComponent
        });
            
        return true;
    }
    
    /// <summary>
    /// Just notifies systems and those subscribed that a component was added, the actual work
    /// is done in AddComponent. Called during EntityContext.FlushCommands.
    /// </summary>
    /// <param name="id">id of the entity to add the component to</param>
    /// <param name="component">the component to add</param>
    /// <param name="index">index of the component type in the registry</param>
    /// <exception cref="Exception">
    ///     If a component of type `TComponent` has already been added to the entity
    /// </exception>
    private void NotifyAddedComponentImpl(Id id, object component, int index)
    {
        if (IsAlive(id) && _components[index][id.Index] != null)
        {
            ComponentAdded?.Invoke(id, _indexes[index]);
        }
    }
    
    public bool RemoveComponent<TComponent>(Id id) where TComponent : class
    {
        if (_entities.CheckValid(id) && _registry.TryGetValue(typeof(TComponent), out var index) && 
            _components[index][id.Index] != null)
        {
            _components[index][id.Index] = null;
            _commandQueue.Add(new Command
            {
                Id = id,
                TypeIndex = index,
                Type = CommandType.NotifyRemovedComponent
            });

            return true;
        }

        return false;
    }
    
    private void NotifyRemovedComponentImpl(Id id, int index)
    {
        if (IsAlive(id) && _components[index][id.Index] == null)
        {
            ComponentRemoved?.Invoke(id, _indexes[index]);
        }

    }

    public int ComponentTypeCount => _indexes.Count;
    public int AliveEntityCount => _alive.Count;

    private void HandlePoolResized(int newSize)
    {
        for (var i = 0; i < _components.Count; ++i)
        {
            var compList = _components[i];
            compList.EnsureCapacity(newSize);
            while (compList.Count < newSize)
            {
                compList.Add(null);
            }
        }
    }
        
    public EntityGroup<TEntity, T> Group<T>()
        where T : class
    {
        return new(this);
    }
        
    public EntityGroup<TEntity, T1, T2> Group<T1, T2>()
        where T1 : class
        where T2 : class
    {
        return new(this);
    }
        
    public EntityGroup<TEntity, T1, T2, T3> Group<T1, T2, T3>()
        where T1 : class
        where T2 : class
        where T3 : class
    {
        return new(this);
    }
        
    public EntityGroup<TEntity, T1, T2, T3, T4> Group<T1, T2, T3, T4>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        return new(this);
    }
        
    public EntityGroup<TEntity, T1, T2, T3, T4, T5> Group<T1, T2, T3, T4, T5>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        return new(this);
    }
        
    public EntityGroup<TEntity, T1, T2, T3, T4, T5, T6> Group<T1, T2, T3, T4, T5, T6>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        return new(this);
    }
        
    public EntityGroup<TEntity, T1, T2, T3, T4, T5, T6, T7> Group<T1, T2, T3, T4, T5, T6, T7>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        return new(this);
    }
        
    public EntityGroup<TEntity, T1, T2, T3, T4, T5, T6, T7, T8> Group<T1, T2, T3, T4, T5, T6, T7, T8>()
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
        where T8 : class
    {
        return new(this);
    }

}
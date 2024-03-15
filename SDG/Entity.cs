using System;

namespace SDG;

/// <summary>
/// Example class for Entities inside an EntityContext.
/// Provides a convenience wrapper around entity-component functionality.
/// </summary>
public class Entity : IPoolable
{
    public Id Id { get; set; }
    private readonly EntityContext<Entity> _context;
    
    public Entity(EntityContext<Entity> context)
    {
        _context = context;
    }

    /// <summary>
    /// Add a component to the entity
    /// </summary>
    /// <param name="component"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public Entity AddComponent<T>(T component) where T : class
    {
        _context.AddComponent(Id, component);
        return this;
    }

    /// <summary>
    /// Get a component from the entity, or null if it doesn't exist
    /// </summary>
    /// <typeparam name="T">type of component to get - it must be a reference class type</typeparam>
    /// <returns>Component or null</returns>
    public T GetComponent<T>() where T : class
    {
        return _context.GetComponent<T>(Id);
    }

    public bool RemoveComponent<T>() where T : class
    {
        return _context.RemoveComponent<T>(Id);
    }

    /// <summary>
    /// Flag this entity to be destroyed. The entity and its components will be invalidated at the end of the frame.
    /// Safe to call if already destroyed.
    /// </summary>
    public void Destroy()
    {
        _context.DestroyEntity(Id);
    }

    public bool IsAlive => _context.IsAlive(Id);
}
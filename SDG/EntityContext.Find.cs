using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace SDG;

public partial class EntityContext<TEntity> where TEntity : class, IPoolable
{
    /// <summary>
    /// Result returned from EntityContext.TryGetComponent
    /// </summary>
    private enum TgcResult
    {
        Ok,
        Missing,
        NotRegistered,
    }

    // Same as GetComponent but no check for id
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TgcResult TryGetComponent<T>(int idIndex, out T t) where T : class
    {
        if (_registry.TryGetValue(typeof(T), out var index))
        {
            t = _components[index][idIndex] as T;
            return t == null ? TgcResult.Missing : TgcResult.Ok;
        }

        t = null;
        return TgcResult.NotRegistered;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool FindById<T>(Id id, out  (TEntity, T) tup) where T : class
    {
        if (TryGetComponent<T>(id.Index, out var t) != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        tup = (_entities.GetEntityById(id), t);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool FindById<T1, T2>(Id id, out (TEntity, T1, T2) tup) 
        where T1 : class
        where T2 : class
    {
        var result = TryGetComponent<T1>(id.Index, out var t1);
        if (result != TgcResult.Ok)            
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T2>(id.Index, out var t2);
        if (result != TgcResult.Ok)            
        {
            tup = default;
            return false;
        }
        
        tup = (_entities.GetEntityById(id), t1, t2);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool FindById<T1, T2, T3>(Id id, out (TEntity, T1, T2, T3) tup) 
        where T1 : class
        where T2 : class
        where T3 : class
    {
        var result = TryGetComponent<T1>(id.Index, out var t1);
        if (result != TgcResult.Ok)            
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T2>(id.Index, out var t2);
        if (result != TgcResult.Ok)             
        {
            tup = default;
            return false;
        }

        result = TryGetComponent<T3>(id.Index, out var t3);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        tup = (_entities.GetEntityById(id), t1, t2, t3);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool FindById<T1, T2, T3, T4>(Id id, out (TEntity, T1, T2, T3, T4) tup) 
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
    {
        var result = TryGetComponent<T1>(id.Index, out var t1);
        if (result != TgcResult.Ok)            
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T2>(id.Index, out var t2);
        if (result != TgcResult.Ok)             
        {
            tup = default;
            return false;
        }

        result = TryGetComponent<T3>(id.Index, out var t3);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T4>(id.Index, out var t4);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        tup = (_entities.GetEntityById(id), t1, t2, t3, t4);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool FindById<T1, T2, T3, T4, T5>(Id id, out (TEntity, T1, T2, T3, T4, T5) tup) 
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        var result = TryGetComponent<T1>(id.Index, out var t1);
        if (result != TgcResult.Ok)            
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T2>(id.Index, out var t2);
        if (result != TgcResult.Ok)             
        {
            tup = default;
            return false;
        }

        result = TryGetComponent<T3>(id.Index, out var t3);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T4>(id.Index, out var t4);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T5>(id.Index, out var t5);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        tup = (_entities.GetEntityById(id), t1, t2, t3, t4, t5);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool FindById<T1, T2, T3, T4, T5, T6>(Id id, out (TEntity, T1, T2, T3, T4, T5, T6) tup) 
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        var result = TryGetComponent<T1>(id.Index, out var t1);
        if (result != TgcResult.Ok)            
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T2>(id.Index, out var t2);
        if (result != TgcResult.Ok)             
        {
            tup = default;
            return false;
        }

        result = TryGetComponent<T3>(id.Index, out var t3);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T4>(id.Index, out var t4);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T5>(id.Index, out var t5);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }            
        
        result = TryGetComponent<T6>(id.Index, out var t6);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        tup = (_entities.GetEntityById(id), t1, t2, t3, t4, t5, t6);
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool FindById<T1, T2, T3, T4, T5, T6, T7>(Id id, out (TEntity, T1, T2, T3, T4, T5, T6, T7) tup) 
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        var result = TryGetComponent<T1>(id.Index, out var t1);
        if (result != TgcResult.Ok)            
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T2>(id.Index, out var t2);
        if (result != TgcResult.Ok)             
        {
            tup = default;
            return false;
        }

        result = TryGetComponent<T3>(id.Index, out var t3);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T4>(id.Index, out var t4);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T5>(id.Index, out var t5);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }            
        
        result = TryGetComponent<T6>(id.Index, out var t6);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T7>(id.Index, out var t7);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        tup = (_entities.GetEntityById(id), t1, t2, t3, t4, t5, t6, t7);
        return true;
    }
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool FindById<T1, T2, T3, T4, T5, T6, T7, T8>(Id id, out (TEntity, T1, T2, T3, T4, T5, T6, T7, T8) tup) 
        where T1 : class
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
        where T8 : class
    {
        var result = TryGetComponent<T1>(id.Index, out var t1);
        if (result != TgcResult.Ok)            
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T2>(id.Index, out var t2);
        if (result != TgcResult.Ok)             
        {
            tup = default;
            return false;
        }

        result = TryGetComponent<T3>(id.Index, out var t3);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T4>(id.Index, out var t4);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T5>(id.Index, out var t5);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }            
        
        result = TryGetComponent<T6>(id.Index, out var t6);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T7>(id.Index, out var t7);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        result = TryGetComponent<T8>(id.Index, out var t8);
        if (result != TgcResult.Ok)
        {
            tup = default;
            return false;
        }
        
        tup = (_entities.GetEntityById(id), t1, t2, t3, t4, t5, t6, t7, t8);
        return true;
    }

    public IEnumerable<(TEntity, T)> Find<T>() where T : class
    {
        foreach (var id in _alive)
        {
            var result = TryGetComponent<T>(id.Index, out var t);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;

            yield return (_entities.GetEntityById(id), t);
        }
    }
    
    public IEnumerable<(TEntity, T1, T2)> Find<T1, T2>() 
        where T1 : class 
        where T2 : class
    {
        foreach (var id in _alive)
        {
            var result = TryGetComponent<T1>(id.Index, out var t1);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T2>(id.Index, out var t2);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            yield return (_entities.GetEntityById(id), t1, t2);
        }
    }
    
    public IEnumerable<(TEntity, T1, T2, T3)> Find<T1, T2, T3>() 
        where T1 : class 
        where T2 : class
        where T3 : class
    {
        foreach (var id in _alive)
        {
            var result = TryGetComponent<T1>(id.Index, out var t1);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T2>(id.Index, out var t2);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T3>(id.Index, out var t3);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            yield return (_entities.GetEntityById(id), t1, t2, t3);
        }
    }
    
    public IEnumerable<(TEntity, T1, T2, T3, T4)> Find<T1, T2, T3, T4>() 
        where T1 : class 
        where T2 : class
        where T3 : class
        where T4 : class
    {
        foreach (var id in _alive)
        {
            var result = TryGetComponent<T1>(id.Index, out var t1);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T2>(id.Index, out var t2);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T3>(id.Index, out var t3);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T4>(id.Index, out var t4);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            yield return (_entities.GetEntityById(id), t1, t2, t3, t4);
        }
    }
    
    public IEnumerable<(TEntity, T1, T2, T3, T4, T5)> Find<T1, T2, T3, T4, T5>() 
        where T1 : class 
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
    {
        foreach (var id in _alive)
        {
            var result = TryGetComponent<T1>(id.Index, out var t1);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T2>(id.Index, out var t2);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T3>(id.Index, out var t3);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T4>(id.Index, out var t4);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T5>(id.Index, out var t5);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;

            yield return (_entities.GetEntityById(id), t1, t2, t3, t4, t5);
        }
    }
    
    public IEnumerable<(TEntity, T1, T2, T3, T4, T5, T6)> Find<T1, T2, T3, T4, T5, T6>() 
        where T1 : class 
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
    {
        foreach (var id in _alive)
        {
            var result = TryGetComponent<T1>(id.Index, out var t1);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T2>(id.Index, out var t2);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T3>(id.Index, out var t3);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T4>(id.Index, out var t4);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T5>(id.Index, out var t5);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T6>(id.Index, out var t6);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;

            yield return (_entities.GetEntityById(id), t1, t2, t3, t4, t5, t6);
        }
    }
    
    public IEnumerable<(TEntity, T1, T2, T3, T4, T5, T6, T7)> Find<T1, T2, T3, T4, T5, T6, T7>() 
        where T1 : class 
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
    {
        foreach (var id in _alive)
        {
            var result = TryGetComponent<T1>(id.Index, out var t1);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T2>(id.Index, out var t2);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T3>(id.Index, out var t3);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T4>(id.Index, out var t4);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T5>(id.Index, out var t5);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T6>(id.Index, out var t6);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T7>(id.Index, out var t7);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;

            yield return (_entities.GetEntityById(id), t1, t2, t3, t4, t5, t6, t7);
        }
    }
    
    public IEnumerable<(TEntity, T1, T2, T3, T4, T5, T6, T7, T8)> Find<T1, T2, T3, T4, T5, T6, T7, T8>() 
        where T1 : class 
        where T2 : class
        where T3 : class
        where T4 : class
        where T5 : class
        where T6 : class
        where T7 : class
        where T8 : class
    {
        foreach (var id in _alive)
        {
            var result = TryGetComponent<T1>(id.Index, out var t1);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T2>(id.Index, out var t2);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T3>(id.Index, out var t3);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T4>(id.Index, out var t4);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T5>(id.Index, out var t5);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T6>(id.Index, out var t6);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T7>(id.Index, out var t7);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;
            
            result = TryGetComponent<T8>(id.Index, out var t8);
            if (result == TgcResult.Missing) continue;
            if (result == TgcResult.NotRegistered) break;

            yield return (_entities.GetEntityById(id), t1, t2, t3, t4, t5, t6, t7, t8);
        }
    }

}
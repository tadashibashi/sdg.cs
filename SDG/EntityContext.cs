using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SDG
{
    internal class EntityContext<T> where T : class, IPoolable
    {
        Dictionary<Type, int> _componentTypes;
        List<List<object>> _components;

        Pool<T> _entities;

        public EntityContext() { }


    }
}

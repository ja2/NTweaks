using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace NTweaks.Collections.Generic
{

    /// <summary>
    /// Many to many dictionary implementation
    /// </summary>
    public class LookupMap<T1, T2>
    {
        #region "MapSide"

        public class MapSide<T3, T4>
        {

            /// <summary>
            /// Defines the method to be called when a remove method is called on a mapside
            /// </summary>
            internal delegate void Remover(T3 t3, T4 t4);

            private readonly Lookup<T3, T4> _lookup;

            private readonly Remover _remover;
            internal MapSide(Lookup<T3, T4> lookup, Remover remover)
            {
                _lookup = lookup;
                _remover = remover;
            }

            /// <summary>
            /// Get items that match the key for this side of the mapping
            /// </summary>
            public IEnumerable<T4> this[T3 key]
            {
                get
                {
                    if (key == null)
                        throw new ArgumentNullException("Key");

                    return _lookup[key];

                }
            }

            /// <summary>
            /// Are there any items that match the key for this side of the mapping
            /// </summary>            
            public bool ContainsKey(T3 key)
            {

                if (key == null)
                    throw new ArgumentNullException("Key");

                return _lookup.ContainsKey(key);
            }

            /// <summary>
            /// Remove any items that match the key for this side of the mapping
            /// </summary>
            public int Remove(T3 key)
            {

                if (key == null)
                    throw new ArgumentNullException("key");

                if (!_lookup.ContainsKey(key))
                    return 0;

                var ret = _lookup[key].Count();

                //Allow for modification of the backing lists
                foreach (var v in _lookup[key].ToList())
                {
                    _remover(key, v);
                }

                return ret;

            }

        }

        #endregion

        private readonly Lookup<T1, T2> _forward;
        private readonly Lookup<T2, T1> _reverse;
        private readonly MapSide<T1, T2> _sidForward;

        private readonly MapSide<T2, T1> _sidReverse;

        /// <summary>
        /// Main constructor - use this one
        /// </summary>
        public LookupMap()
            : this(null, null)
        {
        }

        /// <summary>
        /// Constructor for dependency injection - for unit tests only
        /// </summary>
        public LookupMap(Lookup<T1, T2> forward, Lookup<T2,T1> reverse)
        {
            _forward = forward ?? new Lookup<T1, T2>();
            _reverse = reverse ?? new Lookup<T2, T1>();

            //Define the side, including the functions sort out the argument order for the remove function
            _sidForward = new MapSide<T1, T2>(_forward, (T1 t1, T2 t2) => { Remove(t1, t2); });
            _sidReverse = new MapSide<T2, T1>(_reverse, (T2 t2, T1 t1) => { Remove(t1, t2); });
        }

        /// <summary>
        /// The total count of all items in the map
        /// </summary>
        public int Count()
        {

            return _forward.Count();

        }

        /// <summary>
        /// Adds the specified values to the map
        /// </summary>
        public void Add(T1 t1, T2 t2)
        {

            lock (_forward)
            {
                _forward.Add(t1, t2);
                _reverse.Add(t2, t1);

            }
        }

        /// <summary>
        /// Removing from both side of the map
        /// </summary>
        /// <remarks></remarks>
        public void Remove(T1 t1, T2 t2)
        {
            lock (_forward)
            {
                _forward.Remove(t1, t2);
                _reverse.Remove(t2, t1);
            }

        }

        /// <summary>
        /// Is there an item in the map with both sides matching
        /// </summary>
        public object Contains(T1 t1, T2 t2)
        {
            return _forward.ContainsKey(t1) && _reverse.ContainsKey(t2);
        }

        /// <summary>
        /// The map from T1 to T2
        /// </summary>
        public MapSide<T1, T2> Forward
        {
            get { return _sidForward; }
        }

        /// <summary>
        /// The map from T2 to T1
        /// </summary>
        public MapSide<T2, T1> Reverse
        {
            get { return _sidReverse; }
        }

    }

}
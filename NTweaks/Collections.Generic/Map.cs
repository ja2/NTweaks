using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace NTweaks.Collections.Generic
{

    /// <summary>
    /// Bi-directional dictionary implementation
    /// </summary>
    /// <remarks>Converted (and extended) from here: http://stackoverflow.com/a/10966684</remarks>
    public class Map<T1, T2>
    {

        #region "MapSide"

        public class MapSide<T3, T4>
        {

            /// <summary>
            /// Defines the method to be called when a remove method is called on a mapside
            /// </summary>
            internal delegate void Remover(T3 t3, T4 t4);

            private readonly Dictionary<T3, T4> _dictionary;

            private readonly Remover _remover;
            internal MapSide(Dictionary<T3, T4> dict, Remover remover)
            {
                _dictionary = dict;
                _remover = remover;
            }

            /// <summary>
            /// Get the value match the key for this side of the mapping
            /// </summary>
            public T4 this[T3 key]
            {
                get { return _dictionary[key]; }
                set { _dictionary[key] = value; }
            }

            /// <summary>
            /// Is there an items that matches the key for this side of the mapping
            /// </summary>          
            public bool ContainsKey(T3 key)
            {
                return _dictionary.ContainsKey(key);
            }

            /// <summary>
            /// Remove any item that matches the key for this side of the mapping
            /// </summary>
            public void Remove(T3 key)
            {
                if (!_dictionary.ContainsKey(key))
                    return;
                _remover(key, _dictionary[key]);
            }
        }

        #endregion

        private readonly Dictionary<T1, T2> _forward;
        private readonly Dictionary<T2, T1> _reverse;
        private readonly MapSide<T1, T2> _sidForward;

        private readonly MapSide<T2, T1> _sidReverse;

        /// <summary>
        /// Main constructor - use this one
        /// </summary>
        public Map()
            : this(null, null)
        {}

        /// <summary>
        /// Constructor for dependency injection - for unit tests only
        /// </summary>
        public Map(Dictionary<T1, T2> forward, Dictionary<T2, T1> reverse)
        {
            _forward = forward ?? new Dictionary<T1, T2>();
            _reverse = reverse ?? new Dictionary<T2, T1>();

            //Define the side, including the functions sort out the argument order for the remove function
            _sidForward = new MapSide<T1, T2>(_forward, (T1 t1, T2 t2) => { Remove(t1, t2); });
            _sidReverse = new MapSide<T2, T1>(_reverse, (T2 t2, T1 t1) => { Remove(t1, t2); });
        }

        /// <summary>
        /// Adds the specified values to the map
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <remarks></remarks>

        public void Add(T1 t1, T2 t2)
        {
            if (t1 == null)
                throw new ArgumentNullException("t1");
            if (t2 == null)
                throw new ArgumentNullException("t2");


            lock (_forward)
            {
                _forward.Add(t1, t2);
                _reverse.Add(t2, t1);

            }
        }

        /// <summary>
        /// The count of mappings added to the map
        /// </summary>
        public int Count()
        {

            return _forward.Count;

        }

        /// <summary>
        /// Removing from both side of the map
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <remarks></remarks>

        private void Remove(T1 t1, T2 t2)
        {
            lock (_forward)
            {
                _forward.Remove(t1);
                _reverse.Remove(t2);
            }

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

        /// <summary>
        /// Return a distinct list of items on the left side of the mapping
        /// </summary>
        public IEnumerable<T1> AllLeft()
        {
            return _forward.Keys;
        }

        /// <summary>
        /// Return a distinct list of items on the right side of the mapping
        /// </summary>
        public IEnumerable<T2> AllRight()
        {
            return _reverse.Keys;
        }

        /// <summary>
        /// Return a list of all keys and their values
        /// </summary>        
        public IEnumerable<KeyValuePair<T1, T2>> All()
        {
             return _forward.ToList();
        }

    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace NTweaks.Collections.Generic
{
    /// <summary>
    /// One to many dictionary implementation
    /// </summary>
    public class Lookup<TKey, TValue>
    {
        
        private readonly Dictionary<TKey, List<TValue>> _dict;

        /// <summary>
        /// Main constructor - use this one
        /// </summary>
        public Lookup()
            : this(null)
        {
        }

        /// <summary>
        /// Constructor for dependency injection - for unit tests only
        /// </summary>
        public Lookup(Dictionary<TKey, List<TValue>> dict)
        {
            _dict = dict ?? new Dictionary<TKey, List<TValue>>();
        }

        /// <summary>
        /// The total count of items in the lookup
        /// </summary>
        public int Count()
        {
            return _dict.Values.SelectMany(v => v).Count();   
        }

        /// <summary>
        /// The count of items for the specified key
        /// </summary>
        public int Count(TKey key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            return _dict[key].Count;
        }


        /// <summary>
        /// Add an item to the lookup
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            if (key == null)
                throw new ArgumentNullException("key");


            lock (_dict)
            {
                List<TValue> values = null;


                if (!_dict.TryGetValue(key, out values))
                {
                    values = new List<TValue>();

                    _dict.Add(key, values);

                }


                lock (values)
                {

                    if (!values.Contains(value))
                    {
                        values.Add(value);

                    }

                }

            }

        }

        /// <summary>
        /// Return values for a key
        /// </summary>
        /// <remarks>Returns an empty list of TValue where the key is not present at all</remarks>
        public IEnumerable<TValue> this[TKey key]
        {

            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");


                if (_dict.ContainsKey(key))
                {
                    return _dict[key];

                }

                return Enumerable.Empty<TValue>();

            }
        }


        /// <summary>
        /// Check that the lookup has values for the specified key
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            return _dict.ContainsKey(key);
        }


        /// <summary>
        /// Remove all items for a key
        /// </summary>
        public bool Remove(TKey key)
        {

            if (key == null)
                throw new ArgumentNullException("key");


            lock (_dict)
            {
                return _dict.Remove(key);

            }

        }

        /// <summary>
        /// Remove items that match the key and value
        /// </summary>
        /// <returns>Returns false if there was nothing to remove, true if items were removed.</returns>
        public bool Remove(TKey key, TValue value)
        {

            if (key == null)
                throw new ArgumentNullException("key");


            lock (_dict)
            {
                if (!_dict.ContainsKey(key))
                    return false;

                var items = _dict[key];
                
                lock (items)
                {
                    dynamic count = items.RemoveAll(v => v.Equals(value));

                    if (items.Count == 0)
                        _dict.Remove(key);

                    return count > 0;

                }

            }

        }

        /// <summary>
        /// Return a list of all keys in the lookup
        /// </summary>
        public IEnumerable<TKey> Keys()
        {
            return _dict.Keys;
        }

        /// <summary>
        /// Return a distinct list of all values in the lookup
        /// </summary>
        public IEnumerable<TValue> Values()
        {
            return _dict.Values.SelectMany(v => v).Distinct();
        }

        /// <summary>
        /// Return a list of all keys and their values
        /// </summary>        
        public IEnumerable<KeyValuePair<TKey,TValue>> All()
        {
            return (from k in _dict.ToList()
                    from v in k.Value
                    select new KeyValuePair<TKey,TValue>(k.Key, v));
        }
    }

}
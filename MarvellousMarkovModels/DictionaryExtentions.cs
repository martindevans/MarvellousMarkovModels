using System;
using System.Collections.Generic;

namespace MarvellousMarkovModels
{
    static class DictionaryExtentions
    {
        public static void AddOrUpdate<K, V>(this Dictionary<K, V> dict, K key, V value, Func<V, V> update)
        {
            V existing;
            if (!dict.TryGetValue(key, out existing))
            {
                dict.Add(key, value);
            }
            else
            {
                dict[key] = update(existing);
            }
        }
    }
}

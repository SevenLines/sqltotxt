﻿using System.Collections.Generic;

namespace sqltotxt
{
    public static class DictionaryExtensions
    {
        public static TValue Default<TKey, TValue>
            (this IDictionary<TKey, TValue> dictionary, TKey key, TValue defValue)
        {
            TValue ret;
            return dictionary.TryGetValue(key, out ret) ? ret : defValue;
        }
    }
}
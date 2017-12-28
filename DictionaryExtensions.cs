using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Relentless {

    public static class DictionaryExtensions {

        public static void AddIfNotExists<TKey, TItem>(this Dictionary<TKey, TItem> dictionary, TKey key, TItem item) {
            if(dictionary == null) {
                throw new ArgumentNullException("dictionary");
            }
            if(!dictionary.ContainsKey(key)) {
                dictionary.Add(key, item);
            }
        }

    }

}

namespace RSToolkit.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public static class CollectionHelpers 
    {
        public static int GetCircularIndex(int index, int size){
            return ((index % size) + size) % size;
        }

        public static T Majority<T>(this IEnumerable<T> self) {
            return self.GroupBy(x => x)
                           .OrderByDescending(g => g.Count())
                           .First()
                           .Key;
        }
    }
}
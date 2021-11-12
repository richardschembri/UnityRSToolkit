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

        public static int GetNextCircularIndex(int index, int size){
            index++;
            return GetCircularIndex(index, size);
        }

        /// <summary>
        /// Get Majorit element of IEnumerable
        /// </summary>
        public static T Majority<T>(this IEnumerable<T> self) {
            return self.GroupBy(x => x)
                           .OrderByDescending(g => g.Count())
                           .First()
                           .Key;
        }

        /// <summary>
        /// Splits an array into several smaller arrays.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="array">The array to split.</param>
        /// <param name="size">The size of the smaller arrays.</param>
        /// <returns>An array containing smaller arrays.</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size)
        {
            for (var i = 0; i < (float)array.Length / size; i++)
            {
                yield return array.Skip(i * size).Take(size);
            }
        }
    }
}
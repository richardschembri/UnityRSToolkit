namespace RSToolkit.Helpers
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    public static class TransformHelpers
    {
        public static T[] GetTopLevelChildren<T>(this T self) where T : Transform{
            return self.GetTopLevelChildrenEnumerable().ToArray();
        }
        /// <summary>
        /// Depth First Search of Rect Transforms
        /// </summary>
        public static IEnumerable<T> GetTopLevelChildrenEnumerable<T>(this T self) where T : Transform{
            for(int i = 0; i < self.childCount; i++){
                var c = self.GetChild(i);
                if (c is T){
                    yield return (T)self.GetChild(i);
                }
            }
        }
    }
}

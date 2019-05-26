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

        public static void ResetScaleAndRotation(this Transform t)
        {
            t.localEulerAngles = new Vector3(0f, 0f, 0f);
            t.localScale = new Vector3(1f, 1f, 1f);
        }

        public static void CopyScaleAndRotation(this Transform t, Transform from)
        {
            t.localEulerAngles = new Vector3(from.localEulerAngles.x, from.localEulerAngles.y, from.localEulerAngles.z);
            t.localScale = new Vector3(from.localScale.x, from.localScale.y, from.localScale.z);
        }
    }
}

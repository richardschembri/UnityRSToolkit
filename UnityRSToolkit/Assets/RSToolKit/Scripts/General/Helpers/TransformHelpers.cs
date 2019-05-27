namespace RSToolkit.Helpers
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    public static class TransformHelpers
    {
        public static T[] GetTopLevelChildren<T>(this Transform self){
            return self.GetTopLevelChildrenEnumerable<T>().ToArray();
        }
        /// <summary>
        /// Depth First Search of Rect Transforms
        /// </summary>
        public static IEnumerable<T> GetTopLevelChildrenEnumerable<T>(this Transform self) {
            for(int i = 0; i < self.childCount; i++){
                var c = self.GetChild(i).GetComponent<T>();
                if (c != null){
                    yield return c;
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

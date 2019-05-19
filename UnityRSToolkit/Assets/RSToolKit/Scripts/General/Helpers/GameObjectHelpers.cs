namespace RSToolkit.Helpers
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class GameObjectHelpers
    {
        public static T GetGameObject<T>(string name)
        {
            return GameObject.Find(name).GetComponent<T>();
        }

        public static T GetGameObject<T>(T gameObject, string name)
        {
            if (gameObject == null)
            {
                gameObject = GameObject.Find(name).GetComponent<T>();
            }

            return gameObject;
        }

        public static Toggle GetToggle(Toggle toggle, string name)
        {

            if (toggle == null)
            {
                toggle = GameObject.Find(name).GetComponent<Toggle>();
            }
            return toggle;
        }

        public static GameObject GetGameObject(GameObject go, string name)
        {
            if (go == null)
            {
                go = GameObject.Find(name);
            }
            return go;
        }

        public static GameObject GetChild(GameObject parent, string childName)
        {
            Component[] transforms = parent.GetComponentsInChildren(typeof(Transform), true);

            foreach (Transform transform in transforms)
            {
                if (transform.gameObject.name == childName)
                {
                    return transform.gameObject;
                }
            }

            return null;
        }
    

        public static float GetTransformTopY(GameObject go)
        {
            return go.transform.position.y + (go.GetComponent<Renderer>().bounds.size.y / 2);

        }
        
        public static float GetTransformBottomY(GameObject go)
        {
            return go.transform.position.y - (go.GetComponent<Renderer>().bounds.size.y / 2);

        }
        public static float GetTransformRightX(Transform goTransform)
        {
            return goTransform.position.x + (goTransform.localScale.x / 2);
        }

        public static float GetTransformRightX(GameObject go)
        {
            return go.transform.position.x + (go.GetComponent<Renderer>().bounds.size.x / 2);

        }

        public static float GetTransformLeftX(GameObject go)
        {
            return go.transform.position.x - (go.GetComponent<Renderer>().bounds.size.x / 2);

        }

        public static void FlipX(Transform t, bool Flip)
        {
            var ScaleX = Mathf.Abs(t.localScale.x);
            if (Flip)
            {
                ScaleX = -ScaleX;
            }
            t.localScale = new Vector3(ScaleX, t.localScale.y, t.localScale.z);
        }

        public static void FlipY(Transform t, bool Flip)
        {
            var ScaleY = Mathf.Abs(t.localScale.y);
            if (Flip)
            {
                ScaleY = -ScaleY;
            }
            t.localScale = new Vector3(t.localScale.x, ScaleY, t.localScale.z);
        }

        public static void NormalizeTransform(Transform t){
            t.localEulerAngles = new Vector3(0f, 0f, 0f);
            t.localScale = new Vector3(1f, 1f, 1f);
        }

        public static void CopyTransformValues(Transform source, Transform target, bool includingParent = false)
        {
            if (includingParent)
            {
                target.parent = source.parent;
            }
            
            target.localPosition = source.localPosition;
            target.localRotation = source.localRotation;
            target.localScale = source.localScale;
        }
    }
}

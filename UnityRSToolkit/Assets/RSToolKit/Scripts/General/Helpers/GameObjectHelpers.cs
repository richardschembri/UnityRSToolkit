namespace RSToolkit.Helpers
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public static class GameObjectHelpers
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

        public static GameObject GetChild(this GameObject parent, string childName)
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

        public static bool HasChild(this GameObject source, GameObject target){
            return source.transform.HasChild(target.transform);
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



    }
}

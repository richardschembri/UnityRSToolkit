namespace RSToolkit.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RSToolkit.Helpers;

    public class Spawner : MonoBehaviour
    {
        public int SpawnLimit = 1;

        public GameObject GameObjectToSpawn;

        private List<GameObject> m_spawnedGameObjects;
        public List<GameObject> SpawnedGameObjects
        {
            get
            {
                return m_spawnedGameObjects;
            }
            private set
            {
                m_spawnedGameObjects = value;
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public GameObject SpawnGameObject(bool useSpawnerTransformValues = true)
        {
            if (SpawnedGameObjects.Count == SpawnLimit)
            {
                return null;
            }

            var spawnedGameObject = Instantiate(GameObjectToSpawn);
            if (useSpawnerTransformValues)
            {
                GameObjectHelpers.CopyTransformValues(gameObject.transform, spawnedGameObject.transform, true);
            }
            else
            {
                GameObjectHelpers.CopyTransformValues(GameObjectToSpawn.transform, spawnedGameObject.transform, false);
            }
            SpawnedGameObjects.Add(spawnedGameObject);
            return spawnedGameObject;
        }

        public bool DestroyLastSpawnedGameObject()
        {
            if (SpawnedGameObjects.Count > 0)
            {
                var spawnedGameObject = SpawnedGameObjects[SpawnedGameObjects.Count - 1];
                return DestroySpawnedGameObject(spawnedGameObject);
            }
            return false;
        }

        public bool DestroySpawnedGameObject(GameObject spawnedGameObject)
        {
            if (spawnedGameObject != null && SpawnedGameObjects.Contains(spawnedGameObject)){
                Destroy(spawnedGameObject);
                SpawnedGameObjects.Remove(spawnedGameObject);
                return true;
            }

            return false;
        }


    }
}
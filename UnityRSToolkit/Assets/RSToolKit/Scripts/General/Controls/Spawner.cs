namespace RSToolkit.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RSToolkit.Helpers;

    public class Spawner : MonoBehaviour
    {
        public int SpawnLimit = -1;
        public bool isParent = false;
        public GameObject GameObjectToSpawn;

        private List<GameObject> m_spawnedGameObjects;
        public List<GameObject> SpawnedGameObjects
        {
            get
            {
                if(m_spawnedGameObjects == null){
                    m_spawnedGameObjects = new List<GameObject>();
                }
                return m_spawnedGameObjects;
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
            if (SpawnLimit > 0 && SpawnedGameObjects.Count >= SpawnLimit){
                return null;
            }

            var spawnedGameObject = Instantiate(GameObjectToSpawn);
            if (useSpawnerTransformValues)
            {
                GameObjectHelpers.CopyTransformValues(gameObject.transform, spawnedGameObject.transform, false);
            }
            else
            {
                GameObjectHelpers.CopyTransformValues(GameObjectToSpawn.transform, spawnedGameObject.transform, false);
                spawnedGameObject.transform.localPosition = transform.localPosition;
            }
            if(isParent){
                spawnedGameObject.transform.SetParent(transform);
            }else{
                spawnedGameObject.transform.SetParent(transform.parent);
            }
            SpawnedGameObjects.Add(spawnedGameObject);
            return spawnedGameObject;
        }

        public void DestroyLastSpawnedGameObject()
        {
            if (SpawnedGameObjects.Count > 0)
            {
                var spawnedGameObject = SpawnedGameObjects[SpawnedGameObjects.Count - 1];
                DestroySpawnedGameObject(spawnedGameObject);
            }
        }

        public void DestroySpawnedGameObject(GameObject spawnedGameObject)
        {
            if (spawnedGameObject != null && SpawnedGameObjects.Contains(spawnedGameObject)){
                Destroy(spawnedGameObject);
                SpawnedGameObjects.Remove(spawnedGameObject);
            }
        }


    }
}
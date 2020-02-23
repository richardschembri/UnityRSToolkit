namespace RSToolkit.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RSToolkit.Helpers;

    public abstract class SpawnerCore : MonoBehaviour
    {
        public int SpawnLimit = -1;
        public bool isParent = false;
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
                DestroyImmediate(spawnedGameObject);
                SpawnedGameObjects.Remove(spawnedGameObject);
            }
        }
        public void DestroyAllSpawns(){
            if (SpawnedGameObjects.Count > 0){
                DestroySpawnedGameObject(SpawnedGameObjects[SpawnedGameObjects.Count -1]);
                DestroyAllSpawns();
            }
        }
        public GameObject SpawnAndGetGameObject(GameObject gameObjectToSpawn ,bool useSpawnerTransformValues = true)
        {
            if (SpawnLimit > 0 && SpawnedGameObjects.Count >= SpawnLimit){
                return null;
            }

            var spawnedGameObject = Instantiate(gameObjectToSpawn);

            if(isParent){
                spawnedGameObject.transform.SetParent(transform);
            }else{
                spawnedGameObject.transform.SetParent(transform.parent);
            }

            if (useSpawnerTransformValues)
            {
                gameObject.transform.CopyValuesTo(spawnedGameObject.transform, false);
            }
            else
            {
                spawnedGameObject.transform.ResetScaleAndRotation();
                spawnedGameObject.transform.localPosition = transform.localPosition;
            }

            SpawnedGameObjects.Add(spawnedGameObject);
            return spawnedGameObject;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}
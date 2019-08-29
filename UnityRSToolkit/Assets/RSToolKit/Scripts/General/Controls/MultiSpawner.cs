namespace RSToolkit.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MultiSpawner : SpawnerCore
    {
        public GameObject[] GameObjectsToSpawn;
        public GameObject SpawnAndGetGameObject(int gameObjectToSpawnIndex = 0 ,bool useSpawnerTransformValues = true)
        {
            return SpawnAndGetGameObject(GameObjectsToSpawn[gameObjectToSpawnIndex] ,useSpawnerTransformValues);
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
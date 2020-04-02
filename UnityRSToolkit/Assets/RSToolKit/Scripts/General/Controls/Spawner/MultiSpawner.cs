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

        public GameObject SpawnAndGetRandomGameObject(bool useSpawnerTransformValues = true)
        {
            int randomIndex = Random.Range(0, GameObjectsToSpawn.Length);
            return SpawnAndGetGameObject(randomIndex ,useSpawnerTransformValues);
        }
    }
}
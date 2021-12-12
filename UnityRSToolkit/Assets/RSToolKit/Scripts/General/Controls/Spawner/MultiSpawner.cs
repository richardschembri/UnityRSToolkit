namespace RSToolkit.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MultiSpawner<T> : SpawnerCore<T> where T : MonoBehaviour
    {
        public T[] GameObjectsToSpawn;
        public T SpawnAndGetGameObject(int gameObjectToSpawnIndex = 0 ,bool useSpawnerTransformValues = true, bool force = false)
        {
            return SpawnAndGetGameObject(GameObjectsToSpawn[gameObjectToSpawnIndex] ,useSpawnerTransformValues, force);
        }

        public T SpawnAndGetRandomGameObject(bool useSpawnerTransformValues = true)
        {
            int randomIndex = Random.Range(0, GameObjectsToSpawn.Length);
            return SpawnAndGetGameObject(randomIndex ,useSpawnerTransformValues);
        }
    }
}
using UnityEngine;

namespace RSToolkit.Controls
{
    public class Spawner<T> : SpawnerCore<T> where T : MonoBehaviour
    {
        public T GameObjectToSpawn;
        public void SpawnGameObject(bool useSpawnerTransformValues = true){
            SpawnAndGetGameObject(useSpawnerTransformValues);
        }

        public T SpawnAndGetGameObject(bool useSpawnerTransformValues = true)
        {
            return SpawnAndGetGameObject(GameObjectToSpawn, useSpawnerTransformValues);
        }

    }
}
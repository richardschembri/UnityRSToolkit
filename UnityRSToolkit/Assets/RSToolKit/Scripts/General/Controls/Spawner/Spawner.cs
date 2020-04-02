namespace RSToolkit.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using RSToolkit.Helpers;

    public class Spawner : SpawnerCore
    {
        public GameObject GameObjectToSpawn;
        public void SpawnGameObject(bool useSpawnerTransformValues = true){
            SpawnAndGetGameObject(useSpawnerTransformValues);
        }

        public GameObject SpawnAndGetGameObject(bool useSpawnerTransformValues = true)
        {
            return SpawnAndGetGameObject(GameObjectToSpawn, useSpawnerTransformValues);
        }

    }
}
using UnityEngine;

namespace RSToolkit.Controls
{
    public class Spawner<T> : SpawnerCore<T> where T : MonoBehaviour
    {
        public T GameObjectToSpawn;

        [SerializeField]
        protected int _preSpawnCount = -1;
        public void SpawnGameObject(bool useSpawnerTransformValues = true, bool force = false){
            SpawnAndGetGameObject(useSpawnerTransformValues, force);
        }

        public T SpawnAndGetGameObject(bool useSpawnerTransformValues = true, bool force = false)
        {
            return SpawnAndGetGameObject(GameObjectToSpawn, useSpawnerTransformValues, force);
        }

        private void PreSpawn()
        {
            if(_preSpawnCount < SpawnedGameObjects.Count){
                return;
            }
            int remaining = SpawnedGameObjects.Count - _preSpawnCount;
            for(int i = 0; i < remaining; i++){
                SpawnGameObject();
            }
        }

        public override bool Init(bool force = false)
        {
            if( base.Init(force)){
                PreSpawn();
                return true;
            }
            return false;
        }

    }
}
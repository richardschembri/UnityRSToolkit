using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.Controls
{
    public abstract class AutoSpawnerCore<T> : Spawner<T> where T : MonoBehaviour
    {
        public float timeFrom = 3f;
        public float timeTo = 4f;
        public float spawnOffset = 0.1f;

        public bool Paused {get; set;} = false;

        public bool IsSpawning { get; private set; } = false; 

        public int SpawningCount {get; private set;} = 0;
        public int SpawningRemaining {get; private set;} = 0;
        public void StartAutoSpawn(bool useSpawnerTransformValues = true)
        {
            if (!IsSpawning)
            {
                IsSpawning = true;
                StartCoroutine(AutoSpawn(useSpawnerTransformValues ));
            }
        }
        public void StopAutoSpawn()
        {
            IsSpawning = false;
        }

        protected abstract int GetSpawnCount();

        private void trySpawn(bool useSpawnerTransformValues = true){
           if(SpawningRemaining > 0 && IsSpawning)
           {
               SpawningRemaining--;
               SpawnGameObject(useSpawnerTransformValues);
               StartCoroutine(OffsetSpawn(useSpawnerTransformValues));
           }
        }

        IEnumerator AutoSpawn(bool useSpawnerTransformValues = true) {
           yield return new WaitForSeconds(RandomHelpers.RandomFloatWithinRange(timeFrom, timeTo));
           yield return new WaitUntil(()=>!Paused);
           if(IsSpawning)
           {
               SpawningCount = GetSpawnCount();
               SpawningRemaining = SpawningCount;
               trySpawn(useSpawnerTransformValues);
           }
        }

        IEnumerator OffsetSpawn(bool useSpawnerTransformValues = true) {
           yield return new WaitForSeconds(spawnOffset);
           yield return new WaitUntil(()=>!Paused);
           trySpawn(useSpawnerTransformValues);
        }

    }
}

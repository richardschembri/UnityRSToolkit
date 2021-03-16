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

        public bool IsSpawning { get; private set; } = false; 
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

        IEnumerator AutoSpawn(bool useSpawnerTransformValues = true) {
           yield return new WaitForSeconds(RandomHelpers.RandomFloatWithinRange(timeFrom, timeTo));
           if(IsSpawning)
           {
               int spawnCount = GetSpawnCount();
               // Debug.Log(transform.name + " " + spawnCount);
                if(spawnCount > 0)
                {
                   SpawnGameObject(useSpawnerTransformValues);
                   StartCoroutine(OffsetSpawn(spawnCount - 1, useSpawnerTransformValues));
                }
               StartCoroutine(AutoSpawn(useSpawnerTransformValues));
           }
        }

        IEnumerator OffsetSpawn(int count, bool useSpawnerTransformValues = true) {
           yield return new WaitForSeconds(spawnOffset);
           if(count > 0 && IsSpawning)
           {
               SpawnGameObject(useSpawnerTransformValues);
               StartCoroutine(OffsetSpawn(count - 1, useSpawnerTransformValues));
           }
        }

    }
}

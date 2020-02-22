using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.Controls
{
    public class AutoSpawner : Spawner
    {
        // To implement editor
        public float time_From = 3f;
        public float time_To = 4f;

        public int spawnAmount_Min = 2;
        public int spawnAmount_Max = 3;
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

        IEnumerator AutoSpawn(bool useSpawnerTransformValues = true) {
           yield return new WaitForSeconds(RandomHelpers.RandomFloatWithinRange(time_From, time_To));
           if(IsSpawning)
           {
               int spawnCount = RandomHelpers.RandomIntWithinRange(spawnAmount_Min, spawnAmount_Max + 1);
               Debug.Log(spawnCount);
               SpawnGameObject(useSpawnerTransformValues);
               StartCoroutine(OffsetSpawn(spawnCount - 1, useSpawnerTransformValues));
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

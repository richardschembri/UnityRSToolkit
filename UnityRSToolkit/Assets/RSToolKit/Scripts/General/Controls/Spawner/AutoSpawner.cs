using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.Controls
{
    public class AutoSpawner<T> : AutoSpawnerCore<T> where T : MonoBehaviour 
    {

        public int spawnBatchMin = 2;
        public int spawnBatchMax = 3;

        protected override int GetSpawnCount()
        {
           return RandomHelpers.RandomIntWithinRange(spawnBatchMin, spawnBatchMax + 1);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.Controls
{
    public class AutoSpawner : AutoSpawnerCore 
    {

        public int spawnAmount_Min = 2;
        public int spawnAmount_Max = 3;

        protected override int GetSpawnCount()
        {
           return RandomHelpers.RandomIntWithinRange(spawnAmount_Min, spawnAmount_Max + 1);
        }

    }
}

using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit.Controls
{
    public class AutoSpawnerWeighted : AutoSpawnerCore
    {
        [System.Serializable]
        public struct SpawnAmountWeight
        {
            public int amount;
            public int weight;
        }

        public SpawnAmountWeight[] SpawnAmountWeights;

        protected override int GetSpawnCount()
        {
            if(SpawnAmountWeights.Length <= 0)
            {
                throw new System.Exception("No amounts specified");
            }
            var orderedWeights = SpawnAmountWeights.OrderBy(saw => saw.weight).ToArray();
            var randChoice = Random.Range(0, orderedWeights[orderedWeights.Length - 1].weight);
            for (int i = 0; i < orderedWeights.Length; i++)
            {
                if(randChoice < orderedWeights[i].weight)
                {
                    // Debug.Log(i + " " + randChoice + " " + orderedWeights[i].weight);
                    return orderedWeights[i].amount;
                }
            }
            return 0;
        }
    }
}

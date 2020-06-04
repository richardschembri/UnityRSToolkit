using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space3D
{
    public class GameObjectNavSpawnMarker : NavSpawnMarker
    {
        public GameObject CanSpawnGameObject;
        public GameObject CannotSpawnGameObject;

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            CanSpawnGameObject.SetActive(CanSpawn);
            CannotSpawnGameObject.SetActive(!CanSpawn);
        }
    }
}

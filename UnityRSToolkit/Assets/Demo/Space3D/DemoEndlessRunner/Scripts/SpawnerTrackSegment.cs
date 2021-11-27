using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Controls;

namespace Demo.Space3D.EndlessRunner{
    public class SpawnerTrackSegment : Spawner<TrackSegment>
    {
        private BoxCollider _pathCollider;

        protected override void InitEvents()
        {
            base.InitEvents();
            OnSpawnEvent.AddListener(OnSpawnEvent_Listener);
        }
        private void OnSpawnEvent_Listener(TrackSegment spawn){
            spawn.transform.position = spawn.SpawnPoint.position;
        }
    }
}

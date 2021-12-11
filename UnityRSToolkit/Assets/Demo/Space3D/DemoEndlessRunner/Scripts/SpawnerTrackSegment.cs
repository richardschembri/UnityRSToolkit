using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Controls;

namespace Demo.Space3D.EndlessRunner{
    public class SpawnerTrackSegment : Spawner<TrackSegment>
    {
        private BoxCollider _pathCollider;
        private Vector3 _spawnPosition;

        protected override void InitEvents()
        {
            base.InitEvents();
            OnSpawnEvent.AddListener(OnSpawnEvent_Listener);
        }
        private void OnSpawnEvent_Listener(TrackSegment spawn){
            InitTrackSegment(spawn);
        }
        private void OnTrackSegmentTriggerEnter_Listener(TrackSegment target)
        {
            SpawnAndGetGameObject(true, true);
        }

        private void InitTrackSegment(TrackSegment target){
            target.Init();
            target.transform.position = _spawnPosition;
            _spawnPosition = target.SpawnPoint.position;
            target.InnerColliderTrackSegmentComponent.OnTrackSegmentTriggerEnter.RemoveAllListeners();
            target.InnerColliderTrackSegmentComponent.OnTrackSegmentTriggerEnter.AddListener(OnTrackSegmentTriggerEnter_Listener);
        }

        public override bool Init(bool force = false)
        {
            if(!base.Init(force)){
                return false;
            }
            _spawnPosition = transform.position;
            for(int i = 0; i < SpawnedGameObjects.Count; i++){
                InitTrackSegment(SpawnedGameObjects[i]);
            }
            return true;
        }
    }
}

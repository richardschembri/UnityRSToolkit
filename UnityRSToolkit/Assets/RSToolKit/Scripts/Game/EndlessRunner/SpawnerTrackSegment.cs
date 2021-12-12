using UnityEngine;
using RSToolkit.Controls;
using RSToolkit.Helpers;

namespace RSToolkit.Game.EndlessRunner{
    public class SpawnerTrackSegment : MultiSpawner<TrackSegment>
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

        protected virtual int GetNextSegmentIndex(){
            return RandomHelpers.RandomInt(GameObjectsToSpawn.Length);
        }
        private void OnTrackSegmentTriggerEnter_Listener(TrackSegment target)
        {
            SpawnAndGetGameObject(GetNextSegmentIndex(), true, true);
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

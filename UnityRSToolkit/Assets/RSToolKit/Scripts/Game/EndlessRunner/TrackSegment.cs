using UnityEngine;

namespace RSToolkit.Game.EndlessRunner{
    [DisallowMultipleComponent]
    public class TrackSegment : RSMonoBehaviour
    {
        public InnerColliderTrackSegment InnerColliderTrackSegmentComponent {get; private set;}

        [SerializeField]
        private Transform _spawnPoint;
        public Transform SpawnPoint { get { return _spawnPoint; }}

        protected override void InitComponents()
        {
            base.InitComponents();
            InnerColliderTrackSegmentComponent = GetComponentInChildren<InnerColliderTrackSegment>();
        }

    }
}
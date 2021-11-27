using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit;

namespace Demo.Space3D.EndlessRunner{
    public class InnerColliderTrackSegment : RSMonoBehaviour
    {
        private SpawnerTrackSegment _spawnerTrackSegmentComponent;

        protected override void InitComponents()
        {
            base.InitComponents();
            _spawnerTrackSegmentComponent = GetComponentInParent<SpawnerTrackSegment>();
        }

        void OnTriggerEnter(Collider hit){
            _spawnerTrackSegmentComponent.SpawnGameObject();
        }

    }
}
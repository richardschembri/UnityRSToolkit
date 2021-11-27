using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit;

namespace Demo.Space3D.EndlessRunner{
    [DisallowMultipleComponent]
    public class TrackSegment : RSMonoBehaviour
    {

        private SpawnerTrackSegment _spawnerTrackSegmentComponent;

        [SerializeField]
        private Transform _spawnPoint;
        public Transform SpawnPoint { get { return _spawnPoint; }}

    }
}
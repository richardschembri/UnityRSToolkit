using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.Game.EndlessRunner{
    public class InnerColliderTrackSegment : RSMonoBehaviour
    {
        public TrackSegment ParentTrackSegment { get; private set; }
        public class OnTrackSegmentTriggerEnterEvent : UnityEvent<TrackSegment> { }
        public OnTrackSegmentTriggerEnterEvent OnTrackSegmentTriggerEnter = new OnTrackSegmentTriggerEnterEvent();

        protected override void InitComponents()
        {
            base.InitComponents();
            ParentTrackSegment = GetComponentInParent<TrackSegment>();
        }

        void OnTriggerEnter(Collider hit){
            OnTrackSegmentTriggerEnter.Invoke(ParentTrackSegment);
        }

    }
}
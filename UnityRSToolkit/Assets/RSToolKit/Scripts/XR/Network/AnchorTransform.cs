using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.XR.Network
{
    // Used to compare with
    public class AnchorTransform : MonoBehaviour
    {

        public Transform tetherObject;


        public Vector3 CachedOffset { get; private set; } = Vector3.zero;
        public Vector3 OffsetErrorMargin { get; private set; } = Vector3.zero;

        public Vector3 GetPositionDifference(Vector3 target)
        {
            return transform.position - target;
        }

        public Vector3 GetPositionDifference()
        {
            return transform.position - tetherObject.position;
        }

        public Vector3 CalculateOffset(Vector3 peerTetherPosition, ProximityHelpers.DistanceDirection direction)
        {
            CachedOffset = GetPositionDifference() - GetPositionDifference(peerTetherPosition);
            switch (direction)
            {
                case ProximityHelpers.DistanceDirection.HORIZONTAL:
                    CachedOffset = new Vector3(CachedOffset.x, 0f, CachedOffset.z);
                    break;
                case ProximityHelpers.DistanceDirection.VERTICAL:
                    CachedOffset = new Vector3(0f, CachedOffset.y, 0f);
                    break;
            }
            return CachedOffset;
        }

        public Vector3 ApplyOffsetTo(Vector3 target)
        {
            return target + CachedOffset;
        }
        
    }
}
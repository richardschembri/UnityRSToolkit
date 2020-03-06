using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RSToolkit.Space3D
{
    public class ProximityChecker : MonoBehaviour
    {
        public enum RayDirectionEnum
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            FORWARD,
            BACK
        }
        
        public float RayDistance = 0.5f;
        public RayDirectionEnum RayDirection = RayDirectionEnum.DOWN;
        public LayerMask LayerMask;
        public bool DebugMode;

        public UnityEvent OnProximityEntered { get; private set; } = new UnityEvent();
        bool proximityEnteredTriggered = false;
        private Vector3 GetRayDirectionVector()
        {
            switch (RayDirection)
            {
                case RayDirectionEnum.UP:
                    return Vector3.up;
                case RayDirectionEnum.DOWN:
                    return Vector3.down;
                case RayDirectionEnum.LEFT:
                    return Vector3.left;
                case RayDirectionEnum.RIGHT:
                    return Vector3.right;
                case RayDirectionEnum.FORWARD:
                    return Vector3.forward;
                case RayDirectionEnum.BACK:
                    return Vector3.back;
            }
            return Vector3.zero;
        }

        public float? IsWithinRayDistance()
        {
            RaycastHit hit;
            Color rayColor = Color.green;
            float? hitDistance = null;
            if(Physics.Raycast(transform.position, GetRayDirectionVector(), out hit, RayDistance, LayerMask, QueryTriggerInteraction.Ignore))
            {
                rayColor = Color.red;
                hitDistance = hit.distance;
                if (!proximityEnteredTriggered)
                {
                    proximityEnteredTriggered = true;
                    OnProximityEntered.Invoke();
                }
            }
            else
            {
                proximityEnteredTriggered = false;
            }
            if (DebugMode)
            {
                Debug.DrawLine(transform.position, transform.TransformPoint(GetRayDirectionVector() * RayDistance), rayColor);
            }
            
            return hitDistance;
        }

        // Update is called once per frame
        void Update()
        {
            IsWithinRayDistance();
        }
    }
}

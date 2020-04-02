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
        public bool IsTrigger = true;
        public bool DebugMode;

        public UnityEvent OnProximityEntered { get; private set; } = new UnityEvent();
        bool proximityEnteredTriggered = false;
        Color m_rayColor = Color.green;
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
// To Refactor
        public float? IsWithinRayDistance()
        {
            RaycastHit hit;
            return IsWithinRayDistance(out hit);
        }

        public float? IsWithinRayDistance(out RaycastHit hit)
        {

            float? hitDistance = null;
            if (Physics.Raycast(transform.position, GetRayDirectionVector(), out hit, RayDistance, LayerMask, QueryTriggerInteraction.Ignore))
            {
                m_rayColor = Color.red;
                hitDistance = hit.distance;
                if (IsTrigger && !proximityEnteredTriggered)
                {
                    proximityEnteredTriggered = true;
                    OnProximityEntered.Invoke();
                }
            }
            else
            {
                proximityEnteredTriggered = false;
            }


            return hitDistance;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsTrigger)
            {
                IsWithinRayDistance();
            }
            
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (DebugMode)
            {
                Debug.DrawLine(transform.position, transform.TransformPoint(GetRayDirectionVector() * RayDistance), m_rayColor);
            }
        }
#endif

    }
}

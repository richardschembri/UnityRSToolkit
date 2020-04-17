using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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

        public float MinRayDistance = 0f;
        public float MaxRayDistance = 0.5f;

        public RayDirectionEnum RayDirection = RayDirectionEnum.DOWN;
        public LayerMask LayerMask;

        public bool IsTrigger = true;
        
        public bool DebugMode;

        public UnityEvent OnProximityEntered { get; private set; } = new UnityEvent();
        public UnityEvent OnTouchingEntered { get; private set; } = new UnityEvent();
        bool proximityEnteredTriggered = false;
        bool touchEnteredTriggered = false;
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

        public bool IsAlmostTouching(bool checkForNavMesh = true)
        {
            bool result = IsWithinRayDistance(checkForNavMesh) != null && IsWithinRayDistance(checkForNavMesh) < 0.2f;
            if(result)
            {
                if(IsTrigger && !touchEnteredTriggered)
                {
                    OnTouchingEntered.Invoke();
                    touchEnteredTriggered = true;
                }
            }
            else
            {
                touchEnteredTriggered = false;
            }
            return result;
        }

        public bool IsBeyondRayDistance(float offsetDistance)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, GetRayDirectionVector(), out hit, MaxRayDistance + offsetDistance, LayerMask, QueryTriggerInteraction.Ignore))
            {
                return false;
            }
            return true;
        }

// To Refactor
        public float? IsWithinRayDistance(bool checkForNavMesh = true)
        {
            if (checkForNavMesh)
            {
                NavMeshHit navHit;
                return IsWithinRayDistance(out navHit);
            }
            else
            {
                RaycastHit rayHit;
                return IsWithinRayDistance(out rayHit);
            }
            
        }

        public float? IsWithinRayDistance(out RaycastHit hit)
        {
            float? hitDistance = null;
            if (Physics.Raycast(transform.position, GetRayDirectionVector(), out hit, MaxRayDistance, LayerMask, QueryTriggerInteraction.Ignore))
            {
                if (hit.distance >= MinRayDistance)
                {
                    
                    hitDistance = hit.distance;
                    m_rayColor = Color.red;

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
            }
            else
            {
                proximityEnteredTriggered = false;
            }
            return hitDistance;
        }

        public float? IsWithinRayDistance(out NavMeshHit hit)
        {
            float? hitDistance = null;
            if (NavMesh.SamplePosition(transform.position, out hit, MaxRayDistance, NavMesh.AllAreas))
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
            /*
            if (IsTrigger)
            {
                IsAlmostTouching(); //IsWithinRayDistance();
            }
            */
            
        }


        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            Debug.DrawLine(transform.position, transform.TransformPoint(GetRayDirectionVector() * MaxRayDistance), m_rayColor);
#endif
        }


    }
}

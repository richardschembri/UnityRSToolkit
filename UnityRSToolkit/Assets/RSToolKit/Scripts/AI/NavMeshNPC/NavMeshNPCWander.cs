using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Helpers;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(NavMeshNPC))]
    public class NavMeshNPCWander : MonoBehaviour
    {
        public float wanderRadius = 20f;

        private NavMeshNPC m_navMeshNPCComponent;
        public NavMeshNPC NavMeshNPCComponent
        {
            get
            {
                if (m_navMeshNPCComponent == null)
                {
                    m_navMeshNPCComponent = GetComponent<NavMeshNPC>();
                }
                return m_navMeshNPCComponent;
            }

        }

        public Vector3? LastWanderToPosition { get; private set; } = null;

        public bool IsWanderingToPosition
        {
            get
            {
                return (NavMeshNPCComponent.NavMeshAgentComponent.destination == LastWanderToPosition
                    && NavMeshNPCComponent.NavMeshAgentComponent.speed == NavMeshNPCComponent.walkSpeed
                    && NavMeshNPCComponent.NavMeshAgentComponent.angularSpeed == NavMeshNPCComponent.walkRotationSpeed &&
                    NavMeshNPCComponent.CurrentSpeed > 0);
            }
        }


        public void Wander(float distance)
        {
            NavMeshNPCComponent.UnFocus();
            if (!IsWanderingToPosition)
            {
                LastWanderToPosition = NavMeshHelpers.RandomNavPosInSphere(transform.position, distance);
                NavMeshNPCComponent.WalkTo(LastWanderToPosition.Value);
            }
        }

        public void Wander()
        {
            Wander(wanderRadius);
        }

            public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = new Color(0f, 0f, 0.75f, .075f);

            UnityEditor.Handles.DrawSolidDisc(NavMeshNPCComponent.transform.position, Vector3.up, wanderRadius);
            UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
            UnityEditor.Handles.DrawSolidDisc(NavMeshNPCComponent.NavMeshAgentComponent.destination, Vector3.up, 0.25f);

            UnityEditor.Handles.color = oldColor;
#endif
        }

    }
}
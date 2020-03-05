using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Helpers;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(BotNavMesh))]
    public class BotNavMeshWander : MonoBehaviour
    {
        public float wanderRadius = 20f;

        private BotNavMesh m_botNavMeshComponent;
        public BotNavMesh BotNavMeshComponent
        {
            get
            {
                if (m_botNavMeshComponent == null)
                {
                    m_botNavMeshComponent = GetComponent<BotNavMesh>();
                }
                return m_botNavMeshComponent;
            }

        }

        public Vector3? LastWanderToPosition { get; private set; } = null;

        public bool IsWanderingToPosition
        {
            get
            {
                return (BotNavMeshComponent.NavMeshAgentComponent.destination == LastWanderToPosition
                    && BotNavMeshComponent.NavMeshAgentComponent.speed == BotNavMeshComponent.walkSpeed
                    && BotNavMeshComponent.NavMeshAgentComponent.angularSpeed == BotNavMeshComponent.walkRotationSpeed &&
                    BotNavMeshComponent.CurrentSpeed > 0);
            }
        }


        public void Wander(float distance)
        {
            BotNavMeshComponent.BotComponent.UnFocus();
            if (!IsWanderingToPosition)
            {
                LastWanderToPosition = NavMeshHelpers.RandomNavPosInSphere(transform.position, distance);
                BotNavMeshComponent.WalkTo(LastWanderToPosition.Value);
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

            UnityEditor.Handles.DrawSolidDisc(BotNavMeshComponent.transform.position, Vector3.up, wanderRadius);
            UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
            UnityEditor.Handles.DrawSolidDisc(BotNavMeshComponent.NavMeshAgentComponent.destination, Vector3.up, 0.25f);

            UnityEditor.Handles.color = oldColor;
#endif
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RSToolkit.AI.Helpers
{

    public static class NavMeshHelpers
    {
        // Might use only velovity magnitude instead
        public static float GetCurrentSpeed(this NavMeshAgent self)
        {
            return self.velocity.magnitude / self.speed;
        }

        public static Vector3 RandomNavPosInSphere(Vector3 origin, float radius, float offset = 0f, int areamask = NavMesh.AllAreas)
        {
            var randDirection = Random.insideUnitSphere * radius;
            randDirection += origin;
            NavMeshHit navHit;
            NavMesh.SamplePosition(randDirection, out navHit, radius - offset, areamask);
            return navHit.position;
        }

        public static void DrawGizmoDestination(NavMeshAgent agent)
        {
#if UNITY_EDITOR
            if(agent.GetCurrentSpeed() <= 0)
            {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
            UnityEditor.Handles .DrawWireDisc(agent.destination, Vector3.up, 0.25f);

            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}
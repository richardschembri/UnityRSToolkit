using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RSToolkit.AI.Helpers;

namespace RSToolkit.Space3D
{
    public class NavSpawnMarker : SpawnMarker
    {
        private NavMeshAgent m_parentNavMeshAgentComponent;
        public NavMeshAgent ParentNavMeshAgentComponent
        {
            get
            {
                if (m_parentNavMeshAgentComponent == null)
                {
                    m_parentNavMeshAgentComponent = GetComponentInParent<NavMeshAgent>();
                }
                return m_parentNavMeshAgentComponent;
            }
        }

        Vector3 m_navPosition;
       
        protected override Vector3 GetSurfacePoint()
        {
            CanSpawn = ParentNavMeshAgentComponent.IsAboveNavMeshSurface(ParentColliderComponent, out m_navPosition,out m_rayHit);           
            return m_rayHit.point;
        }

    }
}
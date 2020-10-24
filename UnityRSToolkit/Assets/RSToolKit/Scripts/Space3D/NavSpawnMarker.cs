using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RSToolkit.AI.Helpers;

namespace RSToolkit.Space3D
{
    public class NavSpawnMarker : SpawnMarker
    {
        private NavMeshAgent _parentNavMeshAgentComponent;
        public NavMeshAgent ParentNavMeshAgentComponent
        {
            get
            {
                if (_parentNavMeshAgentComponent == null)
                {
                    _parentNavMeshAgentComponent = GetComponentInParent<NavMeshAgent>();
                }
                return _parentNavMeshAgentComponent;
            }
        }

        Vector3 m_navPosition;
       
        protected override Vector3 GetSurfacePoint()
        {
            if(ParentNavMeshAgentComponent != null)
            {
                CanSpawn = ParentNavMeshAgentComponent.IsAboveNavMeshSurface(ParentColliderComponent, out m_navPosition, out m_rayHit);
            }else if (ParentColliderComponent != null)
            {
                CanSpawn = ParentColliderComponent.IsAboveNavMeshSurface(out m_navPosition, out m_rayHit);
            }
                      
            return m_rayHit.point;
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Helpers;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(BotNavMesh))]
    public class BotWanderNavMesh : BotWander
    {

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

        public override bool CanWander()
        {
            return (BotNavMeshComponent.NavMeshAgentComponent.speed > 0
                && BotNavMeshComponent.NavMeshAgentComponent.angularSpeed > 0); // &&
                //!BotNavMeshComponent.NavMeshAgentComponent.isStopped);
        }

        protected override Vector3 GetNewWanderPosition(float radius)
        {
            return NavMeshHelpers.RandomNavPosInSphere(transform.position, radius);
        }

    }
}
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

        public override bool IsWanderingToPosition()
        {
            return (BotNavMeshComponent.NavMeshAgentComponent.destination == WanderPosition
                && BotNavMeshComponent.NavMeshAgentComponent.speed == BotNavMeshComponent.walkSpeed
                && BotNavMeshComponent.NavMeshAgentComponent.angularSpeed == BotNavMeshComponent.walkRotationSpeed &&
                BotNavMeshComponent.CurrentSpeed > 0);
        }

        protected override Vector3 GetNewWanderPosition(float distance)
        {
            return NavMeshHelpers.RandomNavPosInSphere(transform.position, distance);
        }

        protected override void MoveToWanderPosition()
        {
            BotNavMeshComponent.WalkTo(WanderPosition.Value);
        }

    }
}
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
                && BotNavMeshComponent.NavMeshAgentComponent.angularSpeed > 0 &&
                !BotNavMeshComponent.NavMeshAgentComponent.isStopped);
        }

        public override bool Wander(float radius)
        {
            BotNavMeshComponent.NavMeshAgentComponent.stoppingDistance = BotComponent.SqrPersonalSpaceMagnitude *.75f;
            return base.Wander(radius);
        }
        public override bool StopWandering()
        {
            BotNavMeshComponent.NavMeshAgentComponent.stoppingDistance = 0f;
            return base.StopWandering();
        }

        protected override Vector3 GetNewWanderPosition(float radius)
        {
            return NavMeshHelpers.RandomNavPosInSphere(transform.position, radius);
        }

        protected override void MoveTowardsWanderPosition()
        {
            BotNavMeshComponent.WalkTo(WanderPosition.Value);
        }

        protected override void Awake()
        {   
            base.Awake();
        }

    }
}
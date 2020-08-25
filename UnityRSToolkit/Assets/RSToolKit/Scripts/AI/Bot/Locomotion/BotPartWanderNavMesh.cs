using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Helpers;

namespace RSToolkit.AI.Locomotion
{
    [RequireComponent(typeof(BotLocomotive))]
    public class BotPartWanderNavMesh : BotPartWander
    {

        public override bool CanWander()
        {
            //BotNavMesh
            return BotLocomotiveComponent.CurrentLocomotionType is BotLogicNavMesh 
                    && BotLocomotiveComponent.CurrentLocomotionType.CanMove();
        }

        protected override Vector3? GetNewWanderPosition(Transform wanderCenter, float radius)
        {
            Vector3 result;
            NavMeshHelpers.AttemptRandomNavPosInSphere(transform.position, radius, out result, BotLocomotiveComponent.SqrInteractionMagnitude);
            // NavMeshHelpers.AttemptRandomNavPosInSphere(wanderCenter.position, radius, out result, BotLocomotiveComponent.SqrPersonalSpaceMagnitude);
            return result;
        }

    }
}
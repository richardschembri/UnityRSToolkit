using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Helpers;

namespace RSToolkit.AI.Locomotion
{
    [RequireComponent(typeof(BotLocomotive))]
    public class BotPartWanderNavMesh : BotPartWander
    {

        /// <summary>
        /// Check if Bot is able to wander
        /// </summary>
        public override bool CanWander()
        {
            //BotNavMesh
            return BotLocomotiveComponent.CurrentLocomotionType is BotLogicNavMesh 
                    && BotLocomotiveComponent.CurrentLocomotionType.CanMove();
        }

        /// <summary>
        /// Get a valid random position within the wander radius of the provided wanderCenter
        /// </summary>
        protected override Vector3? GetNewWanderPosition(Transform wanderCenter, float radius)
        {
            return NavMeshHelpers.RandomNavPosInSphere(wanderCenter.position, radius, BotLocomotiveComponent.SqrInteractionMagnitude);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Space3D;
using RSToolkit.Helpers;
using UnityEngine.AI;
using RSToolkit.AI.FSM;

namespace RSToolkit.AI.Locomotion
{

    public class BotLogicFlight : BotLogicLocomotion
    {

        public Flying3DObject Flying3DObjectComponent { get; private set; }

        public override float CurrentSpeed
        {
            get
            {
                return Flying3DObjectComponent.CurrentSpeed;
            }
        }

        public override void RotateTowardsPosition()
        {
            var rotation = Quaternion.LookRotation(BotLocomotiveComponent.FocusedOnPosition.Value - BotLocomotiveComponent.transform.position, Vector3.up);
            Flying3DObjectComponent.YawTo(rotation.eulerAngles.y);
        }

        public override void RotateAwayFromPosition(){
            var rotation = Quaternion.LookRotation(BotLocomotiveComponent.GetMoveAwayDestination() - BotLocomotiveComponent.transform.position, Vector3.up);
            Flying3DObjectComponent.YawTo(rotation.eulerAngles.y);
        }

        public override bool MoveTowardsPosition(bool fullspeed = true)
        {

            RotateTowardsPosition();

            // Match height of position
            if (BotLocomotiveComponent.FocusedOnPosition.Value.y - BotLocomotiveComponent.transform.position.y >= (BotLocomotiveComponent.SqrAtPositionErrorMargin / 2 ) )
            {
                Flying3DObjectComponent.ApplyVerticalThrust(true);
            }

            if (BotLocomotiveComponent.FocusedOnPosition.Value.y - BotLocomotiveComponent.transform.position.y < -(BotLocomotiveComponent.SqrAtPositionErrorMargin / 2) )
            {
                Flying3DObjectComponent.ApplyVerticalThrust(false);
            }

            if (BotLocomotiveComponent.IsWithinDistance(Bot.DistanceType.AT_POSITION, ProximityHelpers.DistanceDirection.ALL))
            {
                // At position, stop all forces
                Flying3DObjectComponent.ResetAppliedForces();
                Flying3DObjectComponent.RigidBodyComponent.ResetInertiaTensor();
                // Flying3DObjectComponent.ResetAppliedAxis();
                return false;
            }
            else if (BotLocomotiveComponent.IsWithinDistance(Bot.DistanceType.PERSONAL_SPACE, ProximityHelpers.DistanceDirection.ALL, 0.85f)) // .IsWithinPersonalSpace(0.85f))
            {
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? 0.1f : 0.05f); //-0.35f : 0.05f);
            }
            else if (BotLocomotiveComponent.IsWithinDistance(Bot.DistanceType.PERSONAL_SPACE, ProximityHelpers.DistanceDirection.HORIZONTAL, 1.1f))// IsWithinPersonalSpace(1.1f))
            {                
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? 0.25f : 0.05f);                
            }
            else if (BotLocomotiveComponent.IsWithinDistance(Bot.DistanceType.INTERACTION, ProximityHelpers.DistanceDirection.HORIZONTAL)) // IsWithinInteractionDistance())
            {
                // Slow down
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? 0.5f : 0.15f);
            }
            // Move towards position
            else // if (!BotLocomotiveComponent.IsWithinInteractionDistance())
            {
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? 1f : 0.2f);
            }                      
                    
            return true;
        }
            
        /// <summary>
        /// Move away from FocusedOnPosition
        /// </summary>
        public override void MoveAway(bool fullspeed = true){
            RotateAwayFromPosition();
            if(!BotLocomotiveComponent.IsAway()){
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? 1f : 0.2f);
            }
        }

        public override void OnStateChange(BotLocomotive.FStatesLocomotion locomotionState)
        {
        }

        public BotLogicFlight(BotLocomotive botLocomotion, Flying3DObject flying3DObjectComponent) :base(botLocomotion)
        {
            Flying3DObjectComponent = flying3DObjectComponent;
        }

        public BotLogicFlight(BotFlyable botFlyable) : base(botFlyable)
        {
            Flying3DObjectComponent = botFlyable.Flying3DObjectComponent;
            botFlyable.BotLogicFlyingRef = this;
        }
    }
}

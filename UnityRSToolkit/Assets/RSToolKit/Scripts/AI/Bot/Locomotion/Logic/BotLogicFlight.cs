using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Space3D;
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

        public override void MoveTowardsPosition(bool fullspeed = true)
        {

            RotateTowardsPosition();

            if (Mathf.RoundToInt(BotLocomotiveComponent.FocusedOnPosition.Value.y) > Mathf.RoundToInt(BotLocomotiveComponent.transform.position.y))
            {
                Flying3DObjectComponent.ApplyVerticalThrust(true);
            }
            if (Mathf.RoundToInt(BotLocomotiveComponent.FocusedOnPosition.Value.y) < Mathf.RoundToInt(BotLocomotiveComponent.transform.position.y))
            {
                Flying3DObjectComponent.ApplyVerticalThrust(false);
            }
            if (!BotLocomotiveComponent.IsWithinInteractionDistance())
            {
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? 1f : 0.2f);
            }
            else if (!BotLocomotiveComponent.IsWithinPersonalSpace())
            {
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? -0.5f : 0.1f);
            }
        }

        public override void MoveAway(bool fullspeed = true){
            RotateAwayFromPosition();
            if(!BotLocomotiveComponent.IsAway()){
                Flying3DObjectComponent.ApplyForwardThrust(fullspeed ? 1f : 0.2f);
            }
        }

        public override void OnStateChange(BotLocomotive.LocomotionState locomotionState)
        {
            
        }


        public BotLogicFlight(BotLocomotive botLocomotion, Flying3DObject flying3DObjectComponent) :base(botLocomotion)
        {
            Flying3DObjectComponent = flying3DObjectComponent;
        }

    }
}
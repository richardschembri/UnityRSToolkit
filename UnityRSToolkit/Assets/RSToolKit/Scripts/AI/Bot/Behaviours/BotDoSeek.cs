using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Locomotion;

namespace RSToolkit.AI.Behaviour.Task{

using StopMovementConditions = BotLocomotive.StopMovementConditions;

	public class BotDoSeek : BehaviourAction
	{
        private const string NODE_NAME = "DoSeek";
		public bool StopAtPersonalOrInteract {get; set;}
		public BotLocomotive BotLocomotiveComponent{get; private set;}

		public BotDoSeek(BotLocomotive botLocomotiveComponent,
								bool stopAtPersonalOrInteract = true,
								string name = NODE_NAME) : base( name){
			BotLocomotiveComponent = botLocomotiveComponent;
			StopAtPersonalOrInteract= stopAtPersonalOrInteract;
			_multiFrameFunc = DoSeek;
			OnStarted.AddListener(DoSeekOnStarted_Listener);
		}

        protected virtual void DoSeekOnStarted_Listener(){
            BotLocomotiveComponent.MoveToTarget(StopAtPersonalOrInteract ?
													StopMovementConditions.WITHIN_PERSONAL_SPACE :
													StopMovementConditions.WITHIN_INTERACTION_DISTANCE);
        }

        protected virtual BehaviourAction.ActionResult DoSeek(bool cancel){
            if(cancel || !BotLocomotiveComponent.IsFocused){
                BotLocomotiveComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }

            if((StopAtPersonalOrInteract && BotLocomotiveComponent.IsWithinPersonalSpace())
				|| (!StopAtPersonalOrInteract && BotLocomotiveComponent.IsWithinInteractionDistance())){
                if(BotLocomotiveComponent.IsFacing()){
                    return BehaviourAction.ActionResult.SUCCESS;
                }
                BotLocomotiveComponent.RotateTowardsPosition();
            }

            return BehaviourAction.ActionResult.PROGRESS;
        }
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Locomotion;

namespace RSToolkit.AI.Behaviour.Task{

// using StopMovementConditions = BotLocomotive.StopMovementConditions;

	public class BotDoSeek : BehaviourAction
	{
        private const string NODE_NAME = "DoSeek";
		// public StopMovementConditions StopMovementCondition {get; set;}
		public Bot.DistanceType StopMovementCondition {get; set;}
		public BotLocomotive BotLocomotiveComponent{get; private set;}

		public BotDoSeek(BotLocomotive botLocomotiveComponent,
								Bot.DistanceType stopMovementCondition = Bot.DistanceType.INTERACTION,
								// StopMovementConditions stopMovementCondition = StopMovementConditions.WITHIN_INTERACTION_DISTANCE ,
								string name = NODE_NAME) : base( name){
			BotLocomotiveComponent = botLocomotiveComponent;
			StopMovementCondition= stopMovementCondition;
			_multiFrameFunc = DoSeek;
			OnStarted.AddListener(DoSeekOnStarted_Listener);
		}

        protected virtual void DoSeekOnStarted_Listener(){
            BotLocomotiveComponent.MoveToTarget(StopMovementCondition);
        }

		public bool ArrivedAtDestination(){
			return BotLocomotiveComponent.IsWithinDistance(StopMovementCondition);
			/*
			switch(StopMovementCondition){
				case StopMovementConditions.WITHIN_INTERACTION_DISTANCE:
					return BotLocomotiveComponent.IsWithinInteractionDistance();
				case StopMovementConditions.WITHIN_PERSONAL_SPACE:
					return BotLocomotiveComponent.IsWithinPersonalSpace();
				case StopMovementConditions.AT_POSITION:
					return BotLocomotiveComponent.IsAtPosition();
			}
			return false;
			*/
		}

        protected virtual BehaviourAction.ActionResult DoSeek(bool cancel){
            if(cancel || !BotLocomotiveComponent.IsFocused){
                BotLocomotiveComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }

            if(ArrivedAtDestination()){
                if(BotLocomotiveComponent.IsFacing()){
                    return BehaviourAction.ActionResult.SUCCESS;
                }
                BotLocomotiveComponent.RotateTowardsPosition();
            }

            return BehaviourAction.ActionResult.PROGRESS;
        }
	}
}

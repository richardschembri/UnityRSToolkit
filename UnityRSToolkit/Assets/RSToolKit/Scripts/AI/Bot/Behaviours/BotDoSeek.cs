using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Locomotion;

namespace RSToolkit.AI.Behaviour.Task{

	public class BotDoSeek : BehaviourAction
	{
        private const string NODE_NAME = "DoSeek";
		public Bot.DistanceType StopMovementCondition {get; set;}
		public BotLocomotive BotLocomotiveComponent{get; private set;}

		public BotDoSeek(BotLocomotive botLocomotiveComponent,
								Bot.DistanceType stopMovementCondition = Bot.DistanceType.INTERACTION,
								string name = NODE_NAME) : base( name){
			BotLocomotiveComponent = botLocomotiveComponent;
			StopMovementCondition= stopMovementCondition;
			_multiFrameFunc = DoSeek;
			OnStarted.AddListener(DoSeekOnStarted_Listener);
		}

        protected virtual void DoSeekOnStarted_Listener(){
            BotLocomotiveComponent.MoveToTarget(StopMovementCondition);
        }

        protected virtual BehaviourAction.ActionResult DoSeek(bool cancel){
            if(cancel || !BotLocomotiveComponent.IsFocused){
                BotLocomotiveComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }

            if(BotLocomotiveComponent.IsWithinDistance(StopMovementCondition)){
                if(BotLocomotiveComponent.IsFacing()){
                    return BehaviourAction.ActionResult.SUCCESS;
                }
                BotLocomotiveComponent.RotateTowardsPosition();
            }

            return BehaviourAction.ActionResult.PROGRESS;
        }
	}
}

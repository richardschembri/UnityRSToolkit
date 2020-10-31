using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RSToolkit.Helpers;
using RSToolkit.AI.Locomotion;

namespace RSToolkit.AI.Behaviour.Task{

	public class BotDoPatrol : BehaviourAction
	{
        private const string NODE_NAME = "DoPatrol";
#region Components
		public BotLocomotive BotLocomotiveComponent{get; private set;}
		public BotPartVision _botVisionComponent;
#endregion Components

        private int _waypointIndex = 0;
        private Transform[] _waypoints = null;

		public string TargetTag {get; set;}
		public Transform[] TargetTransforms {get; private set;}

		public BotDoPatrol(BotLocomotive botLocomotiveComponent, string targetTag,
								string name = NODE_NAME) : base(name){
			BotLocomotiveComponent = botLocomotiveComponent;
			_botVisionComponent = BotLocomotiveComponent.GetComponent<BotPartVision>();
			TargetTag = targetTag;
		}

        private void DoPatrol_OnStarted(){
			TargetTransforms = null;
            _waypointIndex = Array.IndexOf(_waypoints, BotLocomotiveComponent.transform.GetClosestTransform(_waypoints));
            MoveToWaypoint();
        }

        private BehaviourAction.ActionResult DoPatrol(bool cancel)
        {
            if(cancel){
                return BehaviourAction.ActionResult.FAILED;
            }
            if(IsTargetWithinSight()){
				TargetTransforms = _botVisionComponent.DoLookoutFor(false, false, TargetTag);
                return BehaviourAction.ActionResult.SUCCESS;
            }

            if(BotLocomotiveComponent.HasReachedDestination()){
                MoveToNextWaypoint();
            }

            return BehaviourAction.ActionResult.PROGRESS;
        }

        protected virtual bool IsTargetWithinSight(){
            return _botVisionComponent.IsWithinSight(TargetTag);
        }

        private void MoveToWaypoint(){
            BotLocomotiveComponent.FocusOnTransform(_waypoints[_waypointIndex]);
            // BotLocomotiveComponent.MoveToTarget(BotLocomotive.StopMovementConditions.WITHIN_PERSONAL_SPACE, false);
            BotLocomotiveComponent.MoveToTarget(Bot.DistanceType.PERSONAL_SPACE, false);
        }

        private void MoveToNextWaypoint(){
            _waypointIndex = CollectionHelpers.GetNextCircularIndex(_waypointIndex, _waypoints.Length);
            MoveToWaypoint();
        }
	}
}

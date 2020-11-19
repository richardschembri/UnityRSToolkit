using UnityEngine;
using RSToolkit.AI;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI.Locomotion;
using RSToolkit.Helpers;
using System;

namespace Demo.BehaviourTree.CTF{
    public class CTFDefenceBot : CTFBot
    {
        [SerializeField]
        private Transform[] _waypoints = null;
        private int m_waypointIndex = 0;

        public CTFBot TargetEnemyBot {get; private set;}


#region Behaviour Structs

        public struct DefendFlagNotTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourSelector MainSelector;
            public BehaviourAction DoSeekStartingPosition ;
            public BotIsWithinSight IsEnemyWithinSight;
            public BehaviourAction DoDefend;
        }

        public struct PatrolFlagNotTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourSequence PatrolSequence;
            public BotDoPatrol DoPatrol;
            public BotDoSeek DoSeekEnemy;
        }

        public struct DefendFlagTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourCondition IsFlagNotCaptured;
            public BotDoSeek DoSeekEnemy;
        }

#endregion Behaviour Structs

#region Init Behaviours
        public DefendFlagNotTakenBehaviours CTFDefend_FlagNotTakenBehaviours; 
        public DefendFlagTakenBehaviours CTFDefend_FlagTakenBehaviours; 
        public PatrolFlagNotTakenBehaviours CTFPatrol_FlagNotTakenBehaviours; 

        protected override void InitFlagNotTakenBehaviours(){
            CTFDefend_FlagNotTakenBehaviours.Root = GenerateRoot(false);
            CTFDefend_FlagNotTakenBehaviours.MainSelector = new BehaviourSelector(false);

            CTFDefend_FlagNotTakenBehaviours.DoDefend = new BehaviourAction(DoDefend, "Do Defend");
            CTFDefend_FlagNotTakenBehaviours.DoDefend.OnStarted.AddListener(DoDefendOnStarted_Listener);
            CTFDefend_FlagNotTakenBehaviours.IsEnemyWithinSight = new BotIsWithinSight(_botVisionComponent, CTFGameManager.TAG_OFFENCE, CTFDefend_FlagNotTakenBehaviours.DoDefend);
            CTFDefend_FlagNotTakenBehaviours.MainSelector.AddChild(CTFDefend_FlagNotTakenBehaviours.IsEnemyWithinSight);

            CTFDefend_FlagNotTakenBehaviours.DoSeekStartingPosition = new BotDoSeek (_botGroundComponent, Bot.DistanceType.AT_POSITION); // // BotLocomotive.StopMovementConditions.AT_POSITION );
        }

        protected override void InitFlagTakenBehaviours(){
            if(_waypoints.Length > 0){
                InitPatrolFlagTakenBehaviours();
            }else{
                InitDefendFlagTakenBehaviours();
            }
        }
        protected void InitDefendFlagTakenBehaviours(){
            // CTFDefend_FlagTakenBehaviours.DoSeekEnemy = new BotDoSeek(_botLocomotiveComponent, BotLocomotive.StopMovementConditions.AT_POSITION, "Do Seek Enemy");
            CTFDefend_FlagTakenBehaviours.DoSeekEnemy = new BotDoSeek(_botGroundComponent, Bot.DistanceType.AT_POSITION, "Do Seek Enemy");
            CTFDefend_FlagTakenBehaviours.IsFlagNotCaptured = new BehaviourCondition(IsFlagNotCapturedCondition, CTFDefend_FlagTakenBehaviours.DoSeekEnemy);
        }

        protected void InitPatrolFlagTakenBehaviours(){
            CTFPatrol_FlagNotTakenBehaviours.PatrolSequence = new BehaviourSequence(false);
            CTFPatrol_FlagNotTakenBehaviours.DoPatrol = new BotDoPatrol(_botGroundComponent, CTFGameManager.TAG_OFFENCE);
			CTFPatrol_FlagNotTakenBehaviours.DoPatrol.OnStopped.AddListener(DoPatrol_OnStopped);
            // CTFPatrol_FlagNotTakenBehaviours.DoSeekEnemy = new BotDoSeek(_botLocomotiveComponent, BotLocomotive.StopMovementConditions.AT_POSITION, "Do Seek Action");
            CTFPatrol_FlagNotTakenBehaviours.DoSeekEnemy = new BotDoSeek(_botGroundComponent, Bot.DistanceType.AT_POSITION, "Do Seek Action");
        }
#endregion Init Behaviours

        protected override BehaviourRootNode GetDefaultTree(){
            if(_waypoints.Length > 0){
                return CTFPatrol_FlagNotTakenBehaviours.Root;
            }else{
                return CTFDefend_FlagNotTakenBehaviours.Root;
            }
        }
        public override void SwitchToTree_FlagTaken(){
            _behaviourManagerComponent.SetCurrentTree(CTFDefend_FlagTakenBehaviours.Root, true);
        }

        public override void SwitchToTree_FlagNotTaken(){
            if(_waypoints.Length > 0){
                _behaviourManagerComponent.SetCurrentTree(CTFPatrol_FlagNotTakenBehaviours.Root, true);
            }else{
                _behaviourManagerComponent.SetCurrentTree(CTFDefend_FlagNotTakenBehaviours.Root, true);
            }
        }

#region Behaviour Logic
        protected bool IsFlagNotCapturedCondition(){
            return false;
        }
        
#region DoDefend
        private void DoDefendOnStarted_Listener(){
            SeekBot();
        }
        private BehaviourAction.ActionResult DoDefend(bool cancel)
        {
            if(cancel || !IsFocused() ||!IsWithinDefenceRadius()){
                _botGroundComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }

            if(_botGroundComponent.IsWithinDistance(Bot.DistanceType.INTERACTION)){// IsWithinInteractionDistance()){
                return BehaviourAction.ActionResult.SUCCESS;
            }

            return BehaviourAction.ActionResult.PROGRESS;
        }
#endregion DoDefend
#region DoPatrol
		private void  DoPatrol_OnStopped(bool success){
			TargetEnemyBot = CTFPatrol_FlagNotTakenBehaviours.DoPatrol.TargetTransforms[0].GetComponent<CTFBot>(); 
		}
#endregion DoPatrol

        private void MoveToWaypoint(){
            _botGroundComponent.FocusOnTransform(_waypoints[m_waypointIndex]);
            // _botLocomotiveComponent.MoveToTarget(BotLocomotive.StopMovementConditions.WITHIN_PERSONAL_SPACE, false);
            _botGroundComponent.MoveToTarget(BotLocomotive.DistanceType.PERSONAL_SPACE, false);
        }


        private void MoveToNextWaypoint(){
            m_waypointIndex = CollectionHelpers.GetNextCircularIndex(m_waypointIndex, _waypoints.Length);
            MoveToWaypoint();
        }

#region Behaviour Conditions
        public bool IsWithinDefenceRadius(){
            if(_botGroundComponent.FocusedOnPosition == null){
                return false;
            }
            var thisPosition = transform.position;
            thisPosition.y = _botGroundComponent.FocusedOnPosition.Value.y;
            return Vector3.SqrMagnitude(thisPosition - _botGroundComponent.FocusedOnPosition.Value) <= _botGroundComponent.SqrAwarenessMagnitude;
        }

        public bool IsFocused(){
            return _botGroundComponent.IsFocused && TargetEnemyBot != null;
        }
#endregion Behaviour Conditions

#endregion Behaviour Logic

        private void SeekBot(){
            TargetEnemyBot = _botVisionComponent.DoLookoutFor(false, false, CTFGameManager.TAG_OFFENCE)[0].GetComponent<CTFBot>();
            TargetEnemyBot.OnDie.AddListener(OnTargetDie_Listener); 
            // _botLocomotiveComponent.MoveToTarget(BotLocomotive.StopMovementConditions.WITHIN_PERSONAL_SPACE);
            _botGroundComponent.MoveToTarget(BotLocomotive.DistanceType.PERSONAL_SPACE);
        }

        #region Events
        public void OnTargetDie_Listener(CTFBot target){
           target.OnDie.RemoveListener(OnTargetDie_Listener);
           if(target == TargetEnemyBot){
               TargetEnemyBot = null;
               _botGroundComponent.UnFocus();
           }
        }

        #endregion Events

        #region Mono Functions
        #endregion Mono Functions
    }
}

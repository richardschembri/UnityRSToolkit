﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI;
using RSToolkit.AI.Locomotion;

namespace Demo.BehaviourTree.CTF{
    public class CTFOffenceBot : CTFBot 
    {

        #region Behaviour Structs

        public struct OffenceFlagNotTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourSelector MainSelector;
            public BotIsWithinSight IsEnemyNotWithinDistance;
            public BehaviourAction DoSeekFlag;
            public BehaviourAction DoFlee;
        }

        public struct OffenceFlagTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourSelector MainSelector;
            public BehaviourCondition HasFlag;            
            public BehaviourSequence TakeFlagToCapturePointSequence;
            public BehaviourAction DoSeekCapturePoint;
            public BehaviourAction DoTakeCelebrate;
            public BehaviourConditionInverse IsFlagNotCaptured;            
            public BehaviourSequence SeekFlagSequence;
            public BehaviourAction DoSeekFlag;
            public BehaviourAction DoSeekCelebrate;

        }

        #endregion Behaviour Structs

        [SerializeField]
        private float m_celebrateRotationSpeed = 7f;

        public OffenceFlagNotTakenBehaviours CTFOffence_FlagNotTakenBehaviours; 
        public OffenceFlagTakenBehaviours CTFOffence_FlagTakenBehaviours; 

        public override void SwitchToTree_FlagTaken(){
            _behaviourManagerComponent.SetCurrentTree(CTFOffence_FlagTakenBehaviours.Root, true);
        }

        public override void SwitchToTree_FlagNotTaken(){
            _behaviourManagerComponent.SetCurrentTree(CTFOffence_FlagNotTakenBehaviours.Root, true);
        }

        protected override void InitFlagNotTakenBehaviours(){
            CTFOffence_FlagNotTakenBehaviours.Root = GenerateRoot(false); // new BehaviourRootNode("Flag Not Taken");
            CTFOffence_FlagNotTakenBehaviours.MainSelector = new BehaviourSelector(false);
            CTFOffence_FlagNotTakenBehaviours.Root.AddChild(CTFOffence_FlagNotTakenBehaviours.MainSelector);

            CTFOffence_FlagNotTakenBehaviours.DoSeekFlag = new BotDoSeek(_botGroundComponent,
																			BotLocomotive.DistanceType.AT_POSITION,
																			"Do Seek Flag");
            CTFOffence_FlagNotTakenBehaviours.DoSeekFlag.OnStarted.AddListener(DoSeekFlagOnStarted_Listener);
            CTFOffence_FlagNotTakenBehaviours.IsEnemyNotWithinDistance = new BotIsWithinSight(_botVisionComponent, CTFGameManager.TAG_DEFENCE, CTFOffence_FlagNotTakenBehaviours.DoSeekFlag, true);

            CTFOffence_FlagNotTakenBehaviours.MainSelector.AddChild(CTFOffence_FlagNotTakenBehaviours.IsEnemyNotWithinDistance);

            CTFOffence_FlagNotTakenBehaviours.DoFlee = new BehaviourAction(DoFlee, "Do Flee");
            CTFOffence_FlagNotTakenBehaviours.DoFlee.OnStarted.AddListener(DoFleeOnStarted_Listener);
            CTFOffence_FlagNotTakenBehaviours.MainSelector.AddChild(CTFOffence_FlagNotTakenBehaviours.DoFlee);
        }

        protected override void InitFlagTakenBehaviours(){
            CTFOffence_FlagTakenBehaviours.Root = GenerateRoot(true);
            CTFOffence_FlagTakenBehaviours.MainSelector = new BehaviourSelector(false);
            CTFOffence_FlagTakenBehaviours.MainSelector.Name = "Main Selector";
            CTFOffence_FlagTakenBehaviours.TakeFlagToCapturePointSequence = new BehaviourSequence(false);
            CTFOffence_FlagTakenBehaviours.TakeFlagToCapturePointSequence.Name = "Take Flag To Capture Point Sequence";

            CTFOffence_FlagTakenBehaviours.HasFlag = new BehaviourCondition(HasFlag, CTFOffence_FlagTakenBehaviours.TakeFlagToCapturePointSequence);
            CTFOffence_FlagTakenBehaviours.HasFlag.Name = "Has Flag";
            CTFOffence_FlagTakenBehaviours.MainSelector.AddChild(CTFOffence_FlagTakenBehaviours.HasFlag);

            // CTFOffence_FlagTakenBehaviours.DoSeekCapturePoint = new BotDoSeek(_botLocomotiveComponent, BotLocomotive.StopMovementConditions.AT_POSITION, "Do Seek Capture Point");//new BehaviourAction(DoSeek, "Do Seek Capture Point");
            CTFOffence_FlagTakenBehaviours.DoSeekCapturePoint = new BotDoSeek(_botGroundComponent, Bot.DistanceType.AT_POSITION, "Do Seek Capture Point");
            CTFOffence_FlagTakenBehaviours.TakeFlagToCapturePointSequence.AddChild(CTFOffence_FlagTakenBehaviours.DoSeekCapturePoint);
            CTFOffence_FlagTakenBehaviours.DoTakeCelebrate = new BehaviourAction(DoCelebrate, "Do Celebrate");
            CTFOffence_FlagTakenBehaviours.TakeFlagToCapturePointSequence.AddChild(CTFOffence_FlagTakenBehaviours.DoTakeCelebrate);

            CTFOffence_FlagTakenBehaviours.SeekFlagSequence = new BehaviourSequence(false);
            CTFOffence_FlagTakenBehaviours.IsFlagNotCaptured = new BehaviourConditionInverse(CTFGameManager.Instance.IsFlagTaken, CTFOffence_FlagTakenBehaviours.SeekFlagSequence);
            // CTFOffence_FlagTakenBehaviours.DoSeekFlag = new BotDoSeek(_botLocomotiveComponent,BotLocomotive.StopMovementConditions.AT_POSITION, "Do Seek Flag");
            CTFOffence_FlagTakenBehaviours.DoSeekFlag = new BotDoSeek(_botGroundComponent,Bot.DistanceType.AT_POSITION, "Do Seek Flag");
            CTFOffence_FlagTakenBehaviours.SeekFlagSequence.AddChild(CTFOffence_FlagTakenBehaviours.DoSeekFlag);
            // CTFOffence_FlagTakenBehaviours.DoSeekCelebrate = new BotDoSeek(_botLocomotiveComponent,BotLocomotive.StopMovementConditions.AT_POSITION, "Do Seek Celebrate");
            CTFOffence_FlagTakenBehaviours.DoSeekCelebrate = new BotDoSeek(_botGroundComponent,Bot.DistanceType.AT_POSITION, "Do Seek Celebrate");
            CTFOffence_FlagTakenBehaviours.SeekFlagSequence.AddChild(CTFOffence_FlagTakenBehaviours.DoSeekCelebrate);
        }

        protected override BehaviourRootNode GetDefaultTree(){
            return CTFOffence_FlagNotTakenBehaviours.Root;
        }

        #region Behaviour Logic

        #region DoFlee

        private void DoFleeOnStarted_Listener(){
            _botGroundComponent.MoveAwayFromTarget();
        }

        protected BehaviourAction.ActionResult DoFlee (bool cancel){
            if(cancel || !_botGroundComponent.IsFocused || _botGroundComponent.CurrentFState == BotLocomotive.FStatesLocomotion.CannotMove){
                _botGroundComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }
            if(!_botGroundComponent.IsWithinDistance(Bot.DistanceType.AWARENESS)){ // IsWithinAwarenessDistance()){
                return BehaviourAction.ActionResult.SUCCESS;
            }else if(!_botGroundComponent.IsMoving()){
                _botGroundComponent.MoveAwayFromTarget();
            }
            return BehaviourAction.ActionResult.PROGRESS;
        }

        #endregion DoFlee
        #region DoSeekFlag
        private void DoSeekFlagOnStarted_Listener(){
           _botGroundComponent.FocusOnTransform(CTFGameManager.Instance.Flag);
           // _botLocomotiveComponent.MoveToTarget(BotLocomotive.StopMovementConditions.WITHIN_PERSONAL_SPACE, true);
           _botGroundComponent.MoveToTarget(Bot.DistanceType.PERSONAL_SPACE, true);
        }
        #endregion DoSeekFlag

        protected BehaviourAction.ActionResult DoCelebrate(bool cancel){
            transform.Rotate(transform.up, m_celebrateRotationSpeed);
            return BehaviourAction.ActionResult.PROGRESS;
        }
        #endregion Behaviour Logic

        #region Mono Functions
        #endregion Mono Functions
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI;
using RSToolkit.Helpers;
using System;

namespace Demo.CTF{
    public class CTFDefenceBot : CTFBot
    {
        [SerializeField]
        private Transform[] m_waypoints;
        private int m_waypointIndex = 0;

        public CTFBot TargetEnemyBot {get; private set;}

        [SerializeField]
        private float m_defendRadius;

        private float m_sqrDefendRadius{
            get{
                return m_defendRadius * m_defendRadius;
            }
        }

        public bool IsWithinDefenceRadius(){
            if(m_botComponent.FocusedOnPosition == null){
                return false;
            }
            var thisPosition = transform.position;
            thisPosition.y = m_botComponent.FocusedOnPosition.Value.y;
            return Vector3.SqrMagnitude(thisPosition - m_botComponent.FocusedOnPosition.Value) <= m_sqrDefendRadius;
        }

        public bool IsFocused(){
            return m_botComponent.IsFocused && TargetEnemyBot != null;
        }

#region Behaviour Structs

        public struct DefendFlagNotTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourSelector MainSelector;
            public BehaviourAction DoSeekStartingPosition ;
            public BehaviourCondition IsWithinSight;
            public BehaviourAction DoDefend;
        }

        public struct PatrolFlagNotTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourSequence PatrolSequence;
            public BehaviourAction DoPatrol;
            public BehaviourAction DoSeekEnemy;
        }

        public struct DefendFlagTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourCondition IsFlagNotCaptured;
            public BehaviourAction DoSeekEnemy;
        }

#endregion Behaviour Structs

#region Init Behaviours
        public DefendFlagNotTakenBehaviours CTFDefend_FlagNotTakenBehaviours; 
        public DefendFlagTakenBehaviours CTFDefend_FlagTakenBehaviours; 
        public PatrolFlagNotTakenBehaviours CTFPatrol_FlagNotTakenBehaviours; 

        protected override void InitFlagNotTakenBehaviours(){
            CTFDefend_FlagNotTakenBehaviours.Root = new BehaviourRootNode("Flag Not Taken");
            CTFDefend_FlagNotTakenBehaviours.MainSelector = new BehaviourSelector(false);

            CTFDefend_FlagNotTakenBehaviours.DoDefend = new BehaviourAction(DoDefend, "Do Defend");
            CTFDefend_FlagNotTakenBehaviours.DoDefend.OnStarted.AddListener(DoDefendOnStarted_Listener);
            CTFDefend_FlagNotTakenBehaviours.IsWithinSight = new BehaviourCondition(IsWithinSight, CTFDefend_FlagNotTakenBehaviours.DoDefend);
            CTFDefend_FlagNotTakenBehaviours.MainSelector.AddChild(CTFDefend_FlagNotTakenBehaviours.IsWithinSight);

            CTFDefend_FlagNotTakenBehaviours.DoSeekStartingPosition = new BehaviourAction(DoSeek, "Do Seek");
        }

        protected override void InitFlagTakenBehaviours(){
            if(m_waypoints.Length > 0){
                InitPatrolFlagTakenBehaviours();
            }else{
                InitDefendFlagTakenBehaviours();
            }
        }
        protected void InitDefendFlagTakenBehaviours(){
            CTFDefend_FlagTakenBehaviours.DoSeekEnemy = new BehaviourAction(DoSeek, "Do Seek Enemy");
            CTFDefend_FlagTakenBehaviours.IsFlagNotCaptured = new BehaviourCondition(IsFlagNotCapturedCondition, CTFDefend_FlagTakenBehaviours.DoSeekEnemy);
        }

        protected void InitPatrolFlagTakenBehaviours(){
            CTFPatrol_FlagNotTakenBehaviours.PatrolSequence = new BehaviourSequence(false);
            CTFPatrol_FlagNotTakenBehaviours.DoPatrol = new BehaviourAction(DoPatrol, "Do Patrol Action");
            CTFPatrol_FlagNotTakenBehaviours.DoPatrol.OnStarted.AddListener(DoPatrol_OnStarted);
            CTFPatrol_FlagNotTakenBehaviours.DoSeekEnemy = new BehaviourAction(DoSeek, "Do Seek Action");
        }
#endregion Init Behaviours

        protected override BehaviourRootNode GetDefaultTree(){
            if(m_waypoints.Length > 0){
                return CTFPatrol_FlagNotTakenBehaviours.Root;
            }else{
                return CTFDefend_FlagNotTakenBehaviours.Root;
            }
        }
        public override void SwitchToTree_FlagTaken(){
            m_behaviourManagerComponent.SetCurrentTree(CTFDefend_FlagTakenBehaviours.Root, true);
        }

        public override void SwitchToTree_FlagNotTaken(){
            if(m_waypoints.Length > 0){
                m_behaviourManagerComponent.SetCurrentTree(CTFPatrol_FlagNotTakenBehaviours.Root, true);
            }else{
                m_behaviourManagerComponent.SetCurrentTree(CTFDefend_FlagNotTakenBehaviours.Root, true);
            }
        }

#region Behaviour Logic
        protected override bool IsWithinSight(){
            return m_botVisionComponent.IsWithinSight(CTFGameManager.TAG_OFFENCE);
        }
        
        protected bool IsFlagNotCapturedCondition(){
            return false;
        }
        
#region Defend
        private void DoDefendOnStarted_Listener(){
            SeekBot();
        }
        private BehaviourAction.ActionResult DoDefend(bool cancel)
        {
            if(cancel || !IsFocused() ||!IsWithinDefenceRadius()){
                m_botComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }

            if(m_botComponent.IsWithinInteractionDistance()){
                return BehaviourAction.ActionResult.SUCCESS;
            }

            return BehaviourAction.ActionResult.PROGRESS;
        }
#endregion Defend

        private void MoveToWaypoint(){
            m_botComponent.FocusOnTransform(m_waypoints[m_waypointIndex]);
            m_botComponent.MoveToTarget(BotLocomotion.StopMovementConditions.WITHIN_PERSONAL_SPACE, false);
        }


        private void MoveToNextWaypoint(){
            m_waypointIndex = CollectionHelpers.GetNextCircularIndex(m_waypointIndex, m_waypoints.Length);
            MoveToWaypoint();
        }

#region Patrol
        private void DoPatrol_OnStarted(){
            m_waypointIndex = Array.IndexOf(m_waypoints, transform.GetClosestTransform(m_waypoints));
            MoveToWaypoint();
        }

        private BehaviourAction.ActionResult DoPatrol(bool cancel)
        {
            if(cancel){
                return BehaviourAction.ActionResult.FAILED;
            }
            if(IsWithinSight()){
                SeekBot();
                return BehaviourAction.ActionResult.SUCCESS;
            }

            if(m_botLocomotionComponent.IsNotFocusedOrReachedDestination()){
                MoveToNextWaypoint();
            }

            return BehaviourAction.ActionResult.PROGRESS;
        }

#endregion Patrol

#endregion Behaviour Logic

        private void SeekBot(){
            TargetEnemyBot = m_botVisionComponent.DoLookoutFor(false, false, CTFGameManager.TAG_OFFENCE)[0].GetComponent<CTFBot>();
            TargetEnemyBot.OnDie.AddListener(OnTargetDie_Listener); 
            m_botComponent.MoveToTarget(BotLocomotion.StopMovementConditions.WITHIN_PERSONAL_SPACE);
        }

        #region Events
        public void OnTargetDie_Listener(CTFBot target){
           target.OnDie.RemoveListener(OnTargetDie_Listener);
           if(target == TargetEnemyBot){
               TargetEnemyBot = null;
               m_botComponent.UnFocus();
           }
        }

        #endregion Events

        #region Mono Functions
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnDrawGizmo(){
            #if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = new Color(1, 1, 0, 0.3f);
            UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, m_defendRadius);
            UnityEditor.Handles.color = oldColor;
            #endif
        }
        #endregion Mono Functions
    }
}

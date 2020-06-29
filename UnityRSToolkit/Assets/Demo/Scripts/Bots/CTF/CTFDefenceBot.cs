using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI;


namespace Demo.CTF{
    public class CTFDefenceBot : CTFBot
    {
        [SerializeField]
        private Transform[] m_waypoints;

        public struct DefendFlagNotTakenBehaviours{
            public BehaviourSelector MainSelector;
            public BehaviourAction DoSeekStartingPosition ;
            public BehaviourCondition IsWithinSight;
            public BehaviourAction DoDefend;
        }

        public struct DefendFlagTakenBehaviours{
            public BehaviourCondition IsFlagNotCaptured;
            public BehaviourAction DoSeekEnemy;
        }

        public struct PatrolFlagTakenBehaviours{
            public BehaviourSequence PatrolSequence;
            public BehaviourAction DoPatrol;
            public BehaviourAction DoSeekFlag;
        }

        public DefendFlagNotTakenBehaviours CTFDefendFlagNotTakenBehaviours; 
        public DefendFlagTakenBehaviours CTFDefendFlagTakenBehaviours; 
        public PatrolFlagTakenBehaviours CTFPatrolFlagTakenBehaviours; 

        protected override void InitFlagNotTakenBehaviours(){
            CTFDefendFlagNotTakenBehaviours.MainSelector = new BehaviourSelector(false);

            CTFDefendFlagNotTakenBehaviours.DoDefend = new BehaviourAction(DoDefendAction, "Do Defend");
            CTFDefendFlagNotTakenBehaviours.IsWithinSight = new BehaviourCondition(IsWithinSight, CTFDefendFlagNotTakenBehaviours.DoDefend);
            CTFDefendFlagNotTakenBehaviours.MainSelector.AddChild(CTFDefendFlagNotTakenBehaviours.IsWithinSight);

            CTFDefendFlagNotTakenBehaviours.DoSeekStartingPosition = new BehaviourAction(DoSeekAction, "Do Seek");
        }
        protected override void InitFlagTakenBehaviours(){
            if(m_waypoints.Length > 0){
                InitPatrolFlagTakenBehaviours();
            }else{
                InitDefendFlagTakenBehaviours();
            }
        }
        protected void InitDefendFlagTakenBehaviours(){
            CTFDefendFlagTakenBehaviours.DoSeekEnemy = new BehaviourAction(DoSeekEnemyAction, "Do Seek Enemy");
            CTFDefendFlagTakenBehaviours.IsFlagNotCaptured = new BehaviourCondition(IsFlagNotCapturedCondition, CTFDefendFlagTakenBehaviours.DoSeekEnemy);
        }
        
        protected bool IsFlagNotCapturedCondition(){
            return false;
        }

        protected void InitPatrolFlagTakenBehaviours(){
            CTFPatrolFlagTakenBehaviours.PatrolSequence = new BehaviourSequence(false);
            CTFPatrolFlagTakenBehaviours.DoPatrol = new BehaviourAction(DoPatrolAction, "Do Patrol Action");
            CTFPatrolFlagTakenBehaviours.DoSeekFlag = new BehaviourAction(DoSeekAction, "Do Seek Action");
        }
        private BehaviourAction.ActionResult DoDefendAction(bool cancel)
        {
            if(cancel){
                m_botComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }
            return BehaviourAction.ActionResult.PROGRESS;
        }
        private BehaviourAction.ActionResult DoPatrolAction(bool cancel)
        {
            if(cancel){
                return BehaviourAction.ActionResult.FAILED;
            }
            return BehaviourAction.ActionResult.PROGRESS;
        }

        private BehaviourAction.ActionResult DoSeekEnemyAction(bool cancel)
        {
            if(cancel){
                return BehaviourAction.ActionResult.FAILED;
            }
            return BehaviourAction.ActionResult.PROGRESS;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

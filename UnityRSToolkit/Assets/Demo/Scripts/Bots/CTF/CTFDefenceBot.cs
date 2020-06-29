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
        public struct DefendFlagNotTakenBehaviours{
            public BehaviourSelector MainSelector;
            public BehaviourAction DoSeek;
            public BehaviourCondition IsWithinSight;
            public BehaviourAction DoDefend;
        }

        public struct DefendFlagTakenBehaviours{
            public BehaviourSequence MainSequence;
            public BehaviourInverter IsFlagCapturedInverter;
            public BehaviourCondition IsFlagCaptured;
            public BehaviourAction DoSeekEnemy;
        }

        public DefendFlagNotTakenBehaviours CTFFlagNotTakenBehaviours; 
        public DefendFlagTakenBehaviours CTFFlagTakenBehaviours; 

        protected override void InitFlagNotTakenBehaviours(){
            CTFFlagNotTakenBehaviours.MainSelector = new BehaviourSelector(false);

            CTFFlagNotTakenBehaviours.DoDefend = new BehaviourAction(DoDefendAction, "Do Defend");
            CTFFlagNotTakenBehaviours.IsWithinSight = new BehaviourCondition(IsWithinSight, CTFFlagNotTakenBehaviours.DoDefend);
            CTFFlagNotTakenBehaviours.MainSelector.AddChild(CTFFlagNotTakenBehaviours.IsWithinSight);

            CTFFlagNotTakenBehaviours.DoSeek = new BehaviourAction(DoSeekAction, "Do Seek");
        }
        protected override void InitFlagTakenBehaviours(){
            CTFFlagTakenBehaviours.MainSequence = new BehaviourSequence(false);
        }

        private BehaviourAction.ActionResult DoDefendAction(bool cancel)
        {
            if(cancel){
                m_botComponent.StopMoving();
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

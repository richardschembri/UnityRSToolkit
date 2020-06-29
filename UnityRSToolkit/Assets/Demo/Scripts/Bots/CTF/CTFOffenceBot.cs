using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI;

namespace Demo.CTF{
    public class CTFOffenceBot : CTFBot 
    {

        public struct OffenceFlagNotTakenBehaviours{
            public BehaviourSelector MainSelector;
            public BehaviourCondition IsEnemyNotWithinDistance;
            public BehaviourAction DoSeekFlag;
            public BehaviourAction DoFlee;
        }
        public struct OffenceFlagTakenBehaviours{
            public BehaviourSelector MainSelector;
            public BehaviourCondition HasFlag;            
            public BehaviourSequence TakeFlagToCapturePointSequence;
            public BehaviourAction DoTakeCelebrate;
            public BehaviourCondition IsFlagNotCaptured;            
            public BehaviourSequence SeekFlagSequence;
            public BehaviourAction DoSeekFlag;
            public BehaviourAction DoSeekCelebrate;

        }
        protected override void InitFlagNotTakenBehaviours(){
        }
        protected override void InitFlagTakenBehaviours(){

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

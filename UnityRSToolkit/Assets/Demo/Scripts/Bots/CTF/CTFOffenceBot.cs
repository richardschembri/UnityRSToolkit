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

        #region Behaviour Structs

        public struct OffenceFlagNotTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourSelector MainSelector;
            public BehaviourCondition IsEnemyNotWithinDistance;
            public BehaviourAction DoSeekFlag;
            public BehaviourAction DoFlee;
        }

        public struct OffenceFlagTakenBehaviours{
            public BehaviourRootNode Root;
            public BehaviourSelector MainSelector;
            public BehaviourCondition HasFlag;            
            public BehaviourSequence TakeFlagToCapturePointSequence;
            public BehaviourAction DoTakeCelebrate;
            public BehaviourCondition IsFlagNotCaptured;            
            public BehaviourSequence SeekFlagSequence;
            public BehaviourAction DoSeekFlag;
            public BehaviourAction DoSeekCelebrate;

        }

        #endregion Behaviour Structs
        public OffenceFlagNotTakenBehaviours CTFOffence_FlagNotTakenBehaviours; 
        public OffenceFlagTakenBehaviours CTFOffence_FlagTakenBehaviours; 

        public override void SwitchToTree_FlagTaken(){
            m_behaviourManagerComponent.SetCurrentTree(CTFOffence_FlagTakenBehaviours.Root, true);
        }

        public override void SwitchToTree_FlagNotTaken(){
            m_behaviourManagerComponent.SetCurrentTree(CTFOffence_FlagNotTakenBehaviours.Root, true);
        }

        protected override bool IsWithinSight(){
            return m_botVisionComponent.IsWithinSight(CTFGameManager.TAG_DEFENCE);
        }
        protected override void InitFlagNotTakenBehaviours(){
            CTFOffence_FlagNotTakenBehaviours.Root = new BehaviourRootNode("Flag Not Taken");
            CTFOffence_FlagNotTakenBehaviours.MainSelector = new BehaviourSelector(false);
        }
        protected override void InitFlagTakenBehaviours(){
            CTFOffence_FlagTakenBehaviours.Root = new BehaviourRootNode("Flag Taken");
            CTFOffence_FlagTakenBehaviours.MainSelector = new BehaviourSelector(false);
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

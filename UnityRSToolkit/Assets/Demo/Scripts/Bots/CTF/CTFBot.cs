using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI;

namespace Demo.CTF{
    [RequireComponent(typeof(Bot))]
    [RequireComponent(typeof(BotNavMesh))]
    [RequireComponent(typeof(BotVision))]
    public class CTFBot : MonoBehaviour
    {
        [SerializeField]
        private bool m_isOffense;
        public bool IsOffense {get {return m_isOffense;}}

        private Vector3 m_startPosition;
        private Quaternion m_startRotation;

        private BehaviourManager m_behaviourManagerComponent;
        private Bot m_botComponent;
        private BotVision m_botVisionComponent;
        private BotNavMesh m_botNavMeshComponent;

        public bool HasFlag {get;set;}

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
            public BehaviourAction DoSeek;
        }

        public DefendFlagNotTakenBehaviours CTFDefendFlagNotTakenBehaviours; 
        public DefendFlagTakenBehaviours CTFDefendFlagTakenBehaviours; 
        private void InitDefendBehaviours(){
            CTFDefendFlagNotTakenBehaviours.MainSelector = new BehaviourSelector(false);

            CTFDefendFlagNotTakenBehaviours.DoDefend = new BehaviourAction(DoDefendAction, "Do Defend");
            CTFDefendFlagNotTakenBehaviours.IsWithinSight = new BehaviourCondition(IsWithinSight, CTFDefendFlagNotTakenBehaviours.DoDefend);
            CTFDefendFlagNotTakenBehaviours.MainSelector.AddChild(CTFDefendFlagNotTakenBehaviours.IsWithinSight);

            CTFDefendFlagNotTakenBehaviours.DoSeek = new BehaviourAction(DoSeekAction, "Do Seek");

            //CTFDefendFlagTakenBehaviours.MainSequence = 
            //CTFDefendFlagNotTakenBehaviours.MainSelector = new BehaviourSelector(false);
        }

        private bool IsWithinSight(){
            if(m_botVisionComponent.IsWithinSight()){
                
                m_botComponent.FocusOnTransform(
                    m_botVisionComponent.
                    GetTrasnformsWithinSight(false)[0]);
                return true;
            }
            return false;
        }

        private void DoSeekOnStarted_Listener(){
            m_botComponent.MoveToTarget(BotLocomotion.StopMovementConditions.WITHIN_PERSONAL_SPACE);
        }
        
        private BehaviourAction.ActionResult DoSeekAction(bool cancel){
            if(cancel){
                m_botComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }
            if(m_botComponent.IsWithinPersonalSpace()){
                return BehaviourAction.ActionResult.SUCCESS;
            }
            return BehaviourAction.ActionResult.PROGRESS;
        }

        private BehaviourAction.ActionResult DoDefendAction(bool cancel)
        {
            if(cancel){
                m_botComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }
            return BehaviourAction.ActionResult.PROGRESS;
        }

        #region Mono Functions
        // Start is called before the first frame update
        void Awake()
        {
            m_botComponent = GetComponent<Bot>();
            m_behaviourManagerComponent = GetComponent<BehaviourManager>();
            m_botVisionComponent = GetComponent<BotVision>();
            m_botNavMeshComponent = GetComponent<BotNavMesh>();
            m_startPosition = transform.position;
            m_startRotation = transform.rotation;
        }
        #endregion Mono Functions
    }
}
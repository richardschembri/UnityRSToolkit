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
    public abstract class CTFBot : MonoBehaviour
    {
        private Vector3 m_startPosition;
        private Quaternion m_startRotation;

        private BehaviourManager m_behaviourManagerComponent;
        protected Bot m_botComponent;
        protected BotVision m_botVisionComponent;
        protected BotNavMesh m_botNavMeshComponent;

        public bool HasFlag {get;set;}

        protected abstract void InitFlagNotTakenBehaviours();
        protected abstract void InitFlagTakenBehaviours();
        protected virtual void InitBehaviours(){
            InitFlagNotTakenBehaviours();
            InitFlagTakenBehaviours();
        }

        protected bool IsWithinSight(){
            if(m_botVisionComponent.IsWithinSight()){
                
                m_botComponent.FocusOnTransform(
                    m_botVisionComponent.
                    GetTrasnformsWithinSight(false)[0]);
                return true;
            }
            return false;
        }

        protected virtual void DoSeekOnStarted_Listener(){
            m_botComponent.MoveToTarget(BotLocomotion.StopMovementConditions.WITHIN_PERSONAL_SPACE);
        }
        
        protected virtual BehaviourAction.ActionResult DoSeekAction(bool cancel){
            if(cancel){
                m_botComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }
            if(m_botComponent.IsWithinPersonalSpace()){
                return BehaviourAction.ActionResult.SUCCESS;
            }
            return BehaviourAction.ActionResult.PROGRESS;
        }


        #region Mono Functions
        // Start is called before the first frame update
        protected virtual void Awake()
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
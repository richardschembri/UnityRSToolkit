using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI;
using RSToolkit.Helpers;
using UnityEngine.Events;

namespace Demo.CTF{
    [RequireComponent(typeof(Bot))]
    [RequireComponent(typeof(BotNavMesh))]
    [RequireComponent(typeof(BotVision))]
    public abstract class CTFBot : MonoBehaviour
    {
        public Vector3 StartPosition {get; private set;}
        public Quaternion StartRotation {get; private set;}

        protected BehaviourManager m_behaviourManagerComponent;
        protected Bot m_botComponent;
        protected BotLocomotion m_botLocomotionComponent;
        protected BotVision m_botVisionComponent;
        protected BotNavMesh m_botNavMeshComponent;
        public GameObject FlagHolder {get; set;}

        public class OnDieEvent : UnityEvent<CTFBot>{} 
        public OnDieEvent OnDie{get; private set;}

        public bool HasFlag {get {return FlagHolder.transform.childCount > 0;}}

        protected abstract void InitFlagNotTakenBehaviours();
        protected abstract void InitFlagTakenBehaviours();
        protected virtual void InitBehaviours(){
            InitFlagNotTakenBehaviours();
            InitFlagTakenBehaviours();
        }

        protected abstract bool IsWithinSight();

        protected virtual void DoSeekOnStarted_Listener(){
            m_botComponent.MoveToTarget(BotLocomotion.StopMovementConditions.WITHIN_PERSONAL_SPACE);
        }
        
        protected virtual BehaviourAction.ActionResult DoSeek(bool cancel){
            if(cancel || !m_botComponent.IsFocused){
                m_botComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }
            if(m_botComponent.IsWithinPersonalSpace()){
                if(m_botComponent.IsFacing()){
                    return BehaviourAction.ActionResult.SUCCESS;
                }
                m_botComponent.RotateTowardsPosition();
            }
            return BehaviourAction.ActionResult.PROGRESS;
        }

        public abstract void SwitchToTree_FlagTaken();

        public abstract void SwitchToTree_FlagNotTaken();

        public void ResetBot(){
            m_behaviourManagerComponent.SetCurrentTree(GetDefaultTree(), true);
            transform.position = StartPosition;
            transform.rotation = StartRotation;
            m_botComponent.StopMoving();
        }

        protected abstract BehaviourRootNode GetDefaultTree();


        #region Mono Functions
        // Start is called before the first frame update
        protected virtual void Awake()
        {
            OnDie = new OnDieEvent();
            m_botComponent = GetComponent<Bot>();
            m_behaviourManagerComponent  = GetComponent<BehaviourManager>();
            m_botVisionComponent = GetComponent<BotVision>();
            m_botNavMeshComponent = GetComponent<BotNavMesh>();
            FlagHolder = gameObject.GetChild("Flag Holder");
            StartPosition = transform.position;
            StartRotation = transform.rotation;
        }
        #endregion Mono Functions
    }
}
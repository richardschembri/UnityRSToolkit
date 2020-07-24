using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.AI.Behaviour;
using RSToolkit.AI.Behaviour.Composite;
using RSToolkit.AI.Behaviour.Task;
using RSToolkit.AI.Behaviour.Decorator;
using RSToolkit.AI;
using RSToolkit.AI.Locomotion;
using RSToolkit.Helpers;
using UnityEngine.Events;

namespace Demo.BehaviourTree.CTF{
    [RequireComponent(typeof(Bot))]
    [RequireComponent(typeof(BotLogicNavMesh))]
    [RequireComponent(typeof(BotPartVision))]
    public abstract class CTFBot : MonoBehaviour
    {
        public Vector3 StartPosition {get; private set;}
        public Quaternion StartRotation {get; private set;}

        protected BehaviourManager m_behaviourManagerComponent;
        protected BotLocomotive _botLocomotiveComponent;
        protected BotLogicLocomotion m_botLocomotionComponent;
        protected BotPartVision m_botVisionComponent;
        protected BotLogicNavMesh m_botNavMeshComponent;
        public GameObject FlagHolder {get; set;}

        public class OnDieEvent : UnityEvent<CTFBot>{} 
        public OnDieEvent OnDie{get; private set;}

        public bool HasFlag() {return FlagHolder.transform.childCount > 0;}

        protected abstract void InitFlagNotTakenBehaviours();
        protected abstract void InitFlagTakenBehaviours();
        protected virtual void InitBehaviours(){
            InitFlagNotTakenBehaviours();
            InitFlagTakenBehaviours();
        }

        protected abstract bool IsEnemyWithinSight();

        protected virtual void DoSeekOnStarted_Listener(){
            _botLocomotiveComponent.MoveToTarget(BotLocomotive.StopMovementConditions.WITHIN_PERSONAL_SPACE);
        }
        
        protected virtual BehaviourAction.ActionResult DoSeek(bool cancel){
            if(cancel || !_botLocomotiveComponent.IsFocused){
                _botLocomotiveComponent.StopMoving();
                return BehaviourAction.ActionResult.FAILED;
            }
            if(_botLocomotiveComponent.IsWithinPersonalSpace()){
                if(_botLocomotiveComponent.IsFacing()){
                    return BehaviourAction.ActionResult.SUCCESS;
                }
                _botLocomotiveComponent.RotateTowardsPosition();
            }
            return BehaviourAction.ActionResult.PROGRESS;
        }

        public abstract void SwitchToTree_FlagTaken();

        public abstract void SwitchToTree_FlagNotTaken();

        public void ResetBot(){
            m_behaviourManagerComponent.SetCurrentTree(GetDefaultTree(), true);
            transform.position = StartPosition;
            transform.rotation = StartRotation;
            _botLocomotiveComponent.StopMoving();
        }

        protected abstract BehaviourRootNode GetDefaultTree();


        #region Mono Functions
        // Start is called before the first frame update
        protected virtual void Awake()
        {
            OnDie = new OnDieEvent();
            _botLocomotiveComponent = GetComponent<BotLocomotive>();
            m_behaviourManagerComponent  = GetComponent<BehaviourManager>();
            m_botVisionComponent = GetComponent<BotPartVision>();
            m_botNavMeshComponent = GetComponent<BotLogicNavMesh>();
            FlagHolder = gameObject.GetChild("Flag Holder");
            StartPosition = transform.position;
            StartRotation = transform.rotation;
        }
        #endregion Mono Functions
    }
}

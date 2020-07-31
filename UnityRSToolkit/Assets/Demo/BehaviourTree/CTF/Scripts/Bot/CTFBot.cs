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
    [RequireComponent(typeof(BotPartVision))]
    public abstract class CTFBot : MonoBehaviour
    {
#region Start Values

        public Vector3 StartPosition {get; private set;}
        public Quaternion StartRotation {get; private set;}

#endregion Start Values

#region Components

        protected BehaviourManager _behaviourManagerComponent;
        protected BotLocomotive _botLocomotiveComponent;
        protected BotLogicLocomotion _botLocomotionComponent;
        protected BotPartVision _botVisionComponent;
        protected BotLogicNavMesh _botNavMeshComponent;

        protected void InitComponents()
        {
            _botLocomotiveComponent = GetComponent<BotLocomotive>();
            _behaviourManagerComponent = GetComponent<BehaviourManager>();
            _botVisionComponent = GetComponent<BotPartVision>();
            _botNavMeshComponent = GetComponent<BotLogicNavMesh>();
            FlagHolder = gameObject.GetChild("Flag Holder");
        }

#endregion Components

        public GameObject FlagHolder {get; set;}

        public class OnDieEvent : UnityEvent<CTFBot>{}
        public OnDieEvent OnDie{get; private set;}

        public bool HasFlag() {return FlagHolder.transform.childCount > 0;}

#region Init Behaviours

        protected abstract void InitFlagNotTakenBehaviours();
        protected abstract void InitFlagTakenBehaviours();
        protected virtual void InitBehaviours(){
            InitFlagNotTakenBehaviours();
            InitFlagTakenBehaviours();
        }

#endregion Init Behaviours

#region Behaviour Logic

#endregion Behaviour Logic

        public abstract void SwitchToTree_FlagTaken();

        public abstract void SwitchToTree_FlagNotTaken();

        public void ResetBot(){
            _behaviourManagerComponent.SetCurrentTree(GetDefaultTree(), true);
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
            InitComponents();
            InitBehaviours();
            StartPosition = transform.position;
            StartRotation = transform.rotation;
        }

        protected virtual void Start()
        {
            _behaviourManagerComponent.SetCurrentTree(GetDefaultTree(), false);
            _behaviourManagerComponent.StartTree();
        }
#endregion Mono Functions
    }
}

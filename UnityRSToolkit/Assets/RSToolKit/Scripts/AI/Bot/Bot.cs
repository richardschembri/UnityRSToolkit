using RSToolkit.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RSToolkit.AI.Locomotion;
using RSToolkit.AI.FSM;
using UnityEngine.Events;

namespace RSToolkit.AI
{
    [DisallowMultipleComponent]
    public class Bot : MonoBehaviour
    {

        #region Enums

        public enum StatesInteraction
        {
            NotInteracting,
            Interactor,
            Interactee
        }

        public enum NetworkTypes
        {
            None,
            Owner,
            Peer
        }

        public enum DistanceType{
            AT_POSITION,
            PERSONAL_SPACE,
            INTERACTION,
            AWARENESS,
            OUTSIDE_AWARENESS
        }

        #endregion Enums

        public StatesInteraction CurrentInteractionState { get; private set; } = StatesInteraction.NotInteracting;

        public float InteractableCooldown = 0f;
        public float CanInteractFromTime { get; private set; } = 0f;
        public Transform TetherToTransform;

        public bool DebugMode = false;
        public NetworkTypes NetworkType { get; private set; } = NetworkTypes.None;

        protected bool _IsNetworkPeer
        {
            get
            {
                return NetworkType == NetworkTypes.Peer;
            }
        }

        public UnityEvent OnAwake = new UnityEvent();

        public void SetNetworkType(NetworkTypes networkType, bool toggleKinematic = true)
        {
            NetworkType = networkType;
            ToggleComponentsForNetwork(toggleKinematic);
        }

        #region Components
        public BTFiniteStateMachineManager BTFiniteStateMachineManagerComponent {get; private set;}

        private Animator _animatorComponent;
        public Animator AnimatorComponent
        {
            get
            {
                if (_animatorComponent == null)
                {
                    _animatorComponent = GetComponent<Animator>();
                    if(_animatorComponent == null)
                    {
                        _animatorComponent = GetComponentInChildren<Animator>();
                    }
                }
                return _animatorComponent;
            }
        }

        private Collider _colliderComponent;
        public Collider ColliderComponent
        {
            get
            {
                if (_colliderComponent == null)
                {
                    _colliderComponent = GetComponent<Collider>();
                }
                return _colliderComponent;
            }
        }

        private Rigidbody _rigidBodyComponent;
        public Rigidbody RigidBodyComponent
        {
            get
            {
                if(_rigidBodyComponent == null)
                {
                    _rigidBodyComponent = GetComponent<Rigidbody>();
                }
                return _rigidBodyComponent;
            }
        }

        protected virtual void ToggleComponentsForNetwork(bool toggleKinematic = true)
        {
            //BTFiniteStateMachineManagerComponent.IsSilent = NetworkType == NetworkTypes.Peer;
            if(NetworkType == NetworkTypes.Peer)
            {
                BTFiniteStateMachineManagerComponent.IsSilent = true;

                if (toggleKinematic)
                {
                    RigidBodyComponent.isKinematic = true;
                }                
            }
            else
            {
                BTFiniteStateMachineManagerComponent.IsSilent = false;

                if (toggleKinematic)
                {
                    RigidBodyComponent.isKinematic = false;
                }
            }
        }

        #endregion Components

        public Transform FocusedOnTransform { get; private set; } = null;
        public Vector3? m_FocusedOnPosition = null;

        public HashSet<Transform> NoticedTransforms { get; private set; } = new HashSet<Transform>();
        public float forgetTransformTimeout = -1f;

        #region Interaction/Focus

        public Vector3? FocusedOnPosition
        {
            get
            {
                if (FocusedOnTransform != null)
                {
                    m_FocusedOnPosition = null;
                    return FocusedOnTransform.position;
                }
                return m_FocusedOnPosition;
            }
            private set
            {
                m_FocusedOnPosition = value;
            }
        }

        public bool IsFocused
        {
            get
            {
                return FocusedOnPosition != null;
            }
        }

        #region Magnitude

        [SerializeField]
        private float m_interactionMagnitude = 1.35f;

        public virtual float InteractionMagnitude
        {
            get
            {
                return m_interactionMagnitude;
            }
        }
        public float SqrInteractionMagnitude
        {
            get
            {
                return InteractionMagnitude * InteractionMagnitude;
            }
        }

        [SerializeField]
        public float personalSpacePercent = .35f;
        public float SqrPersonalSpaceMagnitude
        {
            get
            {                
                return SqrInteractionMagnitude * personalSpacePercent;
                /*
                return Mathf.Max(SqrInteractionMagnitude * personalSpacePercent, 
                                    Mathf.Max(ColliderComponent.bounds.size.x * 1.1f, 
                                                ColliderComponent.bounds.size.z * 1.1f));
                */
            }
        }

        [SerializeField]
        public float safeAwarenessPercent = 2f;
        public float SqrAwarenessMagnitude
        {
            get
            {
                return SqrInteractionMagnitude * safeAwarenessPercent;
            }
        }

        [SerializeField]
        public float atPositionErrorMargin = 0.1f;
        public float SqrAtPositionErrorMargin
        {
            get
            {
                return SqrInteractionMagnitude * atPositionErrorMargin;
            }
        }

        #endregion Magnitude

        #region IsWithinDistance

        public bool IsWithinDistance(DistanceType distanceType, Vector3 position,
            ProximityHelpers.DistanceDirection direction = ProximityHelpers.DistanceDirection.ALL,
            float percent = 1.0f)
        {
            float sqrMagnitude = 0f;
            switch(distanceType){
                case DistanceType.AT_POSITION:
                    if(SqrAtPositionErrorMargin == 0)
                    {
                        return transform.position == position;
                    }
                    sqrMagnitude = SqrAtPositionErrorMargin;
                    break;
                case DistanceType.PERSONAL_SPACE:
                    sqrMagnitude = SqrPersonalSpaceMagnitude;
                    break;
                case DistanceType.INTERACTION:
                    sqrMagnitude = SqrInteractionMagnitude;
                    break;
                case DistanceType.AWARENESS:
                case DistanceType.OUTSIDE_AWARENESS:
                    sqrMagnitude = SqrAwarenessMagnitude;
                    break;
            }

            bool result = ProximityHelpers.IsWithinDistance(ColliderComponent, position, sqrMagnitude * percent, direction)
                            || ProximityHelpers.IsWithinDistance(transform.position, position, sqrMagnitude * percent, direction);

            if (distanceType == DistanceType.OUTSIDE_AWARENESS)
            {
                return !result;
            }

            return result;
        }

        public bool IsWithinDistance(DistanceType distanceType, Transform target,
            ProximityHelpers.DistanceDirection direction = ProximityHelpers.DistanceDirection.ALL,
            float percent = 1.0f)
        {
            return IsWithinDistance(distanceType, target.position, direction,  percent);
        }
        public bool IsWithinDistance(DistanceType distanceType,
            ProximityHelpers.DistanceDirection direction = ProximityHelpers.DistanceDirection.ALL,
            float percent = 1.0f)
        {
            if (FocusedOnTransform != null)
            {
                return IsWithinDistance(distanceType, FocusedOnTransform.position, direction,  percent);
            }
            else if (FocusedOnPosition != null)
            {
                return IsWithinDistance(distanceType, FocusedOnPosition.Value, direction, percent);
            }
            return false;
        }

        #endregion IsWithinDistance

        public DistanceType? GetDistanceTypeFromFocusedPosition()
        {
            if(FocusedOnPosition == null)
            {
                return null;
            }

            if (IsWithinDistance(DistanceType.AT_POSITION))
            {
                return DistanceType.AT_POSITION;
            }
            else if (IsWithinDistance(DistanceType.PERSONAL_SPACE))
            {
                return DistanceType.PERSONAL_SPACE;
            }
            else if (IsWithinDistance(DistanceType.INTERACTION))
            {
                return DistanceType.INTERACTION;
            }
            else if (IsWithinDistance(DistanceType.AWARENESS))
            {
                return DistanceType.AWARENESS;
            }

            return DistanceType.OUTSIDE_AWARENESS;
        }

        public void ResetInteractionCooldown(float percent = 1.0f)
        {
            CanInteractFromTime = Time.time + (InteractableCooldown * percent);
        }

        private bool ChangeInteractionState(StatesInteraction interactionState, bool force)
        {
            if (interactionState == StatesInteraction.NotInteracting)
            {
                CurrentInteractionState = interactionState;
                ResetInteractionCooldown();
                return true;
            }
            else if (force || Time.time > CanInteractFromTime)
            {
                CurrentInteractionState = interactionState;
                return true;
            }
            return false;
        }

        #region AttractMyAttention
        
        public bool AttractMyAttention_ToTransform(Transform target, bool force, StatesInteraction interactionState = StatesInteraction.Interactor, 
                                                    ProximityHelpers.DistanceDirection distanceDirection = ProximityHelpers.DistanceDirection.ALL)
        {

            if (IsWithinDistance(DistanceType.INTERACTION, target, distanceDirection) || force)
            {
                if (ChangeInteractionState(interactionState, force))
                {
                    FocusOnTransform(target);
                    //CurrentInteractionState = interactionState;
                    return true;
                }
            }
            return false;

        }

        public bool AttractMyAttention_ToBot(Bot target, bool force, StatesInteraction interactionState = StatesInteraction.Interactor)
        {

            if (IsWithinDistance(DistanceType.INTERACTION, target.transform) || target.IsWithinDistance(DistanceType.INTERACTION, transform) || force)
            {
                if (target.CurrentInteractionState == StatesInteraction.Interactor)
                {
                    interactionState = StatesInteraction.Interactee;
                }

                if (ChangeInteractionState(interactionState, force))
                {
                    FocusOnTransform(target.transform);
                    //CurrentInteractionState = interactionState;
                    return true;
                }
            }
            return false;

        }

        public bool AttractMyAttention_ToTransform(bool force)
        {
            return AttractMyAttention_ToTransform(FocusedOnTransform, force);

        }

        public bool AttractMyAttention_FromBot(Bot target, bool force)
        {
            if (target.FocusedOnTransform != transform)
            {
                return target.AttractMyAttention_ToBot(this, force, StatesInteraction.Interactee);
            }
            return true;
        }

        public bool AttractMyAttention_FromBot(bool force)
        {
            return AttractMyAttention_FromBot(FocusedOnTransform.GetComponent<Bot>(), force);

        }

        #endregion AttractMyAttention

        public void FocusOnPosition(Vector3 target_position)
        {
            FocusedOnPosition = target_position;
        }

        public bool NoticeTransform(Transform target)
        {
            if (!NoticedTransforms.Contains(target))
            {
                NoticedTransforms.Add(target);
                return true;
            }
            return false;
        }

        public void ForgetTransformAfterTiemout(Transform target)
        {
            if (forgetTransformTimeout > 0)
            {
                StartCoroutine(DelayedForgetTransform(target));

            }else{
                ForgetTransform(target);
            }
        }

        IEnumerator DelayedForgetTransform(Transform target)
        {
            yield return new WaitForSeconds(forgetTransformTimeout);
            ForgetTransform(target);

        }

        public void ForgetTransform(Transform target)
        {
            if (DebugMode)
            {
                Debug.Log($"{transform.name}.ForgetTransform: {target.name}");
            }
            NoticedTransforms.Remove(target);

        }

        public void FocusOnTransform(Transform target)
        {
            if(target == FocusedOnTransform)
            {
                return;
            }
            if (DebugMode)
            {
                Debug.Log($"{transform.name}.FocusOnTransform: {target.name}");
            }
            NoticeTransform(target);
            FocusedOnTransform = target;

        }

        public bool UnFocus(bool forget = true)
        {
            if(FocusedOnPosition != null && FocusedOnTransform == null)
            {
                FocusedOnPosition = null;
                return true;
            }
            else if (FocusedOnTransform == null)
            {
                return false;
            }
            if (DebugMode)
            {
                Debug.Log($"{transform.name}.UnFocus: {FocusedOnTransform.name}");
            }
            if(forget){
                ForgetTransformAfterTiemout(FocusedOnTransform);
            }
            FocusedOnTransform = null;
            FocusedOnPosition = null;
            CurrentInteractionState = StatesInteraction.NotInteracting;
            return true;
        }

        public bool IsFacing(Transform target)
        {
            return Mathf.Round(transform.rotation.eulerAngles.y * 10) == Mathf.Round(Quaternion.LookRotation(target.position - transform.position, Vector3.up).eulerAngles.y * 10);
            // return transform.rotation == Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        }

        public bool IsFacing()
        {
            if (FocusedOnTransform != null)
            {
                return IsFacing(FocusedOnTransform);
            }
            return false;
        }

        public virtual bool CanInteract()
        {
            return Time.time > CanInteractFromTime;
        }

        public bool CanInteractWith(Bot target)
        {
            return target != null && (
                target.FocusedOnTransform == transform 
                || (target.CanInteract() && target.FocusedOnTransform == null 
                        && !target.NoticedTransforms.Contains(transform))
            );
        }

        #endregion Interaction/Focus

        #region Intialize

        public bool AutoInitialize = true;
        public bool Initialized { get; private set; } = false;

        public virtual bool Initialize(bool force = false)
        {
            if(Initialized && !force)
            {
                return false;
            }
            Initialized = false;
            BTFiniteStateMachineManagerComponent = GetComponent<BTFiniteStateMachineManager>();           
            Initialized = true;
            return true;
        }

        public virtual bool Initialize(NetworkTypes networkType, bool force = false)
        {
            if (Initialize(force))
            {
                SetNetworkType(networkType);
                return true;
            }
            
            return false;
        }

        #endregion Intialize

        #region MonoBehaviour Functions

        protected virtual void Awake()
        {
            if (AutoInitialize)
            {
                Initialize();
            }
            OnAwake.Invoke();
        }

        protected virtual void Start()
        {
            if (!Initialized)
            {
                return;
            }

            BTFiniteStateMachineManagerComponent.StartFSMs();
        }

        protected virtual void Update()
        {

        }

        protected virtual void OnCollisionEnter(Collision collision){

        }

        protected virtual void OnDrawGizmos()
        {
#if UNITY_EDITOR          
            ProximityHelpers.DrawGizmoProximity(transform, SqrAwarenessMagnitude, IsWithinDistance(DistanceType.AWARENESS)); // IsWithinAwarenessDistance());
            ProximityHelpers.DrawGizmoProximity(transform, SqrInteractionMagnitude, IsWithinDistance(DistanceType.INTERACTION)); //IsWithinInteractionDistance());
            ProximityHelpers.DrawGizmoProximity(transform, SqrPersonalSpaceMagnitude, IsWithinDistance(DistanceType.PERSONAL_SPACE)); ///IsWithinPersonalSpace());

            if (FocusedOnTransform != null)
            {
                UnityEditor.Handles.color = Color.yellow;
                
                DrawGizmoPositionPoint(FocusedOnTransform.position);
            }
            else if (FocusedOnPosition != null)
            {
                UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
                //UnityEditor.Handles.DrawSolidDisc(FocusedOnPosition.Value, Vector3.up, 0.25f);                
                DrawGizmoPositionPoint(FocusedOnPosition.Value);
                UnityEditor.Handles.DrawLine(transform.position, FocusedOnPosition.Value);
            }
#endif
        }

        #endregion MonoBehaviour Functions

        protected void DrawGizmoPositionPoint(Vector3 position)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
            UnityEditor.Handles.DrawSolidDisc(position, Vector3.up, SqrPersonalSpaceMagnitude / 5);
#endif
        }

    }
}
using RSToolkit.Helpers;
using RSToolkit.Network;
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
    public class Bot : RSMonoBehaviour
    {

        #region Enums

        public enum StatesInteraction
        {
            NotInteracting,
            Interactor,
            Interactee
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

        /// <summary>
        /// Is used in a network enviroment to see if this is a bot on the host
        /// or if it is a peer 
        /// </summary>
        protected bool _IsNetworkPeer
        {
            get
            {
                return RSNetworkObject.NetworkType == RSNetworkObject.NetworkTypes.Peer;
            }
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

        private RSShadow _rsShadowComponent;
        public RSShadow RSShadowComponent
        {
            get
            {
                if(_rsShadowComponent == null)
                {
                   _rsShadowComponent = GetComponent<RSShadow>();
                }
                return _rsShadowComponent;
            }
        }
        #endregion Components

        public Transform PreviousFocusedTransform { get; private set; } = null;
        public Transform FocusedOnTransform { get; private set; } = null;
        public Vector3? _FocusedOnPosition = null;

        public HashSet<Transform> NoticedTransforms { get; private set; } = new HashSet<Transform>();
        public float forgetTransformTimeout = -1f;

        #region Interaction/Focus

        public Vector3? FocusedOnPosition
        {
            get
            {
                if (FocusedOnTransform != null)
                {
                    _FocusedOnPosition = null;
                    return FocusedOnTransform.position;
                }
                return _FocusedOnPosition;
            }
            private set
            {
                _FocusedOnPosition = value;
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
	
	/// <summary>
	/// The radius of when the bot can interact with something 
	/// </summary>
        public float SqrInteractionMagnitude
        {
            get
            {
                return InteractionMagnitude * InteractionMagnitude;
            }
        }

        [SerializeField]
        public float personalSpacePercent = .35f;
	/// <summary>
	/// The radius of when something is too close 
	/// </summary>
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
	/// <summary>
	/// The radius of when bot is aware of something 
	/// </summary>
        public float SqrAwarenessMagnitude
        {
            get
            {
                return SqrInteractionMagnitude * safeAwarenessPercent;
            }
        }

        [SerializeField]
        public float atPositionErrorMargin = 0.1f;
	/// <summary>
	/// The error margin of when the bot is considered at a position 
	/// </summary>
        public float SqrAtPositionErrorMargin
        {
            get
            {
                return SqrInteractionMagnitude * atPositionErrorMargin;
            }
        }

        #endregion Magnitude

        #region IsWithinDistance

	/// <summary>
	/// Check if bot is within desired distance of a position 
	/// </summary>
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

	/// <summary>
	/// Check if bot is within desired distance of a target 
	/// </summary>
        public bool IsWithinDistance(DistanceType distanceType, Transform target,
            ProximityHelpers.DistanceDirection direction = ProximityHelpers.DistanceDirection.ALL,
            float percent = 1.0f)
        {
            return IsWithinDistance(distanceType, target.position, direction,  percent);
        }

	/// <summary>
	/// Check if bot is within desired distance of FocusedOnPosition or FocusedOnTransform 
	/// </summary>
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

	/// <summary>
	/// Get distance from FocusedOnPosition in DistanceType form 
	/// </summary>
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

	/// <summary>
	/// Reset the cooldown for how long the bot should wait before being able to
	/// interact with something 
	/// </summary>
        public void ResetInteractionCooldown(float percent = 1.0f)
        {
            CanInteractFromTime = Time.time + (InteractableCooldown * percent);
        }

	/// <param name="force">Ignore the interaction cooldown if true</param>
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
        
	/// <summary>
	/// Focus on transform and change interaction state
	/// </summary>
	/// <returns>
	/// Wether or not the function was successful
	/// </returns>
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

	/// <summary>
	/// Focus on Bot and change interaction state
	/// </summary>
	/// <returns>
	/// Wether or not the function was successful
	/// </returns>
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

	/// <summary>
	/// Focus on Transform and change interaction state to interactor
	/// </summary>
	/// <returns>
	/// Wether or not the function was successful
	/// </returns>
        public bool AttractMyAttention_ToTransform(bool force)
        {
            return AttractMyAttention_ToTransform(FocusedOnTransform, force);

        }

	/// <summary>
	/// Focus on Transform and change interaction state to Interactee
	/// </summary>
	/// <returns>
	/// Wether or not the function was successful
	/// </returns>
        public bool AttractMyAttention_FromBot(Bot target, bool force)
        {
            if (target.FocusedOnTransform != transform)
            {
                return target.AttractMyAttention_ToBot(this, force, StatesInteraction.Interactee);
            }
            return true;
        }

	/// <summary>
	/// Focus on Bot and change interaction state to Interactee
	/// </summary>
	/// <returns>
	/// Wether or not the function was successful
	/// </returns>
        public bool AttractMyAttention_FromBot(bool force)
        {
            return AttractMyAttention_FromBot(FocusedOnTransform.GetComponent<Bot>(), force);

        }

        #endregion AttractMyAttention

        public void FocusOnPosition(Vector3 target_position)
        {
            FocusedOnPosition = target_position;
        }

	/// <summary>
	/// Add Transform to Bot's "memory"
	/// </summary>
        public bool NoticeTransform(Transform target)
        {
            if (!NoticedTransforms.Contains(target))
            {
                NoticedTransforms.Add(target);
                return true;
            }
            return false;
        }

	/// <summary>
	/// Remove Transform from Bot's "memory" after a timeout
	/// </summary>
        public void ForgetTransformAfterTiemout(Transform target, System.Func<bool> forgetCondition = null)
        {
            if (forgetTransformTimeout > 0)
            {
                StartCoroutine(DelayedForgetTransform(target, forgetCondition));

            }else{
                ForgetTransform(target);
            }
        }

	/// <summary>
	/// Remove Transform from Bot's "memory" after a timeout
	/// </summary>
        IEnumerator DelayedForgetTransform(Transform target, System.Func<bool> forgetCondition = null)
        {
            
            if(forgetCondition != null)
            {
                yield return new WaitUntil(forgetCondition);
            }
            yield return new WaitForSeconds(forgetTransformTimeout);
            ForgetTransform(target);

        }

	/// <summary>
	/// Remove Transform from Bot's "memory"
	/// </summary>
        public void ForgetTransform(Transform target)
        {
            if (DebugMode)
            {
                Debug.Log($"{transform.name}.ForgetTransform: {target.name}");
            }
            NoticedTransforms.Remove(target);

        }

	/// <summary>
	/// Add a Transform to Bot's "memory"
	/// </summary>
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


	/// <summary>
	/// Nullify FocusedOnPosition and FocusedOnTransform 
	/// </summary>
	/// <param name="forget">Wether or not the FocusedOnTransform should
	/// be removed from Bot's "memory"</param>
	/// <param name="forgetCondition">The condition for which FocusedOnTransform should be forgetten</param>
	/// <returns>
	/// Wether or not the function was successful
	/// </returns>
        private bool UnFocus(bool forget, System.Func<bool> forgetCondition)
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
            if(forget)
            {
                ForgetTransformAfterTiemout(FocusedOnTransform, forgetCondition);
            }
            PreviousFocusedTransform = FocusedOnTransform;
            FocusedOnTransform = null;
            FocusedOnPosition = null;
            CurrentInteractionState = StatesInteraction.NotInteracting;
            return true;
        }

	/// <summary>
	/// Nullify FocusedOnPosition and FocusedOnTransform 
	/// </summary>
	/// <param name="forget">Wether or not the FocusedOnTransform should
	/// be removed from Bot's "memory"</param>
	/// <returns>
	/// Wether or not the function was successful
	/// </returns>
        public bool UnFocus(bool forget = true)
        {
            return UnFocus(forget, null);
        }

	/// <summary>
	/// Nullify FocusedOnPosition and FocusedOnTransform 
	/// </summary>
	/// <param name="forgetCondition">The condition for which FocusedOnTransform should be forgetten</param>
	/// <returns>
	/// Wether or not the function was successful
	/// </returns>
        public bool UnFocus(System.Func<bool> forgetCondition)
        {
            return UnFocus(true, forgetCondition);
        }

	/// <summary>
	/// Wether or not the Bot is facing the target 
	/// </summary>
        public bool IsFacing(Transform target)
        {
            return Mathf.Round(transform.rotation.eulerAngles.y * 10) == Mathf.Round(Quaternion.LookRotation(target.position - transform.position, Vector3.up).eulerAngles.y * 10);
            // return transform.rotation == Quaternion.LookRotation(target.position - transform.position, Vector3.up);
        }

        /// <summary>
        /// Wether or not the Bot is facing the FocusedOnTransform  
        /// </summary>
        public bool IsFacing()
        {
            if (FocusedOnTransform != null)
            {
                return IsFacing(FocusedOnTransform);
            }
            return false;
        }

        /// <summary>
        /// Wether or not the Bot can interact with something  
        /// </summary>
        public virtual bool CanInteract()
        {
            return Time.time > CanInteractFromTime;
        }

        /// <summary>
        /// Wether or not the Bot can interact with target Bot  
        /// </summary>
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


	/// <param name="force">Initialize even if already initialized</param>
        public override bool Init(bool force = false)
        {
            if(Init(force))
            {
                BTFiniteStateMachineManagerComponent = GetComponent<BTFiniteStateMachineManager>();           
                return true;
            }
            return false;
        }

        #endregion Intialize

        #region MonoBehaviour Functions

        protected virtual void Start()
        {
            if (!Initialized)
            {
                return;
            }

            BTFiniteStateMachineManagerComponent.StartFSMs();
        }

        protected override void Awake()
        {
            RSShadowComponent.OnRSShadowChanged.AddListener(OnRSShadowChanged_Listener);
            if(RSShadowComponent.Initialized){
                OnRSShadowChanged_Listener(RSShadowComponent.IsShadow());
            }
            base.Awake();
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

        public bool ToggleShadowKinematic = true;
        #region RSMonoBehaviour Functions
        protected virtual void OnRSShadowChanged_Listener(bool isShadow){
            if(isShadow){
                BTFiniteStateMachineManagerComponent.IsSilent = false;

                if (ToggleShadowKinematic)
                {
                    RigidBodyComponent.isKinematic = false;
                }
            }else{

                BTFiniteStateMachineManagerComponent.IsSilent = true;

                if (ToggleShadowKinematic)
                {
                    RigidBodyComponent.isKinematic = true;
                }                
            }
        } 
        #endregion RSMonoBehaviour Functions
        protected void DrawGizmoPositionPoint(Vector3 position)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
            UnityEditor.Handles.DrawSolidDisc(position, Vector3.up, SqrPersonalSpaceMagnitude / 5);
#endif
        }


        protected void LogErrorInDebugMode(string message, System.Exception ex = null)
        {
            DebugHelpers.LogErrorInDebugMode(DebugMode, GetDebugTag(), ex == null ? message : $"{message}: \n {ex.Message} \n {ex.StackTrace}");
        }
    }
}

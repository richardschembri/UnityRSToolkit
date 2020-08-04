using RSToolkit.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RSToolkit.AI.Locomotion;
using RSToolkit.AI.FSM;

namespace RSToolkit.AI
{
    [DisallowMultipleComponent]
    public class Bot : MonoBehaviour
    {

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

        public void SetNetworkType(NetworkTypes networkType)
        {
            NetworkType = networkType;
            ToggleComponentsForNetwork();
        }

        protected virtual void ToggleComponentsForNetwork()
        {
            /*
            if (!owner)
            {
                BTFiniteStateMachineManagerComponent.enabled = false;
            }
            else
            {
                BTFiniteStateMachineManagerComponent.enabled = true;
            }
            */

            BTFiniteStateMachineManagerComponent.IsSilent = NetworkType == NetworkTypes.Peer;
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
                return SqrInteractionMagnitude * personalSpacePercent;//.5f;
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

        public bool IsAtPosition()
        {
            if (FocusedOnTransform != null)
            {
                return IsAtPosition(FocusedOnTransform);
            }
            else if (FocusedOnPosition != null)
            {
                return IsAtPosition(FocusedOnPosition.Value);
            }
            return false;
        }

        public bool IsWithinInteractionDistance(float percent = 1.0f)
        {
            if (FocusedOnTransform != null)
            {
                return IsWithinInteractionDistance(FocusedOnTransform, percent);
            }
            else if (FocusedOnPosition != null)
            {
                return IsWithinInteractionDistance(FocusedOnPosition.Value, percent);
            }
            return false;
        }

        public bool IsWithinPersonalSpace(float percent = 1.0f)
        {
            if (FocusedOnTransform != null)
            {
                return IsWithinPersonalSpace(FocusedOnTransform, percent);
            }
            else if (FocusedOnPosition != null)
            {
                return IsWithinPersonalSpace(FocusedOnPosition.Value, percent);
            }
            return false;
        }

        public bool IsWithinAwarenessDistance(float percent = 1.0f)
        {
            if (FocusedOnTransform != null)
            {
                return IsWithinAwarenessDistance(FocusedOnTransform, percent);
            }
            else if (FocusedOnPosition != null)
            {
                return IsWithinAwarenessDistance(FocusedOnPosition.Value, percent);
            }
            return false;
        }

        public bool IsAtPosition(Vector3 position)
        {
            if(SqrAtPositionErrorMargin == 0)
            {
                return transform.position == position;
            }
            return ProximityHelpers.IsWithinDistance(ColliderComponent, position, SqrAtPositionErrorMargin);
        }

        public bool IsWithinInteractionDistance(Vector3 position, float percent = 1.0f)
        {
            return ProximityHelpers.IsWithinDistance(ColliderComponent, position, SqrInteractionMagnitude * percent);
        }

        public bool IsWithinPersonalSpace(Vector3 position, float percent = 1.0f)
        {
            return ProximityHelpers.IsWithinDistance(ColliderComponent, position, SqrPersonalSpaceMagnitude * percent);
        }

        public bool IsWithinAwarenessDistance(Vector3 position, float percent = 1.0f)
        {           
            return ProximityHelpers.IsWithinDistance(ColliderComponent, position, SqrAwarenessMagnitude * percent);
        }

        public bool IsWithinInteractionDistance(Transform target, float percent = 1.0f)
        {
            return IsWithinInteractionDistance(target.position, percent);
        }

        public bool IsWithinPersonalSpace(Transform target, float percent = 1.0f)
        {
            return IsWithinPersonalSpace(target.position, percent);
        }

        public bool IsWithinAwarenessDistance(Transform target, float percent = 1.0f)
        {
            return IsWithinAwarenessDistance(target.position, percent);
        }

        public bool IsAtPosition(Transform target)
        {
            return IsAtPosition(target.position);
        }

        #endregion IsWithinDistance

        public void ResetInteractionCooldown()
        {
            CanInteractFromTime = Time.time + InteractableCooldown;
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
        
        public bool AttractMyAttention_ToTransform(Transform target, bool force, StatesInteraction interactionState = StatesInteraction.Interactor)
        {

            if (IsWithinInteractionDistance(target) || force)
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

            if (IsWithinInteractionDistance(target.transform) || target.IsWithinInteractionDistance(transform) || force)
            {
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

        public void StartForgetTransform(Transform target)
        {
            if (forgetTransformTimeout > 0)
            {
                StartCoroutine(DelayedForgetTransform(target));

            }
            /*
            else if (forgetTransformTimeout < 0)
            {
                NoticeTransform(target);
            }*/
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
            if (DebugMode)
            {
                Debug.Log($"{transform.name}.FocusOnTransform: {target.name}");
            }
            NoticeTransform(target);
            FocusedOnTransform = target;

        }

        public bool UnFocus()
        {
            if (FocusedOnTransform == null)
            {
                return false;
            }
            if (DebugMode)
            {
                Debug.Log($"{transform.name}.UnFocus: {FocusedOnTransform.name}");
            }
            StartForgetTransform(FocusedOnTransform);
            FocusedOnTransform = null;
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

        public bool CanInteract()
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

        void OnDrawGizmos()
        {
#if UNITY_EDITOR          
            ProximityHelpers.DrawGizmoProximity(transform, SqrAwarenessMagnitude, IsWithinAwarenessDistance());
            ProximityHelpers.DrawGizmoProximity(transform, SqrInteractionMagnitude, IsWithinInteractionDistance());
            ProximityHelpers.DrawGizmoProximity(transform, SqrPersonalSpaceMagnitude, IsWithinPersonalSpace());

            if (FocusedOnTransform != null)
            {
                UnityEditor.Handles.color = Color.yellow;
                UnityEditor.Handles.DrawLine(transform.position, FocusedOnTransform.position);
                DrawGizmoPositionPoint(FocusedOnTransform.position);
            }
            else if (FocusedOnPosition != null)
            {
                UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
                UnityEditor.Handles.DrawSolidDisc(FocusedOnPosition.Value, Vector3.up, 0.25f);
                DrawGizmoPositionPoint(FocusedOnPosition.Value);
            }
#endif
        }

        #endregion MonoBehaviour Functions

        protected void DrawGizmoPositionPoint(Vector3 position)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.color = new Color(1f, 1f, 0.008f, 0.55f);
            UnityEditor.Handles.DrawSolidDisc(position, Vector3.up, 0.25f);
#endif
        }

    }
}
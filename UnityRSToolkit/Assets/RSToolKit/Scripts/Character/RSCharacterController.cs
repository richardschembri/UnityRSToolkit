using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RSToolkit.Animation;
using RSToolkit.Helpers;

namespace RSToolkit.Character
{
    public abstract class RSCharacterController : RSMonoBehaviour
    {
        #region Settings
        [System.Serializable]
        public struct RSSettingsLocomotion
        {
            public float Acceleration; // In meters/second
            public float Decceleration; // In meters/second
            public float MaxSpeedHorizontal; // In meters/second
            public float JumpSpeed; // In meters/second
            public float JumpAbortSpeed; // In meters/second
            public RSSettingsLocomotion(
                    float acceleration = 25.0f,
                    float decceleration = 25.0f,
                    float maxSpeedHorizontal = 8.0f,
                    float jumpSpeed = 10.0f,
                    float jumpAbortSpeed = 10.0f
                )
            {
                Acceleration = acceleration; // In meters/second
                Decceleration = decceleration; // In meters/second
                MaxSpeedHorizontal = maxSpeedHorizontal; // In meters/second
                JumpSpeed = jumpSpeed; // In meters/second
                JumpAbortSpeed = jumpAbortSpeed; // In meters/second

            }
        }

        [System.Serializable]
        public struct RSSettingsGravity
        {
            public float Gravity; // Gravity applied when the player is airborne
            public float GroundedGravity; // A constant gravity that is applied when the player is grounded
            public float MaxSpeedFall; // The max speed at which the player can fall
            public RSSettingsGravity(
                float gravity = 20.0f,
                float groundedGravity = 5.0f,
                float maxSpeedFall = 40.0f
                )
            {
                Gravity = gravity;
                GroundedGravity = groundedGravity;
                MaxSpeedFall = maxSpeedFall;
            }
        }

        [System.Serializable]
        public struct RSSettingsRotation
        {
            public enum ERotationType
            {
                MANUAL,
                TO_MOVEMENT
            }

            [Header("Manual Rotation")]
            public float MinAnglePitch;
            public float MaxAnglePitch;

            public ERotationType RotationType;
            public float MinSpeedRotation; // The turn speed when the player is at max speed (in degrees/second)
            public float MaxSpeedRotation; // The turn speed when the player is stationary (in degrees/second)

            public RSSettingsRotation(
                float minAnglePitch = -45.0f,float maxAnglePitch = 75.0f,
                ERotationType rotationType = ERotationType.TO_MOVEMENT,
                float minSpeedRotation = 600.0f, float maxSpeedRotation = 1200.0f)
            {
                MinAnglePitch = minAnglePitch;
                MaxAnglePitch = maxAnglePitch;

                RotationType = rotationType;
                MinSpeedRotation = minSpeedRotation;
                MaxSpeedRotation = maxSpeedRotation;
            }
        }

        #endregion Settings

        public string DisplayName;
        public class RSCharacterEvent :UnityEvent<RSCharacterController> {}
        public Animator AnimatorComponent {get; private set;}

        public RSSettingsLocomotion SettingsLocomotion = new RSSettingsLocomotion( .0f, 25.0f, 8.0f, 10.0f, 10.0f);
        public RSSettingsGravity SettingsGravity = new RSSettingsGravity( 20.0f, 5.0f, 40.0f );

        [SerializeField]
        protected RSPlayerInputManager _playerInputManager;

        public bool IsGrounded { get; protected set; }
        #region Current Speed Horizontal
        protected float _targetSpeedHorizontal;
        private float _currentSpeedHorizontal;
        public float CurrentSpeedHorizontal {
            get{
                return _currentSpeedHorizontal;
            } 
            protected set{
                _currentSpeedHorizontal = Mathf.Min(value, SettingsLocomotion.MaxSpeedHorizontal);
            }
        }

        public float CurrentSpeedPercentHorizontal {
            get{
                return MathHelpers.GetValuePercent(CurrentSpeedHorizontal, SettingsLocomotion.MaxSpeedHorizontal);
            }
        }
        #endregion Current Speed Horizontal

        public float CurrentSpeedVertical {
            get;
            protected set;
        }

        public virtual void Move(){
        }

        public virtual void Move(Vector2 directionAxis, float speed){
            CurrentSpeedHorizontal = speed;
            DirectionAxis = directionAxis;
        }

        #region Update Speed
        public Vector3 DirectionAxis { get; protected set; }
        protected virtual void UpdateHorizontalSpeed()
        {
            if(_playerInputManager != null){
                DirectionAxis = _playerInputManager.DirectionAxis;
            }
            if (DirectionAxis .sqrMagnitude > 1.0f)
            {
                DirectionAxis.Normalize();
            }

            _targetSpeedHorizontal = DirectionAxis.magnitude * SettingsLocomotion.MaxSpeedHorizontal;
            float acceleration = _playerInputManager != null && _playerInputManager.HasDirectionInput ? SettingsLocomotion.Acceleration : SettingsLocomotion.Decceleration;

            CurrentSpeedHorizontal = Mathf.MoveTowards(CurrentSpeedHorizontal, _targetSpeedHorizontal, acceleration * Time.deltaTime);
        }

        protected virtual void UpdateVerticalSpeed()
        {
            if (IsGrounded)
            {
                CurrentSpeedVertical = -SettingsGravity.GroundedGravity;
                if (_playerInputManager != null && _playerInputManager.JumpInput)
                {
                    CurrentSpeedVertical = SettingsLocomotion.JumpSpeed;
                    IsGrounded = false;
                }
            }
            else
            {

                if ((_playerInputManager != null && !_playerInputManager.JumpInput)
                                                        && CurrentSpeedVertical > 0.0f)
                {
                    CurrentSpeedVertical  = Mathf.MoveTowards(CurrentSpeedVertical, -SettingsGravity.MaxSpeedFall, SettingsLocomotion.JumpAbortSpeed * Time.deltaTime);
                }

                CurrentSpeedVertical = Mathf.MoveTowards(CurrentSpeedVertical, -SettingsGravity.MaxSpeedFall, SettingsGravity.Gravity * Time.deltaTime);
            }
        }
        #endregion Update Speed

        protected virtual void UpdateAnimationParams()
        {
            CharacterAnimParams.TrySetSpeedPercent(AnimatorComponent, Mathf.Abs(CurrentSpeedPercentHorizontal));
        }

        protected virtual void UpdateRotation()
        {

        }

        protected Vector3 GetLocomotionDirection()
        {
            Vector3 result = Vector3.zero;
            if(_playerInputManager != null){
                result = _playerInputManager.HasDirectionInput ? _playerInputManager.DirectionAxis : _playerInputManager.LastDirectionAxis;
                if (result.sqrMagnitude > 1f)
                {
                    result.Normalize();
                }
            }

            return result;
        }


        #region RSMonoBehaviour Functions
        protected override void InitComponents()
        {
            base.InitComponents();
            AnimatorComponent = GetComponent<Animator>();
        }
        public override bool Init(bool force = false)
        {
            if(base.Init(force)){
                if(string.IsNullOrEmpty(DisplayName)){
                    DisplayName = gameObject.name;
                }
                return true;
            }
            return false;
        }
        #endregion RSMonoBehaviour Functions

        #region MonoBehaviour Functions
        protected virtual void Update(){
            _playerInputManager?.UpdateInput();
            UpdateHorizontalSpeed();
            UpdateVerticalSpeed();
            Move();
            UpdateRotation();
            UpdateAnimationParams();
        }
        #endregion MonoBehaviour Functions
    }
}

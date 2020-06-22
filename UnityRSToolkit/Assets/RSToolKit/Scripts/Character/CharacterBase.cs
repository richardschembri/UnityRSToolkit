using System.Collections;
using System.Collections.Generic;
using RSToolkit.Character;
using UnityEngine;

namespace RSToolkit.Character
{
    public class CharacterBase : MonoBehaviour
    {
        [System.Serializable]
        public struct MovementSettings
        {
            public float AccelerationMPS; // In meters/second
            public float DecelerationMPS; // In meters/second
            public float MaxHorizontalSpeedMPS; // In meters/second
            public float JumpSpeedMPS; // In meters/second
            public float JumpAbortSpeedMPS; // In meters/second
            public MovementSettings(float accelerationMPS = 25.0f, float decelerationMPS = 25.0f, float maxHorizontalSpeedMPS = 8.0f,
                                    float jumpSpeedMPS = 10.0f, float jumpAbortSpeedMPS = 10.0f){
                this.AccelerationMPS = accelerationMPS;
                this.DecelerationMPS = decelerationMPS;
                this.MaxHorizontalSpeedMPS = maxHorizontalSpeedMPS;
                this.JumpSpeedMPS = jumpSpeedMPS;
                this.JumpAbortSpeedMPS = jumpAbortSpeedMPS;
            }
        }

        [System.Serializable]
        public struct GravitySettings
        {
            public float Gravity; // Gravity applied when the player is airborne
            public float GroundedGravity; // A constant gravity that is applied when the player is grounded
            public float MaxFallSpeed; // The max speed at which the player can fall
            public GravitySettings(float gravity = 20.0f, float groundedGravity = 5.0f, float maxFallSpeed = 40.0f)
            {
                this.Gravity = gravity;
                this.GroundedGravity = groundedGravity;
                this.MaxFallSpeed = maxFallSpeed;
            }
        }

        [System.Serializable]
        public class RotationSettings
        {
            [Header("Control Rotation")]
            public float MinPitchAngle = -45.0f;
            public float MaxPitchAngle = 75.0f;

            [Header("Character Orientation")]
            [SerializeField] private bool m_useControlRotation = false;
            [SerializeField] private bool m_orientRotationToMovement = true;
            public float MinRotationSpeed = 600.0f; // The turn speed when the player is at max speed (in degrees/second)
            public float MaxRotationSpeed = 1200.0f; // The turn speed when the player is stationary (in degrees/second)

            public bool UseControlRotation { get { return m_useControlRotation; } set { SetUseControlRotation(value); } }
            public bool OrientRotationToMovement { get { return m_orientRotationToMovement; } set { SetOrientRotationToMovement(value); } }

            private void SetUseControlRotation(bool useControlRotation)
            {
                m_useControlRotation = useControlRotation;
                m_orientRotationToMovement = !m_useControlRotation;
            }

            private void SetOrientRotationToMovement(bool orientRotationToMovement)
            {
                m_orientRotationToMovement = orientRotationToMovement;
                m_useControlRotation = !m_orientRotationToMovement;
            }
        }

        public RSCharacterController CharacterController; // The controller that controls the character
        public MovementSettings CharacterMovementSettings;
        public GravitySettings CharacterGravitySettings;
        public RotationSettings CharacterRotationSettings;

        private CharacterController _characterController; // The Unity's CharacterController
        private CharacterAnimator _characterAnimator;
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

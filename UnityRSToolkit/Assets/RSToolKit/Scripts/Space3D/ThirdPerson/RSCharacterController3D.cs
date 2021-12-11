using RSToolkit.Character;
using UnityEngine;

namespace RSToolkit.Space3D.ThirdPerson
{
    [RequireComponent(typeof(CharacterController))]
    public class RSCharacterController3D : RSCharacterController
    {
        private CharacterController _characterController;
        public RSSettingsRotation SettingsRotation = new RSSettingsRotation(
                -45.0f, 75.0f,
                RSSettingsRotation.ERotationType.TO_MOVEMENT,
                600.0f, 1200.0f
            );
        private Vector3 _locomotionAxis = Vector3.zero;
        private Vector3 _locomotionAxisHorizontal = Vector3.zero;
        private Vector2 _manualRotation = Vector3.zero;
        private Quaternion _targetRotation;

        public override void Move(){
            _locomotionAxis = CurrentSpeedHorizontal * GetLocomotionDirection() + CurrentSpeedVertical * Vector3.up;
            _characterController.Move(_locomotionAxis * Time.deltaTime);
        }
        protected override void UpdateRotation()
        {
            _locomotionAxisHorizontal = new Vector3(_locomotionAxis.x, 0f, _locomotionAxis.z);
            switch (SettingsRotation.RotationType)
            {
                case RSSettingsRotation.ERotationType.TO_MOVEMENT when _locomotionAxisHorizontal.sqrMagnitude > 0.0f:
                    float rotationSpeed = Mathf.Lerp(
                        SettingsRotation.MaxSpeedRotation, SettingsRotation.MinSpeedRotation, CurrentSpeedHorizontal / _targetSpeedHorizontal);

                    _targetRotation = Quaternion.LookRotation(_locomotionAxisHorizontal, Vector3.up);

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, rotationSpeed * Time.deltaTime);
                    break;
                case RSSettingsRotation.ERotationType.MANUAL:
                    _targetRotation = Quaternion.Euler(0.0f, _manualRotation.y, 0.0f);
                    transform.rotation = _targetRotation;
                    break;
            }
        }

        public void SetManualRotation(Vector2 controlRotation)
        {
            // Adjust the pitch angle (X Rotation)
            float pitchAngle = controlRotation.x;
            pitchAngle %= 360.0f;
            pitchAngle = Mathf.Clamp(pitchAngle, SettingsRotation.MinAnglePitch, SettingsRotation.MaxAnglePitch);

            // Adjust the yaw angle (Y Rotation)
            float yawAngle = controlRotation.y;
            yawAngle %= 360.0f;

            _manualRotation = new Vector2(pitchAngle, yawAngle);
        }

        protected override Vector3 GetPlayerInputDirectionAxis()
        {
            return (_playerInputManager.HasDirectionInput ? _playerInputManager.DirectionAxis3D : _playerInputManager.LastDirectionAxis3D);
        }

        #region RSMonoBehaviour Functions
        protected override void InitComponents()
        {
            base.InitComponents();
            _characterController = GetComponent<CharacterController>();
        }
        #endregion RSMonoBehaviour Functions
    }
}

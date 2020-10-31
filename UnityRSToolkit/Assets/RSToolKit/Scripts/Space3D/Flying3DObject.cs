using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit.Space3D
{
    [System.Serializable]
    public class FlightAxis
    {
        public float pitch; // negative = down, positive = up;
        public float yaw; // negative = left, negative = right 
        public float roll; // negative = left, negative = right

        public FlightAxis(float pitch = 0, float yaw = 0, float roll = 0)
        {
            this.pitch = pitch;
            this.yaw = yaw;
            this.roll = roll;

        }

        public Vector3 toVector3()
        {
            return new Vector3(pitch, yaw, roll);
        }
    }

    [System.Serializable]
    public class FlightInputControls
    {
        public KeyCode VerticalThrustPositive = KeyCode.I;
        public KeyCode VerticalThrustNegative = KeyCode.K;

        public KeyCode LateralThrustPositive = KeyCode.A;
        public KeyCode LateralThrustNegative = KeyCode.D;


        public KeyCode ForwardThrustPositive = KeyCode.W;
        public KeyCode ForwardThrustNegative = KeyCode.S;

        public KeyCode YawPositive = KeyCode.L;
        public KeyCode YawNegative = KeyCode.J;

        public KeyCode ToggleHover = KeyCode.Space;


#region GetKey

        private bool GetFlightKey(bool positive, KeyCode postiveKey, KeyCode negativeKey)
        {
            return positive ? Input.GetKey(postiveKey) : Input.GetKey(negativeKey);
        }

        public bool GetVerticalThrustKey(bool positive)
        {
            return GetFlightKey(positive, VerticalThrustPositive, VerticalThrustNegative);
        }

        public bool GetLateralThrustKey(bool positive)
        {
            return GetFlightKey(positive, LateralThrustPositive, LateralThrustNegative);
        }

        public bool GetForwardThrustKey(bool positive)
        {
            return GetFlightKey(positive, ForwardThrustPositive, ForwardThrustNegative);
        }

        public bool GetYawKey(bool positive)
        {
            return GetFlightKey(positive, YawPositive, YawNegative);
        }

        public bool GetToggleHoverKey()
        {
            return Input.GetKeyUp(ToggleHover);
        }

#endregion GetKey

#region Key Down

        public bool IsVerticalThrustDown()
        {
            return Input.GetKey(VerticalThrustPositive) || Input.GetKey(VerticalThrustNegative);
        }

        public bool IsLateralThrustDown()
        {
            return Input.GetKey(LateralThrustPositive) || Input.GetKey(LateralThrustNegative);
        }

        public bool IsForwardThrustDown()
        {
            return Input.GetKey(ForwardThrustPositive) || Input.GetKey(ForwardThrustNegative);
        }

        public bool IsYawDown()
        {
            return Input.GetKey(YawPositive) || Input.GetKey(YawNegative);
        }

#endregion Key Down

    }

    [RequireComponent(typeof(Rigidbody))]
    public class Flying3DObject : MonoBehaviour
    {
        private Rigidbody _rigidBodyComponent;
        [SerializeField]
        private FlightAxis DefaultFlightAxis = new FlightAxis();
        public FlightAxis CurrentFlightAxis { get; private set; } = new FlightAxis();
        [SerializeField]
        private FlightAxis MovementFlightAxis = new FlightAxis(20f, 2.5f, 20f);

        private FlightAxis _targetFlightAxis = new FlightAxis();

        // Thrust: X = Lateral // Y = Vertical // Z = Forward 
        [SerializeField]
        public Vector3 HoverFlightThrust = new Vector3(0f, 98.1f, 0f);

        [SerializeField]
        public Vector3 MovementFlightThrust = new Vector3(300f, 450f, 250f); // new FlightForces(450f, 0f, 0f, 500f); // 200f negative lift
        public Vector3 CurrentFlightThrust = new Vector3(0f, 0f, 0f);
        // private Vector3 m_targetFlightThrust = new Vector3(0f, 0f, 0f);

        public bool ManualControl = true;

        public bool FlightEnabled = true;

        //[SerializeField]
        //private Vector3 TargetFlightThrust = new Vector3();

        public float rollVerticalThrust = 281f;

        public float verticalInputDeadzone = 0.2f;
        public float horizontalInputDeadzone = 0.2f;

        public FlightInputControls InputControls;

        public float VerticalDeadzone = 0.2f;
        public float LateralDeadzone = 0.2f;

        public bool HoverWhenIdle = true;
        public bool IsGrounded { get; private set; } = false;

#region IsOutDeadzone

        public bool IsOutDeadzoneVertical()
        {
            return Mathf.Abs(Input.GetAxis("Vertical")) > VerticalDeadzone;
        }
        public bool IsOutDeadzoneLateral()
        {
            return Mathf.Abs(Input.GetAxis("Horizontal")) > LateralDeadzone;
        }

        public bool IsOutDeadzone()
        {
            return IsOutDeadzoneVertical() || IsOutDeadzoneLateral();
        }

#endregion IsOutDeadzone

#region Reset Functions

/*
        public void ResetAppliedAxis()
        {
            CurrentFlightAxis.pitch = DefaultFlightAxis.pitch;
            CurrentFlightAxis.yaw = DefaultFlightAxis.yaw;
            CurrentFlightAxis.roll = DefaultFlightAxis.roll;
        }
*/

        public void ResetAppliedForces()
        {
            if (HoverWhenIdle)
            {
                CurrentFlightThrust.x = HoverFlightThrust.x;
                CurrentFlightThrust.y = HoverFlightThrust.y;
                CurrentFlightThrust.z = HoverFlightThrust.z;
            }
            else
            {
                CurrentFlightThrust = Vector3.zero;
            }

        }

        public void ResetAppliedValues()
        {
            // ResetAppliedAxis();
            ResetAppliedForces();
        }

        public void ResetRigidBodyForces()
        {
            RigidBodyComponent.velocity = Vector3.zero;
            RigidBodyComponent.angularVelocity = Vector3.zero;
			RigidBodyComponent.rotation =  Quaternion.identity;
        }

#endregion Reset Functions

        public Rigidbody RigidBodyComponent
        {
            get
            {
                if (_rigidBodyComponent == null)
                {
                    _rigidBodyComponent = GetComponent<Rigidbody>();
                }
                return _rigidBodyComponent;
            }
        }

        public float CurrentSpeed
        {
            get
            {
                return RigidBodyComponent.velocity.magnitude;
            }
        }

        float m_decelerateVelocity;

        public bool HasLateralVelocity()
        {
            return RigidBodyComponent.velocity.x != 0 || RigidBodyComponent.velocity.z != 0;
        }

        public bool HasVerticalVelocity()
        {
            return RigidBodyComponent.velocity.y != 0;
        }

        public bool IsMovingHorizontally()
        {
            return Mathf.Abs(CurrentFlightThrust.x) > 1 || Mathf.Abs(CurrentFlightThrust.z) > 1;
        }

        public (KeyCode, KeyCode) VerticalThrustKeys;

        public void EnabledFlight()
        {
            FlightEnabled = true;
        }
        public void DisableFlight()
        {
            FlightEnabled = false;
        }

        void ManualVerticalThrustControl()
        {
            if (IsOutDeadzone())
            {
                if (!InputControls.IsVerticalThrustDown() && !InputControls.IsYawDown())
                {
                    RigidBodyComponent.velocity = new Vector3(RigidBodyComponent.velocity.x,
                        Mathf.Lerp(RigidBodyComponent.velocity.y, 0, Time.deltaTime * 5), RigidBodyComponent.velocity.z);

                    // CurrentFlightThrust.y = rollVerticalThrust;
                }

                if (InputControls.IsYawDown())
                {
                    CurrentFlightThrust.y = 410f;
                }
            }
            if (!IsOutDeadzoneVertical() || IsOutDeadzoneLateral())
            {
                //CurrentFlightThrust.y = 135f;
            }

            if (InputControls.GetVerticalThrustKey(true))
            {
                ApplyVerticalThrust(true);
            }
            else if (InputControls.GetVerticalThrustKey(false))
            {
                ApplyVerticalThrust(false); //.lift; // MovementFlightForces.weight;
            }
        }

        public void ApplyVerticalThrust(bool positive)
        {
            if (positive)
            {
                CurrentFlightThrust.y = MovementFlightThrust.y; //.lift;
                if (IsOutDeadzoneLateral())
                {
                    CurrentFlightThrust.y = 500f;
                }
            }
            else
            {
                CurrentFlightThrust.y = ManualControl ? HoverFlightThrust.y * .5f : HoverFlightThrust.y * .75f; // - MovementFlightThrust.y;
            }
        }

#region Update Functions
#region Update Thrust

        private void UpdateVerticalThrust()
        {
            if (HoverWhenIdle)
            {
                CurrentFlightThrust.y = HoverFlightThrust.y;
            }
            else
            {
                CurrentFlightThrust.y = 0;
            }
            _targetFlightAxis.pitch = 0;
        }

        private void UpdateForwardThrust()
        {
            if (HoverWhenIdle)
            {
                CurrentFlightThrust.z = HoverFlightThrust.z;
            }
            else
            {
                CurrentFlightThrust.z = 0;
            }
        }

#endregion Update Thrust

#region Update Axis

        private void UpdatePitch()
        {
            if (CurrentFlightAxis.pitch != _targetFlightAxis.pitch)
            {
                CurrentFlightAxis.pitch = Mathf.SmoothDamp(CurrentFlightAxis.pitch, _targetFlightAxis.pitch, ref m_forwardVelocity, 0.1f);
            }
        }

        private float _lateralVelocity;
        private void UpdateRoll()
        {
            if (CurrentFlightAxis.roll != _targetFlightAxis.roll)
            {
                CurrentFlightAxis.roll = Mathf.SmoothDamp(CurrentFlightAxis.roll, _targetFlightAxis.roll, ref _lateralVelocity, 0.1f);
            }
        }

        private void UpdateYaw()
        {
            if (CurrentFlightAxis.yaw != _targetFlightAxis.yaw)
            {
                CurrentFlightAxis.yaw = Mathf.SmoothDamp(CurrentFlightAxis.yaw, _targetFlightAxis.yaw, ref _yawVelocity, 0.25f);

            }
        }

#endregion Update Axis
#endregion Update Functions

#region Apply Functions

        public void ApplyForwardThrust(float percent)
        {
            if (percent > 1f)
            {
                percent = 1f;
            }
            else if (percent < -1f)
            {
                percent = -1f;
            }

            CurrentFlightThrust.z = percent * MovementFlightThrust.z;
            _targetFlightAxis.pitch = MovementFlightAxis.pitch * percent;
        }

        public void ApplyYaw(bool positive)
        {
            if (positive)
            {
                _targetFlightAxis.yaw += MovementFlightAxis.yaw;
            }
            else
            {
                _targetFlightAxis.yaw -= MovementFlightAxis.yaw;
            }

        }

        public void ApplyLateralThrust(float percent)
        {
            if (percent > 1f)
            {
                percent = 1f;
            }
            else if (percent < -1f)
            {
                percent = -1f;
            }

            CurrentFlightThrust.x = percent * MovementFlightThrust.x;
            if (Mathf.Abs(percent) > LateralDeadzone)
            {
                _targetFlightAxis.roll = MovementFlightAxis.roll * -percent;
            }
            else
            {
                _targetFlightAxis.roll = 0;
            }
        }

        public void ApplyRoll(bool positive)
        {
            if (positive)
            {
                _targetFlightAxis.roll = MovementFlightAxis.roll;

            }
            else
            {
                _targetFlightAxis.roll = -MovementFlightAxis.roll;
            }
        }

#endregion Apply Functions

#region Manual Control

        private float m_forwardVelocity;
        void ManualForwardMovementControl()
        {
            ApplyForwardThrust(Input.GetAxis("Vertical"));
        }

        void ManualLateralMovementControl()
        {
            ApplyLateralThrust(Input.GetAxis("Horizontal"));

        }

        private float _yawVelocity;
        void ManualYawControl()
        {
            if (InputControls.GetYawKey(true))
            {
                ApplyYaw(true);
            }
            if (InputControls.GetYawKey(false))
            {
                ApplyYaw(false);
            }
        }

#endregion Manual Control

        public void YawTo(float y)
        {
            _targetFlightAxis.yaw = y;
        }

        Vector3 _clampedVelocity;
        void ClampForces()
        {
            if (ManualControl && IsOutDeadzone())
            {
                float magnitudeLength = 10f;
                if (!IsOutDeadzoneVertical() && IsOutDeadzoneLateral())
                {
                    magnitudeLength = 5f;
                }
                RigidBodyComponent.velocity = Vector3.ClampMagnitude(RigidBodyComponent.velocity, Mathf.Lerp(RigidBodyComponent.velocity.magnitude, magnitudeLength, Time.deltaTime * 5f));
            }
            else if (!ManualControl && CurrentFlightThrust.x == 0f && CurrentFlightThrust.z == 0f)
            {
                RigidBodyComponent.velocity = Vector3.SmoothDamp(RigidBodyComponent.velocity, Vector3.zero, ref _clampedVelocity, 0.25f);
            }
            else
            {
                RigidBodyComponent.velocity = Vector3.SmoothDamp(RigidBodyComponent.velocity, Vector3.zero, ref _clampedVelocity, 0.95f);
            }
        }

#region Mono Functions

        void Awake()
        {
            ResetAppliedForces();
        }

        void Update()
        {
            if (InputControls.GetToggleHoverKey())
            {
                HoverWhenIdle = !HoverWhenIdle;
            }
        }

        void FixedUpdate()
        {
            if (!FlightEnabled)
            {
                return;
            }

            if (ManualControl)
            {
                ManualVerticalThrustControl();
                ManualForwardMovementControl();
                ManualLateralMovementControl();
                ManualYawControl();
            }

            UpdateYaw();
            UpdateRoll();
            UpdatePitch();
            ClampForces();

            RigidBodyComponent.AddRelativeForce(CurrentFlightThrust);

            RigidBodyComponent.rotation = Quaternion.Euler(CurrentFlightAxis.toVector3()); // new Vector3(CurrentFlightAxis.pitch, CurrentFlightAxis.yaw, m_RigidBodyComponent.rotation.z));
            UpdateForwardThrust();
            UpdateVerticalThrust();
        }

        void OnCollisionStay(Collision collision)
        {
            IsGrounded = false;
            // To optimize. Use GetContacts.
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.point.y < transform.position.y)
                {
                    // print("Grounded");
                    IsGrounded = true;
                }

            }
        }

#endregion Mono Functions

    }
}

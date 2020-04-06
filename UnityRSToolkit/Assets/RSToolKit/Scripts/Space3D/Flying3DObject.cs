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

        #region Down
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
        #endregion

    }

    [RequireComponent(typeof(Rigidbody))]
    public class Flying3DObject : MonoBehaviour
    {
        private Rigidbody m_rigidBodyComponent;
        [SerializeField]
        private FlightAxis DefaultFlightAxis = new FlightAxis();
        public FlightAxis CurrentFlightAxis { get; private set; } = new FlightAxis();
        [SerializeField]
        private FlightAxis MovementFlightAxis = new FlightAxis(20f, 2.5f, 20f);

        private FlightAxis m_targetFlightAxis = new FlightAxis();

        // Thrust: X = Lateral // Y = Vertical // Z = Forward 
        [SerializeField]
        public Vector3 HoverFlightThrust = new Vector3(0f, 98.1f, 0f);

        [SerializeField]
        public Vector3 MovementFlightThrust = new Vector3(300f, 450f, 250f); // new FlightForces(450f, 0f, 0f, 500f); // 200f negative lift
        public Vector3 CurrentFlightThrust = new Vector3(0f, 0f, 0f);
        // private Vector3 m_targetFlightThrust = new Vector3(0f, 0f, 0f);

        public bool ManualControl = true;

        public bool m_FlightEnabled = true;

        //[SerializeField]
        //private Vector3 TargetFlightThrust = new Vector3();

        public float rollVerticalThrust = 281f;

        public float verticalInputDeadzone = 0.2f;
        public float horizontalInputDeadzone = 0.2f;

        public FlightInputControls InputControls;

        public float VerticalDeadzone = 0.2f;
        public float LateralDeadzone = 0.2f;

        public bool HoverWhenIdle = true;
        

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
      
        public void ResetAppliedAxis()
        {
            CurrentFlightAxis.pitch = DefaultFlightAxis.pitch;
            CurrentFlightAxis.yaw = DefaultFlightAxis.yaw;
            CurrentFlightAxis.roll = DefaultFlightAxis.roll;
        }

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
            ResetAppliedAxis();
            ResetAppliedForces();
        }


        private Rigidbody m_RigidBodyComponent
        {
            get
            {
                if (m_rigidBodyComponent == null)
                {
                    m_rigidBodyComponent = GetComponent<Rigidbody>();
                }
                return m_rigidBodyComponent;
            }
        }

        public float CurrentSpeed
        {
            get
            {
                return m_RigidBodyComponent.velocity.magnitude;
            }
        }

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
        float m_decelerateVelocity;
        void FixedUpdate()
        {
            if (!m_FlightEnabled)
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

            m_RigidBodyComponent.AddRelativeForce(CurrentFlightThrust);

            m_RigidBodyComponent.rotation = Quaternion.Euler(CurrentFlightAxis.toVector3()); // new Vector3(CurrentFlightAxis.pitch, CurrentFlightAxis.yaw, m_RigidBodyComponent.rotation.z));
            UpdateForwardThrust();
            UpdateVerticalThrust();

            
        }

        public bool IsMovingHorizontally()
        {
            return Mathf.Abs(CurrentFlightThrust.x) > 1 || Mathf.Abs(CurrentFlightThrust.z) > 1;
        }

        public (KeyCode, KeyCode) VerticalThrustKeys;

        public void EnabledFlight()
        {
            m_FlightEnabled = true;
        }
        public void DisableFlight()
        {
            m_FlightEnabled = false;
        }


        void ManualVerticalThrustControl()
        {
            if(IsOutDeadzone())
            {
                if(!InputControls.IsVerticalThrustDown() && !InputControls.IsYawDown())
                {
                    m_RigidBodyComponent.velocity = new Vector3(m_RigidBodyComponent.velocity.x, 
                        Mathf.Lerp(m_RigidBodyComponent.velocity.y, 0, Time.deltaTime * 5), m_RigidBodyComponent.velocity.z);

                    // CurrentFlightThrust.y = rollVerticalThrust;
                }

                if(InputControls.IsYawDown())
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
                CurrentFlightThrust.y =ManualControl ? HoverFlightThrust.y * .5f : HoverFlightThrust.y * .75f; // - MovementFlightThrust.y;
            }
        }

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
            m_targetFlightAxis.pitch = 0;
        }

        private float m_forwardVelocity;
        void ManualForwardMovementControl()
        {
            ApplyForwardThrust(Input.GetAxis("Vertical"));
        }

        public void ApplyForwardThrust(float percent)
        {
            if(percent > 1f)
            {
                percent = 1f;
            }
            else if( percent < -1f)
            {
                percent = -1f;
            }

            CurrentFlightThrust.z = percent * MovementFlightThrust.z;
            m_targetFlightAxis.pitch = MovementFlightAxis.pitch * percent;
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

        private void UpdatePitch()
        {
            if (CurrentFlightAxis.pitch != m_targetFlightAxis.pitch)
            {
                CurrentFlightAxis.pitch = Mathf.SmoothDamp(CurrentFlightAxis.pitch, m_targetFlightAxis.pitch, ref m_forwardVelocity, 0.1f);
            }
        }



        
        void ManualLateralMovementControl()
        {
            ApplyLateralThrust(Input.GetAxis("Horizontal"));

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
            if(Mathf.Abs(percent) > LateralDeadzone)
            {
                m_targetFlightAxis.roll = MovementFlightAxis.roll * -percent;
            }
            else
            {
                m_targetFlightAxis.roll = 0;
            }
            
        }

        public void ApplyRoll(bool positive)
        {
            if (positive)
            {
                m_targetFlightAxis.roll = MovementFlightAxis.roll;
        
            }
            else
            {
                m_targetFlightAxis.roll = -MovementFlightAxis.roll;
            }
           
        }
        private float m_lateralVelocity;
        private void UpdateRoll()
        {
            if(CurrentFlightAxis.roll != m_targetFlightAxis.roll)
            {
                CurrentFlightAxis.roll = Mathf.SmoothDamp(CurrentFlightAxis.roll, m_targetFlightAxis.roll, ref m_lateralVelocity, 0.1f);
            }
        }


        private float m_yawVelocity;
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

        public void ApplyYaw(bool positive)
        {
            if (positive)
            {
                m_targetFlightAxis.yaw += MovementFlightAxis.yaw;
            }
            else
            {
                m_targetFlightAxis.yaw -= MovementFlightAxis.yaw;
            }

        }

        private void UpdateYaw()
        {
            if(CurrentFlightAxis.yaw != m_targetFlightAxis.yaw)
            {
                CurrentFlightAxis.yaw = Mathf.SmoothDamp(CurrentFlightAxis.yaw, m_targetFlightAxis.yaw, ref m_yawVelocity, 0.25f);
                
            }
        }

        public void YawTo(float y)
        {
            m_targetFlightAxis.yaw = y;
        }

        Vector3 m_clampedVelocity;
        void ClampForces()
        {
            if (ManualControl && IsOutDeadzone())
            {
                float magnitudeLength = 10f;
                if (!IsOutDeadzoneVertical() && IsOutDeadzoneLateral())
                {
                    magnitudeLength = 5f;
                }
                m_RigidBodyComponent.velocity = Vector3.ClampMagnitude(m_RigidBodyComponent.velocity, Mathf.Lerp(m_RigidBodyComponent.velocity.magnitude, magnitudeLength, Time.deltaTime * 5f));
            }
            else
            {
                m_RigidBodyComponent.velocity = Vector3.SmoothDamp(m_RigidBodyComponent.velocity, Vector3.zero, ref m_clampedVelocity, 0.95f);
            }
        }

        public bool IsGrounded { get; private set; } = false;

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

    }
}
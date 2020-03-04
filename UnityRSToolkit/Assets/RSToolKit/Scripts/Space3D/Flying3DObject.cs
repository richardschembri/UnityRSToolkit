using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit.Space3D
{
    [System.Serializable]
    public class FlightAxis
    {
        public float pitch; // negative = down, positive = up;
        public float roll; // negative = left, negative = right
        public float yaw; // negative = left, negative = right 

        public FlightAxis(float pitch = 0, float roll = 0, float yaw = 0)
        {
            this.pitch = pitch;
            this.roll = roll;
            this.yaw = yaw;
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

        public FlightAxis DefaultFlightAxis = new FlightAxis();
        public FlightAxis CurrentFlightAxis { get; private set; } = new FlightAxis();
        public FlightAxis MovementFlightAxis = new FlightAxis(20f, 20f, 2.5f);

        // Thrust: X = Lateral // Y = Vertical // Z = Forward 
        public Vector3 DefaultFlightThrust = new Vector3(0f, 98.1f, 0f);
        public Vector3 MovementFlightThrust = new Vector3(300f, 450f, 250f); // new FlightForces(450f, 0f, 0f, 500f); // 200f negative lift
        public Vector3 CurrentFlightThrust = new Vector3(0f, 0f, 0f);

        public float verticalInputDeadzone = 0.2f;
        public float horizontalInputDeadzone = 0.2f;

        public FlightInputControls InputControls;

        public float VerticalDeadzone = 0.2f;
        public float LateralDeadzone = 0.2f;

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

        private float m_yawTo = 0f;
      
        public void ResetAppliedAxis()
        {
            CurrentFlightAxis = DefaultFlightAxis;
        }

        public void ResetAppliedForces()
        {
            // CurrentVerticalThrust = DefaultLift;
            // CurrentForwardThrust = DefaultThrust;
            CurrentFlightThrust.x = DefaultFlightThrust.x;
            CurrentFlightThrust.y = DefaultFlightThrust.y;
            CurrentFlightThrust.z = DefaultFlightThrust.z;
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

        void FixedUpdate()
        {
            ManualVerticalThrustControl();
            ManualForwardMovementControl();
            ManualLateralMovementControl();
            ManualYawControl();
            ClampForces();

            m_RigidBodyComponent.AddRelativeForce(CurrentFlightThrust);

            m_RigidBodyComponent.rotation = Quaternion.Euler(CurrentFlightAxis.toVector3()); // new Vector3(CurrentFlightAxis.pitch, CurrentFlightAxis.yaw, m_RigidBodyComponent.rotation.z));
        }

        public (KeyCode, KeyCode) VerticalThrustKeys;


        void ManualVerticalThrustControl()
        {
            if(IsOutDeadzone())
            {
                if(!InputControls.IsVerticalThrustDown() && !InputControls.IsYawDown())
                {
                    m_RigidBodyComponent.velocity = new Vector3(m_RigidBodyComponent.velocity.x, 
                        Mathf.Lerp(m_RigidBodyComponent.velocity.y, 0, Time.deltaTime * 5), m_RigidBodyComponent.velocity.z);

                    CurrentFlightThrust.y = InputControls.IsYawDown()? 110f : 281f;


                }

                if(InputControls.IsYawDown())
                {
                    CurrentFlightThrust.y = 410f;
                }
            }
            if (!IsOutDeadzoneVertical() || IsOutDeadzoneLateral())
            {
                CurrentFlightThrust.y = 135f;
            }

            if (InputControls.GetVerticalThrustKey(true))
            {
                CurrentFlightThrust.y = MovementFlightThrust.y; //.lift;
                if(IsOutDeadzoneLateral())
                {
                    CurrentFlightThrust.y = 500f;
                }
            }
            else if (InputControls.GetVerticalThrustKey(false))
            {
                CurrentFlightThrust.y = -MovementFlightThrust.y; //.lift; // MovementFlightForces.weight;
            }
            else
            {
                CurrentFlightThrust.y = DefaultFlightThrust.y;
            }
        }

        private float m_forwardVelocity;
        void ManualForwardMovementControl()
        {
            CurrentFlightThrust.z = Input.GetAxis("Vertical") * MovementFlightThrust.z; // .thrust;
            CurrentFlightAxis.pitch = Mathf.SmoothDamp(CurrentFlightAxis.pitch, MovementFlightAxis.pitch * Input.GetAxis("Vertical"), ref m_forwardVelocity, 0.1f);
        }

        private float m_lateralVelocity;
        void ManualLateralMovementControl()
        {
            CurrentFlightThrust.x = Input.GetAxis("Horizontal") * MovementFlightThrust.x;
            if (IsOutDeadzoneLateral())
            {
                CurrentFlightAxis.roll = Mathf.SmoothDamp(CurrentFlightAxis.roll, -MovementFlightAxis.roll * Input.GetAxis("Horizontal"), ref m_lateralVelocity, 0.1f);
            }
            else
            {
                CurrentFlightAxis.roll = Mathf.SmoothDamp(CurrentFlightAxis.roll, 0, ref m_lateralVelocity, 0.1f); // Reset
            }
        }


        private float m_yawVelocity;
        void ManualYawControl()
        {
            if (Input.GetKey(KeyCode.J))
            {
                ApplyYaw(true);
            }
            if (Input.GetKey(KeyCode.L))
            {
                ApplyYaw(false);
            }
        }

        public void ApplyYaw(bool negative = false)
        {
            if (negative)
            {
                m_yawTo -= MovementFlightAxis.yaw;
            }
            else
            {
                m_yawTo += MovementFlightAxis.yaw;
            }
            
            CurrentFlightAxis.yaw = Mathf.SmoothDamp(CurrentFlightAxis.yaw, m_yawTo, ref m_yawVelocity, 0.25f);
        }

        Vector3 m_clampedVelocity;
        void ClampForces()
        {
            if(IsOutDeadzoneVertical() && IsOutDeadzoneLateral())
            {
                m_RigidBodyComponent.velocity = Vector3.ClampMagnitude(m_RigidBodyComponent.velocity, Mathf.Lerp(m_RigidBodyComponent.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
            }
            if (IsOutDeadzoneVertical() && !IsOutDeadzoneLateral())
            {
                m_RigidBodyComponent.velocity = Vector3.ClampMagnitude(m_RigidBodyComponent.velocity, Mathf.Lerp(m_RigidBodyComponent.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
            }
            if (!IsOutDeadzoneVertical() && IsOutDeadzoneLateral())
            {
                m_RigidBodyComponent.velocity = Vector3.ClampMagnitude(m_RigidBodyComponent.velocity, Mathf.Lerp(m_RigidBodyComponent.velocity.magnitude, 5.0f, Time.deltaTime * 5f));
            }
            if (!IsOutDeadzoneVertical() && IsOutDeadzoneLateral())
            {
                m_RigidBodyComponent.velocity = Vector3.SmoothDamp(m_RigidBodyComponent.velocity, Vector3.zero, ref m_clampedVelocity, 0.95f);
            }
        }
    }
}
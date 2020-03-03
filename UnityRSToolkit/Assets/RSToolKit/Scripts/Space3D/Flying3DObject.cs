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

        private bool IsBeyondDeadzone_Vertical
        {
            get
            {
                return Mathf.Abs(Input.GetAxis("Vertical")) > verticalInputDeadzone;
            }
        }

        private bool IsBeyondDeadzone_Horizontal
        {
            get
            {
                return Mathf.Abs(Input.GetAxis("Horizontal")) > horizontalInputDeadzone;
            }
        }

        private bool IsBeyondDeadzone_AnyDirection
        {
            get
            {
                return IsBeyondDeadzone_Vertical || IsBeyondDeadzone_Horizontal;
            }
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

        void ManualVerticalThrustControl()
        {
            if(IsBeyondDeadzone_AnyDirection)
            {
                if(!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.J) && !Input.GetKey(KeyCode.L))
                {
                    m_RigidBodyComponent.velocity = new Vector3(m_RigidBodyComponent.velocity.x, Mathf.Lerp(m_RigidBodyComponent.velocity.y, 0, Time.deltaTime * 5), m_RigidBodyComponent.velocity.z);
                    CurrentFlightThrust.y = 281f;
                }
                if(!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && (Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.L)))
                {
                    m_RigidBodyComponent.velocity = new Vector3(m_RigidBodyComponent.velocity.x, Mathf.Lerp(m_RigidBodyComponent.velocity.y, 0, Time.deltaTime * 5), m_RigidBodyComponent.velocity.z);
                    CurrentFlightThrust.y = 110f;
                }
                if(Input.GetKey(KeyCode.J) && Input.GetKey(KeyCode.L))
                {
                    CurrentFlightThrust.y = 410f;
                }
            }
            if (!IsBeyondDeadzone_Vertical || IsBeyondDeadzone_Horizontal)
            {
                CurrentFlightThrust.y = 135f;
            }

            if (Input.GetKey(KeyCode.I))
            {
                CurrentFlightThrust.y = MovementFlightThrust.y; //.lift;
                if(IsBeyondDeadzone_Horizontal)
                {
                    CurrentFlightThrust.y = 500f;
                }
            }
            else if (Input.GetKey(KeyCode.K))
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
            if (IsBeyondDeadzone_Horizontal)
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
            if(IsBeyondDeadzone_Vertical && IsBeyondDeadzone_Horizontal)
            {
                m_RigidBodyComponent.velocity = Vector3.ClampMagnitude(m_RigidBodyComponent.velocity, Mathf.Lerp(m_RigidBodyComponent.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
            }
            if (IsBeyondDeadzone_Vertical && !IsBeyondDeadzone_Horizontal)
            {
                m_RigidBodyComponent.velocity = Vector3.ClampMagnitude(m_RigidBodyComponent.velocity, Mathf.Lerp(m_RigidBodyComponent.velocity.magnitude, 10.0f, Time.deltaTime * 5f));
            }
            if (!IsBeyondDeadzone_Vertical && IsBeyondDeadzone_Horizontal)
            {
                m_RigidBodyComponent.velocity = Vector3.ClampMagnitude(m_RigidBodyComponent.velocity, Mathf.Lerp(m_RigidBodyComponent.velocity.magnitude, 5.0f, Time.deltaTime * 5f));
            }
            if (!IsBeyondDeadzone_Vertical && IsBeyondDeadzone_Horizontal)
            {
                m_RigidBodyComponent.velocity = Vector3.SmoothDamp(m_RigidBodyComponent.velocity, Vector3.zero, ref m_clampedVelocity, 0.95f);
            }
        }
    }
}
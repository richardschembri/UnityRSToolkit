using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit.Space3D
{

    public struct FlightAxis
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
    }
    public struct FlightForces
    {
        public float lift; // Upward force
        public float weight; // Downward force 
        public float drag; // Backward force
        public float thrust; // Forward force

        public FlightForces(float lift = 0, float weight = 0, float drag = 0, float thrust = 0)
        {
            this.lift = lift;
            this.weight = weight;
            this.drag = drag;
            this.thrust = thrust;
        }

    }
    [RequireComponent(typeof(Rigidbody))]
    public class Flying3DObject : MonoBehaviour
    {
        private Rigidbody m_rigidBodyComponent;

        public FlightAxis DefaultFlightAxis = new FlightAxis();
        public FlightAxis CurrentFlightAxis { get; private set; } = new FlightAxis();

        public FlightForces MovementFlightForces = new FlightForces(450f, 200f, 0f, 500f);

        public float DefaultLift = 98.1f;
        public float CurrentLift { get; private set; } = 0f;
        public float DefaultThrust = 0f;
        public float CurrentThrust { get; private set; } = 0f;
        


        public void ResetAppliedAxis()
        {
            CurrentFlightAxis = DefaultFlightAxis;
        }

        public void ResetAppliedForces()
        {
            CurrentLift = DefaultLift;
            CurrentThrust = DefaultThrust;
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
            ManualVerticalMovement();

            m_RigidBodyComponent.AddRelativeForce(Vector3.up * CurrentLift);
        }

        void ManualVerticalMovement()
        {
            if (Input.GetKey(KeyCode.I))
            {
                CurrentLift = MovementFlightForces.lift;
            }else if (Input.GetKey(KeyCode.K))
            {
                CurrentLift = -MovementFlightForces.weight;
            }
            else
            {
                CurrentLift = DefaultLift;
            }
        }

    }
}
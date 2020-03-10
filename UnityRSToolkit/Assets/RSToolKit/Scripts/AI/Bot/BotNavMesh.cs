using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RSToolkit.Helpers;
using RSToolkit.AI.Helpers;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(Bot))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class BotNavMesh : MonoBehaviour
    {
        public float walkSpeed = 0.75f;
        public float walkRotationSpeed = 120f;

        public float runSpeed = 5f;
        public float runRotationSpeed = 120f;

        private NavMeshAgent m_navMeshAgentComponent;
        public NavMeshAgent NavMeshAgentComponent
        {
            get
            {
                if (m_navMeshAgentComponent == null)
                {
                    m_navMeshAgentComponent = GetComponent<NavMeshAgent>();
                }
                return m_navMeshAgentComponent;
            }

        }

        private Bot m_botComponent;
        public Bot BotComponent
        {
            get
            {
                if (m_botComponent == null)
                {
                    m_botComponent = GetComponent<Bot>();
                }
                return m_botComponent;
            }

        }

        public float CurrentSpeed
        {
            get
            {
                return NavMeshHelpers.GetCurrentSpeed(NavMeshAgentComponent);
            }
        }

        public void WalkTo(Vector3 destination)
        {
            MoveTo(destination, walkSpeed, walkRotationSpeed);
        }

        public bool WalkToFocusedTarget()
        {
            if (BotComponent.FocusedOnTransform != null)
            {
                WalkTo(BotComponent.FocusedOnTransform.position);
                return true;
            }
            return false;
        }

        public void RunTo(Vector3 destination)
        {
            MoveTo(destination, runSpeed, runRotationSpeed);
        }

        public bool RunToFocusedTarget()
        {
            if (BotComponent.FocusedOnTransform != null)
            {
                RunTo(BotComponent.FocusedOnTransform.position);
                return true;
            }
            return false;
        }

        // To Refactor
        public void StepRotateTo(Transform target)
        {
            var rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, NavMeshAgentComponent.angularSpeed * Time.deltaTime);
        }


        public void MoveTo(Vector3 destination)
        {

            NavMeshAgentComponent.destination = destination;
            // Debug.Log(string.Format("Move to spot: [{0}]", destination.ToString()));
        }

        public void MoveTo()
        {
            if (BotComponent.FocusedOnPosition != null)
            {
                MoveTo(BotComponent.FocusedOnPosition.Value);
            }
        }

        public void MoveTo(Vector3 destination, float speed, float angularSpeed)
        {
            NavMeshAgentComponent.speed = speed;
            NavMeshAgentComponent.angularSpeed = angularSpeed;
            MoveTo(destination);
        }


        private void OnDrawGizmos()
        {
            NavMeshHelpers.DrawGizmoDestination(NavMeshAgentComponent);
        }



        private void Awake()
        {
            NavMeshAgentComponent.speed = walkSpeed;
            NavMeshAgentComponent.angularSpeed = walkRotationSpeed;
        }

    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RSToolkit.Helpers;
using RSToolkit.AI.Helpers;

namespace RSToolkit.AI
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class BotNavMesh : BotMovement
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

        public float CurrentSpeed
        {
            get
            {
                return NavMeshHelpers.GetCurrentSpeed(NavMeshAgentComponent);
            }
        }

        // To Refactor
        public void StepRotateTo(Transform target)
        {
            var rotation = Quaternion.LookRotation(target.position - transform.position, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, NavMeshAgentComponent.angularSpeed * Time.deltaTime);
        }

        public override void RotateTowardsPosition()
        {
            var rotation = Quaternion.LookRotation(BotComponent.FocusedOnPosition.Value - transform.position, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, NavMeshAgentComponent.angularSpeed* Time.deltaTime);
        }

    private void MoveTo(Vector3 destination, float speed, float angularSpeed)
        {
            NavMeshAgentComponent.speed = speed;
            NavMeshAgentComponent.angularSpeed = angularSpeed;
            NavMeshAgentComponent.destination = destination;
            NavMeshAgentComponent.stoppingDistance = 0f;
            switch (m_stopMovementCondition)
            {
                case StopMovementConditions.WITHIN_INTERACTION_DISTANCE:
                    NavMeshAgentComponent.stoppingDistance = BotComponent.SqrInteractionMagnitude * .75f;
                    break;
                case StopMovementConditions.WITHIN_PERSONAL_SPACE:
                    NavMeshAgentComponent.stoppingDistance = BotComponent.SqrPersonalSpaceMagnitude * .75f;
                    break;
            }
            NavMeshAgentComponent.isStopped = false;
        }

        public override void MoveTowardsPosition(bool fullspeed = true)
        {
            if (fullspeed)
            {
                MoveTo(BotComponent.FocusedOnPosition.Value, runSpeed, runRotationSpeed);
            }
            else
            {
                MoveTo(BotComponent.FocusedOnPosition.Value, walkSpeed, walkRotationSpeed);
            }
        }

        private void Awake()
        {
            NavMeshAgentComponent.speed = walkSpeed;
            NavMeshAgentComponent.angularSpeed = walkRotationSpeed;
        }

        void NotMoving_Enter()
        {
            NavMeshAgentComponent.isStopped = true;
        }

    }
}


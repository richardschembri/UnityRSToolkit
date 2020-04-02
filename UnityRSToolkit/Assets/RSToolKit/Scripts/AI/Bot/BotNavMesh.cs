using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RSToolkit.Helpers;
using RSToolkit.AI.Helpers;
using RSToolkit.Space3D;

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

        public ProximityChecker JumpProximityChecker;

        public float CurrentSpeed
        {
            get
            {
                return NavMeshHelpers.GetCurrentSpeed(NavMeshAgentComponent);
            }
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

        public void MoveToClosestEdge(bool fullspeed = true)
        {
            NavMeshHit hit;
            NavMeshAgentComponent.FindClosestEdge(out hit);
            BotComponent.UnFocus();
            BotComponent.FocusOnPosition(hit.position);
            MoveTowardsPosition(fullspeed);
        }

        public bool JumpOffLedge()
        {
            RaycastHit rayhit;
            
            if(JumpProximityChecker.IsWithinRayDistance(out rayhit) != null)
            {
                var jumpPath = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, rayhit.point, NavMesh.AllAreas, jumpPath);
                if(jumpPath.status == NavMeshPathStatus.PathComplete)
                {
                    Debug.Log(jumpPath.corners);
                    BotComponent.UnFocus();
                    BotComponent.FocusOnPosition(rayhit.point);
                    MoveTowardsPosition(true);
                    return true;
                }
            }
            return false;
        }

        private void Awake()
        {
            NavMeshAgentComponent.speed = walkSpeed;
            NavMeshAgentComponent.angularSpeed = walkRotationSpeed;
            NavMeshAgentComponent.radius = BotComponent.SqrPersonalSpaceMagnitude;
        }

        void NotMoving_Enter()
        {
            if (NavMeshAgentComponent.isOnNavMesh)
            {
                NavMeshAgentComponent.isStopped = true;
            }
        }

    }
}


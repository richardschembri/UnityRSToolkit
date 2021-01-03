using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;
using RSToolkit.Space3D;
using System.Linq;

namespace RSToolkit.AI.Locomotion
{
    public class BotPartWanderFlying : BotPartWander
    {
        // public float DefaultY = 5f;
        private float _defaultY;
        private int _findNewPositionAttempts = 0;
        private const int MAX_FINDNEWPOSITIONATTEMPTS = 20;
        public bool AboveSurface = true;

        // public bool UseSpherecast = true;

        // [Tooltip("If 0 or less a linecast will be used")]
        // public float SpherecastRadius {get; private set;}

        private Flying3DObject _flying3DObjectComponent;
        public Flying3DObject Flying3DObjectComponent
        {
            get
            {
                if (_flying3DObjectComponent == null)
                {
                    _flying3DObjectComponent = GetComponent<Flying3DObject>();
                }
                return _flying3DObjectComponent;
            }

        }

        public override Bot.DistanceType StopMovementCondition
        {
            get
            {
                return Bot.DistanceType.PERSONAL_SPACE;
            }
        }

        /// <summary>
        /// Set y-axis of which the Bot will be flying at
        /// </summary>
        public void SetDefaultY(float defaultY)
        {
            _defaultY = defaultY;
        }

        /// <summary>
        /// Check if Bot is able to wander
        /// </summary>
        public override bool CanWander()
        {
            return BotLocomotiveComponent.CurrentLocomotionType is BotLogicFlight 
                && Mathf.Abs(Flying3DObjectComponent.MovementFlightThrust.x) > 0f
                && Mathf.Abs(Flying3DObjectComponent.MovementFlightThrust.z) > 0f;
        }

        RaycastHit _wanderhit;

        /// <summary>
        /// Get a valid random position within the wander radius of the provided wanderCenter
        /// </summary>
        protected override Vector3? GetNewWanderPosition(Transform wanderCenter, float radius)
        {
            if (_findNewPositionAttempts >= MAX_FINDNEWPOSITIONATTEMPTS || radius <= BotLocomotiveComponent.SqrInteractionMagnitude)
            {
                _findNewPositionAttempts = 0;
                return null;
            }
            _findNewPositionAttempts++;
            //var newPos = transform.GetRandomPositionWithinCircle(radius, BotFlyingComponent.BotComponent.SqrPersonalSpaceMagnitude);
            float offset = BotLocomotiveComponent.SqrPersonalSpaceMagnitude + ((radius - BotLocomotiveComponent.SqrInteractionMagnitude) / 2);
            Vector3? newPos = wanderCenter.position.GetRandomPositionWithinCircle(radius, offset, _defaultY);
            // newPos = new Vector3(newPos.Value.x, BotLocomotiveComponent.transform.position.y, newPos.Value.z);
            // newPos = new Vector3(newPos.Value.x, _defaultY, newPos.Value.z);
            if (AboveSurface && !Physics.Raycast(newPos.Value, Vector3.down, Mathf.Infinity))
            {
                Debug.Log($"{newPos} Not above surface");
                return GetNewWanderPosition(wanderCenter, radius);
            }

            newPos = BotLocomotiveComponent.ColliderComponent.AdjustPositionInVerticalVolume(newPos.Value);

            //if ((!UseSpherecast && BotLocomotiveComponent.ColliderComponent.RaycastFromOutsideBounds(out _wanderhit, newPos.Value, radius))
            //    || (UseSpherecast && BotLocomotiveComponent.ColliderComponent.SpherecastFromOutsideBounds(out _wanderhit, newPos.Value, SpherecastRadius, radius)))
            if(BotLocomotiveComponent.RigidBodyComponent.SweepTestToPosition(out _wanderhit, newPos.Value))
            {
                
                if (DebugMode)
                {
                    Debug.Log($"Wander position is behind {_wanderhit.transform.name}");
                }
            
                if (Vector3.Distance(transform.position, _wanderhit.point) * 0.75f >= BotLocomotiveComponent.SqrPersonalSpaceMagnitude)
                {
                    newPos = Vector3.Lerp(transform.position, _wanderhit.point, 0.75f);
                }
                else
                {
                    newPos = GetNewWanderPosition(wanderCenter, radius);
                }
            }

            _findNewPositionAttempts = 0;

            return newPos;    
        }

        #region MonoBehaviour Functions
        protected override void Awake()
        {
            base.Awake();
            /*
            SpherecastRadius = Mathf.Max(Mathf.Max(BotLocomotiveComponent.ColliderComponent.bounds.size.x,
                                                    BotLocomotiveComponent.ColliderComponent.bounds.size.y),
                                            BotLocomotiveComponent.ColliderComponent.bounds.size.z);
            */
            SetDefaultY(this.transform.position.y);
        }
        #endregion MonoBehaviour Functions


    }

}
 

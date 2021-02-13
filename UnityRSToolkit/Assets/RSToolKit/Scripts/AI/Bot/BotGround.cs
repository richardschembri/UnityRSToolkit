using UnityEngine;
using RSToolkit.AI.Locomotion;
using UnityEngine.AI;

namespace RSToolkit.AI
{
    public class BotGround : BotLocomotive
    {
        public enum StatesGround{
            NOTGROUNDED,
            GROUNDED_NAVMESH_SUCCESS,
            GROUNDED_NAVMESH_FAIL
        }

        public StatesGround CurrentStatesGround {get; private set;} = StatesGround.NOTGROUNDED; // Assume it is starting in the air

        public BotLogicNavMesh BotLogicNavMeshRef { get; set; }

        NavMeshHit navGroundHit;

        #region Components

        private NavMeshAgent _navMeshAgentComponent;
        public NavMeshAgent NavMeshAgentComponent
        {
            get
            {
                if (_navMeshAgentComponent == null)
                {
                    _navMeshAgentComponent = GetComponent<NavMeshAgent>();
                }

                return _navMeshAgentComponent;
            }

        }

        protected BotPartWanderNavMesh BotWanderNavMeshComponent {get; private set;}
        #endregion Components

        /// <summary>
        /// Check if the Bot is in a position of falling/landing
        /// </summary>
        public void HandleFailling()
        {
            if (_IsNetworkPeer)
            {
                return;
            }

            if (CurrentStatesGround != StatesGround.GROUNDED_NAVMESH_SUCCESS)
            {
                if(CheckForGround()){
                    Land();
                }else if(GroundProximityCheckerComponent.IsAlmostTouching(false)){
                    CurrentStatesGround = StatesGround.GROUNDED_NAVMESH_FAIL;
                }
            }else if(!CheckForGround() && !GroundProximityCheckerComponent.IsAlmostTouching(false)){

                CurrentStatesGround = StatesGround.NOTGROUNDED;
            }
        }

        /// <summary>
        /// Handle landing on the surface by enabling NavMeshAgent
        /// </summary>
        private void Land(){
            // if(NavMesh.SamplePosition())
            // IsFreefall = false;

            NavMeshAgentComponent.enabled = true;
            if (!NavMeshAgentComponent.isOnNavMesh)
            {
                NavMeshAgentComponent.enabled = false;
                CurrentStatesGround = StatesGround.GROUNDED_NAVMESH_FAIL;
                
                return;
            }

            RigidBodyComponent.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            RigidBodyComponent.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            
            if(NavMeshAgentComponent.isActiveAndEnabled){
                CurrentStatesGround = StatesGround.GROUNDED_NAVMESH_SUCCESS;
            }else{
                CurrentStatesGround = StatesGround.GROUNDED_NAVMESH_FAIL;
            }
        }

        /*
        /// <summary>
        /// Turn on/off components depending if this bot is a host or a peer 
        /// </summary>
        /// <param name="toggleKinematic">Should the IsKinematic value of a Rigidbody be toggled as well</param>
        protected override void ToggleComponentsForNetwork(bool toggleKinematic = true)
        {
            base.ToggleComponentsForNetwork(toggleKinematic);

            if (_IsNetworkPeer)
            {
                NavMeshAgentComponent.enabled = false;
                if (CurrentStatesGround != StatesGround.NOTGROUNDED)
                {
                    // IsFreefall = false;
                    CurrentStatesGround = StatesGround.GROUNDED_NAVMESH_SUCCESS;
                }
            }
            else
            {
                HandleFailling();
            }

        }
        */

        /// <summary>
        /// Initialize the LocomotionTypes used by BotFlyable
        /// </summary>
        protected override void InitLocomotionTypes(){
            BotLogicNavMeshRef = new BotLogicNavMesh(this, NavMeshAgentComponent, JumpProximityChecker);
            CurrentLocomotionType = BotLogicNavMeshRef;
        }

        /// <summary>
        /// Initialize the BotPartWanders used by BotFlyable
        /// </summary>
        protected override bool InitBotWander(){
            if(!base.InitBotWander()){
                return false;
            }
            BotWanderNavMeshComponent = GetComponent<BotPartWanderNavMesh>();
            BotWanderManagerComponent.Initialize(BotWanderNavMeshComponent);
            return true;
        }

        #region MonoBehaviour Functions

        public override bool Init(bool force = false)
        {
            if (!base.Init(force))
            {
                return false;
            }
            HandleFailling();
            return true;
        }

        protected override void Awake()
        {
            base.Awake();
            
        }

        protected override void Update()
        {
            base.Update();
            // There must be a better way to do this
            HandleFailling();
        }
        protected override void OnCollisionEnter(Collision collision)
        {
            if (CurrentStatesGround != StatesGround.GROUNDED_NAVMESH_SUCCESS)
            {
                for (int i = 0; i < collision.contacts.Length; i++)
                {
                    HandleFailling();
                    if (CurrentStatesGround == StatesGround.GROUNDED_NAVMESH_SUCCESS){
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Check if the is a surface at the point
        /// </summary>
        private bool CheckForGround(Vector3 point){
            return NavMesh.SamplePosition(point, out navGroundHit, GroundProximityCheckerComponent.IsAlmostTouchingDistance, NavMesh.AllAreas);
        }

        /// <summary>
        /// Check if the is a surface at current point
        /// </summary>
        private bool CheckForGround(){
            return CheckForGround(transform.position);
        }

        #endregion MonoBehaviour Functions


        #region RSMonoBehaviour Functions
        protected override void OnRSShadowChanged_Listener(bool isShadow){
            base.OnRSShadowChanged_Listener(isShadow);
            if(isShadow){
                NavMeshAgentComponent.enabled = false;
                if (CurrentStatesGround != StatesGround.NOTGROUNDED)
                {
                    // IsFreefall = false;
                    CurrentStatesGround = StatesGround.GROUNDED_NAVMESH_SUCCESS;
                }
            }else{
                HandleFailling();
            }
        }

        #endregion RSMonoBehaviour Functions
    }
}

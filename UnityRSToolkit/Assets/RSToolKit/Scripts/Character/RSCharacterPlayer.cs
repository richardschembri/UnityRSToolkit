using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Character
{
    [RequireComponent(typeof(RSCharacterController))]
    public class RSCharacterPlayer : RSMonoBehaviour
    {
        [SerializeField]
        private RSPlayerInputManager _playerInputManager;
        private RSCharacterController _characterController;

        #region RSMonoBehaviour Functions
        protected override void InitComponents()
        {
            base.InitComponents();
            _characterController = GetComponent<RSCharacterController>();
        }
        #endregion RSMonoBehaviour Functions

        #region MonoBehaviour Functions
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            
        }
        #endregion MonoBehaviour Functions
    }
}

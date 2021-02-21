using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RSToolkit.Animation;
using RSToolkit.Helpers;

namespace RSToolkit.Character
{
    public abstract class RSCharacterController : RSMonoBehaviour
    {
        public string DisplayName;
        public class RSCharacterEvent :UnityEvent<RSCharacterController> {}
        public Animator AnimatorComponent {get; private set;}

        private float _currentSpeed;
        public float CurrentSpeed {
            get{
                return _currentSpeed;
            } 
            protected set{
                _currentSpeed = Mathf.Min(value, MaxSpeed);
            }
        }
        [SerializeField]
        private float _maxSpeed = 10f;
        public float MaxSpeed {
            get{
                return _maxSpeed;
            }
            protected set{
                _maxSpeed = value;
            }
        }
        public float CurrentSpeedPercent {
            get{
                return MathHelpers.GetValuePercent(CurrentSpeed, MaxSpeed);
            }
        }

        protected virtual void InitRigidBody(){

        }
        
        public override bool Init(bool force = false)
        {
            if(base.Init(force)){
                if(string.IsNullOrEmpty(DisplayName)){
                    DisplayName = gameObject.name;
                }
                InitRigidBody();
                AnimatorComponent = GetComponent<Animator>();
                return true;
            }
            return false;
        }

        public virtual void Move(Vector2 directionAxis, float speed){
            CurrentSpeed = speed;
        }
        #region MonoBehaviour Functions
        protected virtual void Update(){
            CharacterAnimParams.TrySetSpeedPercent(AnimatorComponent, Mathf.Abs(CurrentSpeedPercent));
        }
        #endregion MonoBehaviour Functions
    }
}

using System.Collections;
using System.Collections.Generic;
using RSToolkit.Character;
using UnityEngine;
using RSToolkit.Animation;

namespace RSToolkit.Space2D
{
    public class PlayerLocomotion2D : RSMonoBehaviour
    {

        public RSCharacterController ControllerComponent{ get; private set; }
        Vector2 _directionAxis = Vector2.zero;
        public float Speed = 40f;
        public float MoveAxisDeadZone = 0.2f;

        public override bool Init(bool force = false)
        {
            if(base.Init(force)){
                ControllerComponent = GetComponent<RSCharacterController>();
                return true;
            }
            return false;
        }
        // Update is called once per frame
        protected virtual void Update()
        {
            _directionAxis = Vector2.zero;
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
                _directionAxis.x = Vector2.right.x;
            }else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
                _directionAxis.x = Vector2.left.x;
            }else if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
                _directionAxis.y = Vector2.up.y;
            }else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
                _directionAxis.y = Vector2.down.y;
            }else{
                _directionAxis.x = Input.GetAxisRaw("Horizontal");
                _directionAxis.y = Input.GetAxisRaw("Vertical");
            }          
            if(Mathf.Abs(_directionAxis.x) >= MoveAxisDeadZone
                || Mathf.Abs(_directionAxis.y) >= MoveAxisDeadZone){
                CharacterAnimParams.TrySetAxisHorizontal(ControllerComponent.AnimatorComponent ,_directionAxis.x);
                CharacterAnimParams.TrySetAxisVertical(ControllerComponent.AnimatorComponent ,_directionAxis.y);
            }
        }

        protected virtual void FixedUpdate(){
            ControllerComponent.Move(_directionAxis, Speed);
        }
    }
}

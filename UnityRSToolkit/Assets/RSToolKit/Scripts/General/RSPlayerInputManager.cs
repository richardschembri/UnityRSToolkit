using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit
{
    public class RSPlayerInputManager : RSMonoBehaviour
    {
        public Vector2 _directionAxis; 
        public Vector2 DirectionAxis { get { return _directionAxis; } }
        public Vector3 DirectionAxis3D { get { return new Vector3(_directionAxis.x, 0f, _directionAxis.y); } }
        public Vector2 LastDirectionAxis { get; private set; } = Vector2.zero;
        public Vector3 LastDirectionAxis3D { get { return new Vector3(LastDirectionAxis.x, 0f, LastDirectionAxis.y); } }

        public Vector2 MouseInputAxis { get; private set; }

        public float DirectionAxisDeadZone = 0.2f;
        public bool HasDirectionInput { get; private set; }

        public bool JumpInput { get; private set; }

        // Update is called once per frame
        public void UpdateInput()
        {
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
            if (Mathf.Abs(_directionAxis.x) < DirectionAxisDeadZone)
            {
                _directionAxis.x = 0.0f;
            }

            if (Mathf.Abs(_directionAxis.y) < DirectionAxisDeadZone)
            {
                _directionAxis.y = 0.0f;
            }

            bool hasDirectionInput = _directionAxis.sqrMagnitude > 0.0f;

            if (HasDirectionInput && !hasDirectionInput)
            {
                LastDirectionAxis = DirectionAxis ;
            }

            HasDirectionInput = hasDirectionInput;

            // Update other inputs
            MouseInputAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            JumpInput = Input.GetButton("Jump");
        }
    }
}

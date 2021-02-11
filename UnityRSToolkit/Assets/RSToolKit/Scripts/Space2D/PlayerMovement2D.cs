using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space2D
{
    public class PlayerMovement2D : MonoBehaviour
    {
        public CharacterController2D _controller;
        public Animator _animator;

        public float _moveSpeed = 40f;

        float _horizontalMove = 0f;

        // Update is called once per frame
        void Update () {


            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
                _horizontalMove = _moveSpeed;
            }else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
                _horizontalMove = -_moveSpeed;
            }else{
                _horizontalMove = Input.GetAxisRaw("Horizontal") * _moveSpeed;
            }
            _animator.SetFloat("Speed", Mathf.Abs(_horizontalMove));
        }

        void FixedUpdate ()
        {
            // Move our character
            _controller.Move(_horizontalMove * Time.fixedDeltaTime);
        }
    }
}
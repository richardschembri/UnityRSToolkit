using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Character;

namespace RSToolkit.Space2D.SideScroller
{
    public class CharacterController2D : RSCharacterController
    {
        [Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;	// How much to smooth out the movement

        public Rigidbody2D Rigidbody2DComponent{get; private set;}
        public bool FacingRight {get; private set;} = true;  // For determining which way the player is currently facing.
        private Vector3 _velocity = Vector3.zero;

        protected override void InitComponents()
        {
            base.InitComponents();
            Rigidbody2DComponent = GetComponent<Rigidbody2D>();
        }

        public override void Move(Vector2 directionAxis, float speed)
        {
            base.Move(directionAxis, directionAxis.x * speed);
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(CurrentSpeedHorizontal, Rigidbody2DComponent.velocity.y);
            // And then smoothing it out and applying it to the character
            Rigidbody2DComponent.velocity = Vector3.SmoothDamp(Rigidbody2DComponent.velocity, targetVelocity, ref _velocity, _movementSmoothing);

            if ((directionAxis.x > 0 && !FacingRight)
                ||(directionAxis.x < 0 && FacingRight))
            {
                // ... flip the player.
                Flip();
            }
        }

        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            FacingRight = !FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

    }
}

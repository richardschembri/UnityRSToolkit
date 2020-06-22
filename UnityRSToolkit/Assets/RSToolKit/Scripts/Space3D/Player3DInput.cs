using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RSToolkit.Space3D
{
    public class Player3DInput : MonoBehaviour
    {
        public float MoveAxisDeadZone = 0.2f;

		public Vector2 MoveInput { get; private set; }
		public Vector2 LastMoveInput { get; private set; }
		public Vector2 CameraInput { get; private set; }
		public bool JumpInput { get; private set; }

		public bool HasMoveInput { get; private set; }

        private Vector2 m_moveInput;
        bool m_hasMoveInput;
		public void UpdateInput()
		{
			// Update MoveInput
			m_moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			if (Mathf.Abs(m_moveInput.x) < MoveAxisDeadZone)
			{
				m_moveInput.x = 0.0f;
			}

			if (Mathf.Abs(m_moveInput.y) < MoveAxisDeadZone)
			{
				m_moveInput.y = 0.0f;
			}

			m_hasMoveInput = m_moveInput.sqrMagnitude > 0.0f;

			if (HasMoveInput && !m_hasMoveInput)
			{
				LastMoveInput = MoveInput;
			}

			MoveInput = m_moveInput;
			HasMoveInput = m_hasMoveInput;

			// Update other inputs
			CameraInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			JumpInput = Input.GetButton("Jump");
		}
    }
}

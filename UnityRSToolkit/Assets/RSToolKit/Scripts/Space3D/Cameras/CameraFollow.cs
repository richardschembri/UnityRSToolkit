using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit.Space3D.Cameras
{
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Transform target;
        
        private Vector3 m_velocity;
        public Vector3 TrailPosition = new Vector3(0,1,-3);
        public float m_viewAngle = 15;
        private void FixedUpdate()
        {
            transform.position = Vector3.SmoothDamp(transform.position, target.transform.TransformPoint(TrailPosition), ref m_velocity, 0.1f);
            transform.rotation = Quaternion.Euler(new Vector3(m_viewAngle, target.rotation.eulerAngles.y, 0));
        }

    }
}
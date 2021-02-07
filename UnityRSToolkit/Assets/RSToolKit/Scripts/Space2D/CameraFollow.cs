using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space2D
{

    public class CameraFollow : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;
        [Range(1,10)]
        public float smoothFactor;
        private Vector3 _targetPosition;
        private Vector3 _smoothPosition;

        // Update is called once per frame
        void FixedUpdate()
        {
        }

        void Follow(){
           _targetPosition = target.position + offset;
           _smoothPosition = Vector3.Lerp(transform.position, _targetPosition, smoothFactor * Time.fixedDeltaTime);
           target.position = _smoothPosition; 
        }
    }
}

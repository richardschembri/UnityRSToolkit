<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space2D
{
    public class CameraFollow2D : RSMonoBehaviour
    {
        public Transform target;
        public Vector3 offset;
        [Range(0,10)]
        public float smoothFactor = 1f;
        private Vector3 _targetPosition;
        private Vector3 _newPosition;

        [SerializeField]
        private Transform _boundRight;
        [SerializeField]
        private Transform _boundLeft;
        [SerializeField]
        private Transform _boundTop;
        [SerializeField]
        private Transform _boundBottom;

        // Update is called once per frame
        void FixedUpdate()
        {
            Follow();
        }

        void Follow(){
           _targetPosition = target.position + offset;
           if(smoothFactor > 0){
            _newPosition = Vector3.Lerp(transform.position, _targetPosition, smoothFactor * Time.fixedDeltaTime);
           }else{
            _newPosition = _targetPosition;
           }
           //transform.position = new Vector3(_smoothPosition.x, _smoothPosition.y, transform.position.z);//_smoothPosition; 

           if(_boundRight != null && _newPosition.x > _boundRight.position.x){
            _newPosition.x = _boundRight.position.x;
           }else if(_boundLeft != null && _newPosition.x < _boundLeft.position.x){
            _newPosition.x = _boundLeft.position.x;
           }
           if(_boundTop != null && _newPosition.y > _boundTop.position.y){
            _newPosition.y = _boundTop.position.y;
           }else if(_boundBottom != null && _newPosition.y < _boundBottom.position.y){
            _newPosition.y = _boundBottom.position.y;
           }

           transform.position = new Vector3(_newPosition.x, _newPosition.y, transform.position.z);//_smoothPosition; 
        }
    }
}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Space2D
{
    public class CameraFollow2D : MonoBehaviour
    {
        public Transform target;
        public Vector3 offset;
        [Range(1,10)]
        public float smoothFactor = 1f;
        private Vector3 _targetPosition;
        private Vector3 _smoothPosition;

        // Update is called once per frame
        void FixedUpdate()
        {
            Follow();
        }

        void Follow(){
           _targetPosition = target.position + offset;
           _smoothPosition = Vector3.Lerp(transform.position, _targetPosition, smoothFactor * Time.fixedDeltaTime);
           transform.position = new Vector3(_smoothPosition.x, transform.position.y, transform.position.z);//_smoothPosition; 
        }
    }
}
>>>>>>> dfa7bc39c549895533d19d6e45d5922c55741787

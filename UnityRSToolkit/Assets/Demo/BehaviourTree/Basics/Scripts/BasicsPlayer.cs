using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demo.BehaviourTree.Basics{
    public class BasicsPlayer : MonoBehaviour
    {
        private Camera _sceneCamera;
        private Ray _mouseRay;

        private Vector3 RayToPosition(Ray r){
            float z = 0f;
            float delta = (z - r.origin.z) / r.direction.z;

            if (delta > 0.1f && delta < 10000f){
                return r.origin + r.direction * delta;
            }

            return Vector3.zero;
        }

#region MonoBehaviour Functions
        void Start()
        {
            _sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        void Update()
        {
            _mouseRay = _sceneCamera.ScreenPointToRay(Input.mousePosition);
            this.transform.position = RayToPosition(_mouseRay);
        }
#endregion MonoBehaviour Functions

    }
}

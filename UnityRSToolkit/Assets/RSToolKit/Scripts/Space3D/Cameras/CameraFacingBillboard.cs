// Taken from https://wiki.unity3d.com/index.php/CameraFacingBillboard
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Space3D.Helpers;

namespace RSToolkit.Space3D.Cameras
{
    public class CameraFacingBillboard : RSMonoBehaviour
    {
        private Camera _targetCamera = null;

        public bool reverseFace = false;
        public AxisHelpers.Axis axis = AxisHelpers.Axis.UP;
        public Vector3 OffsetRotation = Vector3.zero;

        #region MonoBehaviour Functions
        protected override void Awake()
        {
            base.Awake();
            if(_targetCamera == null)
            {
                _targetCamera = Camera.allCameras[0];
            }
        }

        //Orient the camera after all movement is completed this frame to avoid jittering
        void LateUpdate()
        {
            // rotates the object relative to the camera
            Vector3 targetPos = transform.position + _targetCamera.transform.rotation * (reverseFace ? Vector3.back : Vector3.forward);
            Vector3 targetOrientation = _targetCamera.transform.rotation * axis.ToVector3();
            transform.LookAt(targetPos, targetOrientation);
            transform.rotation =  Quaternion.Euler(transform.rotation.eulerAngles + OffsetRotation);
        }
        #endregion MonoBehaviour Functions

    }
}

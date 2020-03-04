using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Space3D;

namespace RSToolkit.AI
{
    [RequireComponent(typeof(Flying3DObject))]
    public class NavMeshNPCFlight : MonoBehaviour
    {
        public Vector3? destination;
        private bool m_idle = true;
        public float arrivalMagnitude = 0f;
        public float SqrArrivalMagnitude
        {
            get
            {
                return arrivalMagnitude * arrivalMagnitude;
            }
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
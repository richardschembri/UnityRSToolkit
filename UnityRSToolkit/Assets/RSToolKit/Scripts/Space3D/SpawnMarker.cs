using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSToolkit.Helpers;

namespace RSToolkit.Space3D
{
    public class SpawnMarker : MonoBehaviour
    {
        public bool CanSpawn { get; set; } = false;

        public float MaxDistance = 10f;

        private Collider m_parentColliderComponent;
        public Collider ParentColliderComponent
        {
            get
            {
                if (m_parentColliderComponent == null)
                {
                    m_parentColliderComponent = GetComponentInParent<Collider>();
                }
                return m_parentColliderComponent;
            }
        }

        protected Ray m_ray;
        protected RaycastHit m_rayHit;

        public float DistanceFromSurface = 0.5f;

        protected virtual Vector3 GetSurfacePoint()
        {
            CanSpawn = ParentColliderComponent.RaycastFromOutsideBounds(ref m_ray, out m_rayHit, ParentColliderComponent.transform.position + Vector3.down, MaxDistance);
            return m_rayHit.point;
        }

        private void PositionMarker()
        {
            this.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            transform.position = GetSurfacePoint() + Vector3.up * DistanceFromSurface;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            PositionMarker();
        }
    }
}

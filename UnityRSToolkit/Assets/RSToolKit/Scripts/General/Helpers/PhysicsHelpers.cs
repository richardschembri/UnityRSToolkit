using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Helpers
{
    public static class PhysicsHelpers
    {
        public static Ray GetRayFromOutsideBoundsTowards(this Collider source, Vector3 position)
        {
            Ray result = new Ray();
            source.SetRayFromOutsideBoundsTowards(ref result, position);
            return result;
        }

        public static void SetRayFromOutsideBoundsTowards(this Collider source, ref Ray ray, Vector3 position)
        {
            ray.direction = position - source.transform.position;
            ray.origin = source.ClosestPoint(position) + ray.direction.normalized * 0.05f;
        }

        public static bool RaycastFromOutsideBounds(this Collider source, ref Ray ray, out RaycastHit hit, Vector3 position)
        {
            source.SetRayFromOutsideBoundsTowards(ref ray, position);
            return Physics.Raycast(ray, out hit);
        }

        public static bool LinecastFromOutsideBounds(this Collider source, out RaycastHit hit, Vector3 position)
        {
            var direction = (position - source.transform.position).normalized;
            return Physics.Linecast(source.ClosestPoint(position) + direction.normalized * 0.05f, position, out hit);
        }

        public static bool SpherecastFromOutsideBounds(this Collider source, out RaycastHit hit, Vector3 position, float radius)
        {
            var direction = (position - source.transform.position).normalized;
            return Physics.SphereCast(source.ClosestPoint(position) + direction.normalized * 0.05f, radius, direction, out hit);
        }

    }
}
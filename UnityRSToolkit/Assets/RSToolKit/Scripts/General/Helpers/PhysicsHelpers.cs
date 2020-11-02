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

        public static bool RaycastFromOutsideBounds(this Collider source, out RaycastHit hit, Vector3 position, float maxDistance = Mathf.Infinity)
        {
            Ray ray = source.GetRayFromOutsideBoundsTowards(position);
            return source.RaycastFromOutsideBounds(ref ray, out hit, position, maxDistance);
        }

        public static bool RaycastFromOutsideBounds(this Collider source, ref Ray ray, out RaycastHit hit, Vector3 position, float maxDistance = Mathf.Infinity)
        {
            source.SetRayFromOutsideBoundsTowards(ref ray, position);
            return Physics.Raycast(ray, out hit, maxDistance);
        }

        public static bool LinecastFromOutsideBounds(this Collider source, out RaycastHit hit, Vector3 position)
        {
            var direction = (position - source.transform.position).normalized;
            return Physics.Linecast(source.ClosestPoint(position) + direction.normalized * 0.05f, position, out hit);
        }

        public static bool SpherecastFromOutsideBounds(this Collider source, out RaycastHit hit, Vector3 position, float radius, float maxDistance = Mathf.Infinity)
        {
            var direction = (position - source.transform.position).normalized;
            return Physics.SphereCast(source.ClosestPoint(position) + direction.normalized * 0.05f, radius, direction, out hit, maxDistance);
        }

        // I need better name for this. Need to improve
        public static Vector3 AdjustPositionInVerticalVolume(this Collider source, Vector3 position){
            RaycastHit hit;
            float radius = Mathf.Max(source.bounds.size.x, source.bounds.size.z) / 2;

            float newY = position.y;
            if(Physics.SphereCast(position, radius, Vector3.up, out hit, source.bounds.size.y)){
                newY = hit.point.y - source.bounds.size.y;
                if(newY < position.y){
                    return new Vector3(position.x, newY, position.z);
                }
            }

            if(Physics.SphereCast(position, radius, Vector3.down, out hit, source.bounds.size.y)){
                newY = hit.point.y + source.bounds.size.y;
                if(newY > position.y){
                    return new Vector3(position.x, newY, position.z);
                }
            }

            return position;
        }

    }
}
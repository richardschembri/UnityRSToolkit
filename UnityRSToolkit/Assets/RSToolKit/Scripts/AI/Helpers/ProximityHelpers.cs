using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RSToolkit.Helpers
{
    public class ProximityHelpers
    {
        public enum RayDirection
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            FRONT,
            REAR
        }

        public static Vector3 GetDirection(Transform sourceTransform, Transform targetTransform)
        {
            return targetTransform.position - sourceTransform.position;
        }

        // returns true if targetTransform is within sight of transform
        public static bool IsWithinSight(Transform sourceTransform, Transform targetTransform, float fieldOfViewAngle, float sqrViewMagnitude)
        {
            Vector3 direction = targetTransform.position - sourceTransform.position;
            float angle = Vector3.Angle(direction, sourceTransform.forward);
            // an object is within sight if it is within the field of view and has a magnitude less than what the object can see
            if (IsWithinViewingAngle(sourceTransform, targetTransform, fieldOfViewAngle)
                && IsWithinDistance(sourceTransform, targetTransform, sqrViewMagnitude))
            {
                return true;

                return IsInLineOfSight(sourceTransform, targetTransform, GetDirection(sourceTransform, targetTransform));
            }

            return false;
        }

        public static bool IsWithinViewingAngle(Transform sourceTransform, Transform targetTransform, float fieldOfViewAngle)
        {
            Vector3 direction = targetTransform.position - sourceTransform.position;
            float angle = Vector3.Angle(GetDirection(sourceTransform, targetTransform), sourceTransform.forward);
            return angle < fieldOfViewAngle;
        }

        public static bool IsWithinDistance(Transform sourceTransform, Transform targetTransform, float sqrViewMagnitude)
        {
            return IsWithinDistance(sourceTransform, targetTransform.position, sqrViewMagnitude);
        }

        public static bool IsWithinDistance(Transform sourceTransform, Vector3 position, float sqrViewMagnitude)
        {
            return Vector3.SqrMagnitude(position - sourceTransform.position) < sqrViewMagnitude;
        }

        // returns true if targetTransform is in a line of sight of transform
        public static bool IsInLineOfSight(Transform transform, Transform targetTransform, Vector3 direction)
        {
            RaycastHit hit;
            // cast a ray. If the ray hits the targetTransform then no objects are obtruding the view
            if (Physics.Raycast(transform.position, direction.normalized, out hit))
            {
                if (hit.transform.Equals(targetTransform))
                {
                    return true;
                }
            }
            return false;
        }

        public static void DrawGizmoProximity(Transform transform, float radius)
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = new Color(0.5f, 0f, 1f, 0.5f); // Color.blue;
            UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, radius);
            UnityEditor.Handles.color = oldColor;
#endif 
        }

        // hekp visualize the line of sight within the editor
        public static void DrawGizmoLineOfSight(Transform transform, float fieldOfViewAngle, float viewMagnitude, bool isWithinSight = false)
        {
#if UNITY_EDITOR
            float radius = viewMagnitude * Mathf.Sin(fieldOfViewAngle * Mathf.Deg2Rad);
            var oldColor = UnityEditor.Handles.color;
            if (isWithinSight)
            {
                UnityEditor.Handles.color = Color.red;
            }
            else
            {
                UnityEditor.Handles.color = Color.yellow;
            }

            // draw a disk at the end of the sight distance.
            UnityEditor.Handles.DrawWireDisc(transform.position + transform.forward * viewMagnitude, transform.forward, radius);
            // draw to lines to represent the left and right side of the line of sight
            UnityEditor.Handles.DrawLine(transform.position, transform.TransformPoint(new Vector3(radius, 0, viewMagnitude)));
            UnityEditor.Handles.DrawLine(transform.position, transform.TransformPoint(new Vector3(-radius, 0, viewMagnitude)));
            UnityEditor.Handles.color = oldColor;
#endif
        }

    }
}

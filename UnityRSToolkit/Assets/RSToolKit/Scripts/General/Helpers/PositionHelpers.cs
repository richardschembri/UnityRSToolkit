using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RSToolkit.Helpers{
    public static class PositionHelpers
    {
        public enum VerticalPosition
        {
            ABOVE,
            BELOW,
            EQUAL
        }
        public enum HorizontalPosition2D
        {
            LEFT,
            RIGHT,
            EQUAL
        }
        public enum HorizontalPosition3D
        {
            LEFT,
            RIGHT,
            INFRONT,
            BEHIND,
            EQUAL
        }

        public static HorizontalPosition3D GetHorizontalPosition3D(Transform sourceTransform, Transform targetTransform)
        {
            if(sourceTransform.position == targetTransform.position){
                return HorizontalPosition3D.EQUAL;
            }

            Vector3 direction = targetTransform.position - sourceTransform.position;
            float angle = Vector3.Angle(ProximityHelpers.GetDirection(sourceTransform, targetTransform), sourceTransform.forward);

            if(angle >= -22.5 && angle <= 22.5){
                return HorizontalPosition3D.INFRONT;
            }else if(angle >= -67.5 && angle <= -22.5){
                return HorizontalPosition3D.LEFT;
            }else if(angle <= 67.5 && angle >= 22.5){
                return HorizontalPosition3D.RIGHT;
            }else if(new Vector3(sourceTransform.position.x, 0f, sourceTransform.position.z)
                        == new Vector3(targetTransform.position.x, 0f, targetTransform.position.z))
            {
                return HorizontalPosition3D.EQUAL;
            }

            return HorizontalPosition3D.BEHIND;
        }

        public static VerticalPosition GetTargetVerticalPosition(Transform sourceTransform, Transform targetTransform, bool approximate = true)
        {
            float sourceY = sourceTransform.position.y;
            float targetY = targetTransform.position.y;
            if(approximate){
                sourceY = Mathf.RoundToInt(sourceY);
                targetY = Mathf.RoundToInt(targetY);
            }
            if(sourceY < targetY){
                return VerticalPosition.ABOVE;
            }
            else if(sourceY > targetY){
                return VerticalPosition.BELOW;
            }

            return VerticalPosition.EQUAL;
        }
    }
}

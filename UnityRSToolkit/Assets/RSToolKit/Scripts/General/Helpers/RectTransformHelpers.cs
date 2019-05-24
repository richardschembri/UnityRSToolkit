namespace RSToolkit.Helpers
{
    
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    public static class RectTransformHelpers 
    {
        public enum VerticalPosition{
            ABOVE,
            BELOW,
            CENTER
        }  
        public enum HorizontalPosition{
            LEFT,
            RIGHT,
            CENTER
        }  

        public struct RectTransformPosition{
            public HorizontalPosition horizontalPostion;
            public VerticalPosition verticalPostion;
            public bool isInside;
        }

        public static RectTransformPosition PositionWithinBounds(this RectTransform self, RectTransform target){
            RectTransformPosition rtp;
            rtp.horizontalPostion = HorizontalPosition.CENTER;
            rtp.verticalPostion = VerticalPosition.CENTER;
            rtp.isInside = true;


            Vector3[] selfBounds = new Vector3[4];
            self.GetWorldCorners(selfBounds);
    
            Vector3[] targetBounds = new Vector3[4];
            target.GetWorldCorners(targetBounds);

            float selfMaxY = selfBounds.Max(b => b.y);
            float selfMinY = selfBounds.Min(b => b.y);

            float selfMaxX = selfBounds.Max(b => b.x);
            float selfMinX = selfBounds.Min(b => b.x);

            float targetMaxY = targetBounds.Max(b => b.y);
            float targetMinY = targetBounds.Min(b => b.y);

            float targetMaxX = targetBounds.Max(b => b.x);
            float targetMinX = targetBounds.Min(b => b.x);
            
            if (targetMaxY < selfMinY){
                rtp.verticalPostion = VerticalPosition.BELOW;
            }
            if (targetMinY > selfMaxY){
                rtp.verticalPostion = VerticalPosition.ABOVE;
            }
            if (targetMaxX < selfMinX){
                rtp.horizontalPostion = HorizontalPosition.LEFT;
            }
            if (targetMinX > selfMaxX){
                rtp.horizontalPostion = HorizontalPosition.RIGHT;
            }
            
            return rtp;
        }
        public static bool HasWithinBounds(this RectTransform self, RectTransform target){

            var rtp = self.PositionWithinBounds(target);
            return rtp.horizontalPostion == HorizontalPosition.CENTER && rtp.verticalPostion == VerticalPosition.CENTER;
            
        }


        public static Vector2 ScaledSize(this RectTransform self){

            return new Vector2{
                x = self.rect.x * self.localScale.x,
                y = self.rect.y * self.localScale.y 
            };
        }



    }

}
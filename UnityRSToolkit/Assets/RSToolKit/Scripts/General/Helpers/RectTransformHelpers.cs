namespace RSToolkit.Helpers
{
    
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    public static class RectTransformHelpers 
    {
        public class RectTransformWorldEdges{
            public Vector2 Min{get; private set;}
            public Vector2 Max{get; private set;}

            public RectTransformWorldEdges(RectTransform trans){

                Vector3[] bounds = new Vector3[4];
                trans.GetWorldCorners(bounds);

                Min = new Vector2(bounds.Min(b => b.x), bounds.Min(b => b.y));
                Max = new Vector2(bounds.Max(b => b.x), bounds.Max(b => b.y));
            }
        }
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
            return  PositionWithinBounds(self, target, Vector2.zero, Vector2.zero);
        }

        public static RectTransformPosition PositionWithinBounds(this RectTransform self, RectTransform target, Vector2 paddingMin, Vector2 paddingMax){
            RectTransformPosition rtp;
            rtp.horizontalPostion = HorizontalPosition.CENTER;
            rtp.verticalPostion = VerticalPosition.CENTER;
            rtp.isInside = true;

            var selfEdges = new RectTransformWorldEdges(self);
            var targetEdges = new RectTransformWorldEdges(target);
            
            if (targetEdges.Max.y + paddingMax.y < selfEdges.Min.y){
                rtp.verticalPostion = VerticalPosition.BELOW;
            }
            if (targetEdges.Min.y + paddingMin.y > selfEdges.Max.y){
                rtp.verticalPostion = VerticalPosition.ABOVE;
            }
            if (targetEdges.Max.x + paddingMax.x < selfEdges.Min.x){
                rtp.horizontalPostion = HorizontalPosition.LEFT;
            }
            if (targetEdges.Min.x + paddingMin.x > selfEdges.Max.x){
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

        public static Vector2 ShiftUpPosition(this RectTransform self){
            return new Vector2(self.position.x, self.position.y + self.ScaledSize().y);
        }

        public static Vector2 ShiftDownPosition(this RectTransform self){
            return new Vector2(self.position.x, self.position.y - self.ScaledSize().y);
        }

        public static Vector2 ShiftLeftPosition(this RectTransform self){
            return new Vector2(self.position.x - self.ScaledSize().x, self.position.y);
        }

        public static Vector2 ShiftRightPosition(this RectTransform self){
            return new Vector2(self.position.x + self.ScaledSize().x, self.position.y);
        }

    }

}
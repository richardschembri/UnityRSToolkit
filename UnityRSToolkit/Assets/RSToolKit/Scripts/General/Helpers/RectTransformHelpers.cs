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
            WITHIN
        }  
        public enum HorizontalPosition{
            LEFT,
            RIGHT,
            WITHIN
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
            rtp.horizontalPostion = HorizontalPosition.WITHIN;
            rtp.verticalPostion = VerticalPosition.WITHIN;
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
            return rtp.horizontalPostion == HorizontalPosition.WITHIN && rtp.verticalPostion == VerticalPosition.WITHIN;
            
        }

        public static Vector2 GetResizeByWidth(this RectTransform self, float width){
            var resizePercent = MathHelpers.GetValuePercent(width, self.sizeDelta.x);
            var resizedHeight = MathHelpers.GetPercentValue(resizePercent, self.sizeDelta.y);
            return new Vector2(width, resizedHeight);
        }
        public static void ResizeByWidth(this RectTransform self, float width){
            self.sizeDelta = self.GetResizeByWidth(width);
        }

        public static Vector2 GetResizeByHeight(this RectTransform self, float height){
            var resizePercent = MathHelpers.GetValuePercent(height, self.sizeDelta.y);
            var resizedWidth = MathHelpers.GetPercentValue(resizePercent, self.sizeDelta.x);
            return new Vector2(resizedWidth, height);
        }
        public static void ResizeByHeight(this RectTransform self, float height){
            self.sizeDelta = self.GetResizeByHeight(height);
        }

/*
Aspect Ration Fitter does the same job
        public static void ResizeToParentAndKeepAspect(this RectTransform self){
            var parent = self.parent.GetComponent<RectTransform>();
            var newSize =  self.GetResizeByWidth(parent.sizeDelta.x);
            if(newSize.y < parent.sizeDelta.y){
                newSize =  self.GetResizeByHeight(parent.sizeDelta.y);
            }
           self.sizeDelta = newSize;
        }
*/

        public static Vector2 ScaledSize(this RectTransform self){

            return new Vector2{
                x = self.rect.width * self.localScale.x,
                y = self.rect.height * self.localScale.y 
            };
        }

        public static Vector2 ShiftUpPosition(this RectTransform self, float offset = 0f){
            return new Vector2(self.position.x, self.position.y + self.ScaledSize().y + offset);
        }

        public static Vector2 ShiftDownPosition(this RectTransform self, float offset = 0f){
            return new Vector2(self.position.x, self.position.y - (self.ScaledSize().y + offset));
        }

        public static Vector2 ShiftLeftPosition(this RectTransform self, float offset = 0f){
            return new Vector2(self.position.x - (self.ScaledSize().x + offset), self.position.y);
        }

        public static Vector2 ShiftRightPosition(this RectTransform self, float offset = 0f){
            return new Vector2(self.position.x + self.ScaledSize().x + offset, self.position.y);
        }

        public static Vector2 GetRelativeAnchorPositionOf(this RectTransform self, RectTransform target){
            Vector2 result;
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint( null, target.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(self, screenP, null, out result );
            return result;
        }

    }

}
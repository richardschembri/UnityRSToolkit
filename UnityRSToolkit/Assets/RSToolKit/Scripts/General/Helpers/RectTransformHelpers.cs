namespace RSToolkit.Helpers
{

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;
    public static class RectTransformHelpers
    {
        public enum AnchorPresets
        {
            TopLeft,
            TopCenter,
            TopRight,

            MiddleLeft,
            MiddleCenter,
            MiddleRight,

            BottomLeft,
            BottonCenter,
            BottomRight,
            BottomStretch,

            VertStretchLeft,
            VertStretchRight,
            VertStretchCenter,

            HorStretchTop,
            HorStretchMiddle,
            HorStretchBottom,

            StretchAll
        }

        public enum PivotPresets
        {
            TopLeft,
            TopCenter,
            TopRight,

            MiddleLeft,
            MiddleCenter,
            MiddleRight,

            BottomLeft,
            BottomCenter,
            BottomRight,
        }

        public class RectTransformEdges
        {
            public Vector2 Min { get; private set; }
            public Vector2 Max { get; private set; }
            public Vector2 Mid { get; private set; }

            protected void SetMinMax(Vector3[] bounds)
            {
                Min = new Vector2(bounds.Min(b => b.x), bounds.Min(b => b.y));
                Max = new Vector2(bounds.Max(b => b.x), bounds.Max(b => b.y));
                Mid = new Vector2(
                    ((Max.x - Min.x) / 2) + Min.x,
                    ((Max.y - Min.y) / 2) + Min.y
                );
            }

            public RectTransformEdges(RectTransform trans)
            {
                Vector3[] bounds = new Vector3[4];
                trans.GetWorldCorners(bounds);

                var c = trans.GetComponentInParent<Canvas>();
                if (c.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    for (int i = 0; i < bounds.Length; i++)
                    {
                        bounds[i] = RectTransformUtility.WorldToScreenPoint(c.worldCamera, bounds[i]); //c.worldCamera.WorldToViewportPoint(bounds[i]);
                    }
                }

                SetMinMax(bounds);
            }
        }

        public static RectTransformEdges GetEdges(this RectTransform self)
        {
            return new RectTransformEdges(self);
        }

        public enum VerticalPosition
        {
            ABOVE,
            BELOW,
            WITHIN
        }
        public enum HorizontalPosition
        {
            LEFT,
            RIGHT,
            WITHIN
        }

        public struct RectTransformPosition
        {
            public HorizontalPosition horizontalPostion;
            public VerticalPosition verticalPostion;
            //public bool isInside;
        }
        public static RectTransformPosition PositionWithinBounds(this RectTransform self, RectTransform target)
        {
            return PositionWithinBounds(self, target, Vector2.zero, Vector2.zero);
        }


        public static RectTransformPosition PositionWithinBounds(this RectTransform self, RectTransform target,
                                                                    out RectTransformEdges selfEdges, out RectTransformEdges targetEdges)
        {
            return self.PositionWithinBounds(target, Vector2.zero, Vector2.zero, out selfEdges, out targetEdges);

        }

        public struct RectTransformLRTB
        {
            public float Left;
            public float Right;
            public float Top;
            public float Bottom;
            public RectTransformLRTB(float left, float right, float top, float bottom)
            {
                Left = left;
                Right = right;
                Top = top;
                Bottom = bottom;
            }
        }

        public static RectTransformPosition PositionWithinBounds(this RectTransform self, RectTransform target, 
                                                                    Vector2 paddingMin, Vector2 paddingMax,
                                                                    out RectTransformEdges selfEdges, out RectTransformEdges targetEdges)
        {
            RectTransformPosition rtp;
            rtp.horizontalPostion = HorizontalPosition.WITHIN;
            rtp.verticalPostion = VerticalPosition.WITHIN;
            //rtp.isInside = true;

            selfEdges = self.GetEdges();
            targetEdges = target.GetEdges();

            if (targetEdges.Max.y + paddingMax.y < selfEdges.Min.y)
            {
                rtp.verticalPostion = VerticalPosition.BELOW;
            }
            if (targetEdges.Min.y + paddingMin.y > selfEdges.Max.y)
            {
                rtp.verticalPostion = VerticalPosition.ABOVE;
            }
            if (targetEdges.Max.x + paddingMax.x < selfEdges.Min.x)
            {
                rtp.horizontalPostion = HorizontalPosition.LEFT;
            }
            if (targetEdges.Min.x + paddingMin.x > selfEdges.Max.x)
            {
                rtp.horizontalPostion = HorizontalPosition.RIGHT;
            }

            return rtp;
        }

        public static RectTransformPosition PositionWithinBounds(this RectTransform self, RectTransform target, 
                                                                    Vector2 paddingMin, Vector2 paddingMax)
        {
            
            RectTransformEdges selfEdges;
            RectTransformEdges targetEdges;

            return self.PositionWithinBounds(target, paddingMin, paddingMax, out selfEdges, out targetEdges);

        }


        public static bool HasWithinBounds(this RectTransform self, RectTransform target)
        {

            var rtp = self.PositionWithinBounds(target);
            return rtp.horizontalPostion == HorizontalPosition.WITHIN && rtp.verticalPostion == VerticalPosition.WITHIN;

        }

        public static float OverlapPercent(this RectTransform self, RectTransform target)
        {
            RectTransformEdges selfEdges;
            RectTransformEdges targetEdges;

            var rtp = self.PositionWithinBounds(target, out selfEdges, out targetEdges);
            if(rtp.horizontalPostion != HorizontalPosition.WITHIN 
                    && rtp.verticalPostion != VerticalPosition.WITHIN)
            {
                return 0f;
            }

            float maxOverlap = (self.rect.height / 2) + (target.rect.height / 2);

            var overlap = Mathf.Abs(self.position.y - target.position.y);
            return overlap / maxOverlap;
        }
        
        public static Vector2 GetResizeByWidth(this RectTransform self, float width)
        {
            var resizePercent = MathHelpers.GetValuePercent(width, self.sizeDelta.x);
            var resizedHeight = MathHelpers.GetPercentValue(resizePercent, self.sizeDelta.y);
            return new Vector2(width, resizedHeight);
        }
        public static void ResizeByWidth(this RectTransform self, float width)
        {
            self.sizeDelta = self.GetResizeByWidth(width);
        }

        public static Vector2 GetResizeByHeight(this RectTransform self, float height)
        {
            var resizePercent = MathHelpers.GetValuePercent(height, self.sizeDelta.y);
            var resizedWidth = MathHelpers.GetPercentValue(resizePercent, self.sizeDelta.x);
            return new Vector2(resizedWidth, height);
        }
        public static void ResizeByHeight(this RectTransform self, float height)
        {
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
        public static Vector2 GetScaleToMatch(this RectTransform self, RectTransform to)
        {
            var scaledSize = ScaledSize(to);
            return self.GetScaleToMatch(scaledSize);
        }
        public static Vector2 GetScaleToMatch(this RectTransform self, Vector2 scaledSize)
        {
            var newSize = self.GetResizeByWidth(scaledSize.x);
            if (newSize.y > scaledSize.y)
            {
                newSize = self.GetResizeByHeight(scaledSize.y);
            }
            var newScaleVal = newSize.x / self.sizeDelta.x;
            return new Vector2(newScaleVal, newScaleVal);
        }

        public static Vector2 ScaledSize(this RectTransform self)
        {

            return new Vector2
            {
                x = self.rect.width * self.localScale.x,
                y = self.rect.height * self.localScale.y
            };
        }

        public static Vector2 ShiftUpPosition(this RectTransform self, float offset = 0f)
        {
            return new Vector2(self.position.x, self.position.y + self.ScaledSize().y + offset);
        }

        public static Vector2 ShiftUpLocalPosition(this RectTransform self, float offset = 0f)
        {
            return new Vector2(self.localPosition.x, self.localPosition.y + self.ScaledSize().y + offset);
        }

        public static Vector2 ShiftDownPosition(this RectTransform self, float offset = 0f)
        {
            return new Vector2(self.position.x, self.position.y - (self.ScaledSize().y + offset));
        }

        public static Vector2 ShiftDownLocalPosition(this RectTransform self, float offset = 0f)
        {
            return new Vector2(self.localPosition.x, self.localPosition.y - (self.ScaledSize().y + offset));
        }

        public static Vector2 ShiftLeftPosition(this RectTransform self, float offset = 0f)
        {
            return new Vector2(self.position.x - (self.ScaledSize().x + offset), self.position.y);
        }
        public static Vector2 ShiftLeftLocalPosition(this RectTransform self, float offset = 0f)
        {
            return new Vector2(self.localPosition.x - (self.ScaledSize().x + offset), self.localPosition.y);
        }

        public static Vector2 ShiftRightPosition(this RectTransform self, float offset = 0f)
        {
            return new Vector2(self.position.x + self.ScaledSize().x + offset, self.position.y);
        }

        public static Vector2 ShiftRightLocalPosition(this RectTransform self, float offset = 0f)
        {
            return new Vector2(self.localPosition.x + self.ScaledSize().x + offset, self.localPosition.y);
        }

        public static Vector2 GetRelativeAnchorPositionOf(this RectTransform self, RectTransform target)
        {
            Vector2 result;
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint(null, target.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(self, screenP, null, out result);
            return result;
        }

        public static void SetAnchor(this RectTransform source, AnchorPresets allign, int offsetX = 0, int offsetY = 0)
        {
            source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

            switch (allign)
            {
                case (AnchorPresets.TopLeft):
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case (AnchorPresets.TopCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 1);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case (AnchorPresets.TopRight):
                    {
                        source.anchorMin = new Vector2(1, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case (AnchorPresets.MiddleLeft):
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(0, 0.5f);
                        break;
                    }
                case (AnchorPresets.MiddleCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 0.5f);
                        source.anchorMax = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case (AnchorPresets.MiddleRight):
                    {
                        source.anchorMin = new Vector2(1, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }

                case (AnchorPresets.BottomLeft):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 0);
                        break;
                    }
                case (AnchorPresets.BottonCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 0);
                        break;
                    }
                case (AnchorPresets.BottomRight):
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case (AnchorPresets.HorStretchTop):
                    {
                        source.anchorMin = new Vector2(0, 1);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
                case (AnchorPresets.HorStretchMiddle):
                    {
                        source.anchorMin = new Vector2(0, 0.5f);
                        source.anchorMax = new Vector2(1, 0.5f);
                        break;
                    }
                case (AnchorPresets.HorStretchBottom):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 0);
                        break;
                    }

                case (AnchorPresets.VertStretchLeft):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(0, 1);
                        break;
                    }
                case (AnchorPresets.VertStretchCenter):
                    {
                        source.anchorMin = new Vector2(0.5f, 0);
                        source.anchorMax = new Vector2(0.5f, 1);
                        break;
                    }
                case (AnchorPresets.VertStretchRight):
                    {
                        source.anchorMin = new Vector2(1, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }

                case (AnchorPresets.StretchAll):
                    {
                        source.anchorMin = new Vector2(0, 0);
                        source.anchorMax = new Vector2(1, 1);
                        break;
                    }
            }
        }

        /// <summary>
        /// Sets the pivot based on the local point of the rectangle <see cref="RectTransformUtility.ScreenPointToLocalPointInRectangle"/>.
        /// Keeps the RectTransform in place when changing the pivot by countering the position change when the pivot is set.
        /// </summary>
        /// <param name="target">The target RectTransform</param>
        /// <param name="localPoint">The local point of the target RectTransform</param>
        public static void SetPivot(this RectTransform target, Vector2 localPoint)
        {
            // Calculate the pivot by normalizing the values
            var targetRect = target.rect;
            var pivotX = (float)((localPoint.x - (double)targetRect.x) / (targetRect.xMax - (double)targetRect.x));
            var pivotY = (float)((localPoint.y - (double)targetRect.y) / (targetRect.yMax - (double)targetRect.y));
            var newPivot = new Vector2(pivotX, pivotY);

            // Delta pivot = (current - new) * scale
            var deltaPivot = (target.pivot - newPivot) * target.transform.localScale;
            // The delta position to add after pivot change is the inversion of the delta pivot change * size of the rect * current scale of the rect
            var rectSize = targetRect.size;
            var deltaPosition = new Vector3(deltaPivot.x * rectSize.x, deltaPivot.y * rectSize.y) * -1f;

            // Set the pivot
            target.pivot = newPivot;
            target.localPosition += deltaPosition;
        }

        public static void SetPivot(this RectTransform source, PivotPresets preset)
        {

            switch (preset)
            {
                case (PivotPresets.TopLeft):
                    {
                        source.pivot = new Vector2(0, 1);
                        break;
                    }
                case (PivotPresets.TopCenter):
                    {
                        source.pivot = new Vector2(0.5f, 1);
                        break;
                    }
                case (PivotPresets.TopRight):
                    {
                        source.pivot = new Vector2(1, 1);
                        break;
                    }

                case (PivotPresets.MiddleLeft):
                    {
                        source.pivot = new Vector2(0, 0.5f);
                        break;
                    }
                case (PivotPresets.MiddleCenter):
                    {
                        source.pivot = new Vector2(0.5f, 0.5f);
                        break;
                    }
                case (PivotPresets.MiddleRight):
                    {
                        source.pivot = new Vector2(1, 0.5f);
                        break;
                    }

                case (PivotPresets.BottomLeft):
                    {
                        source.pivot = new Vector2(0, 0);
                        break;
                    }
                case (PivotPresets.BottomCenter):
                    {
                        source.pivot = new Vector2(0.5f, 0);
                        break;
                    }
                case (PivotPresets.BottomRight):
                    {
                        source.pivot = new Vector2(1, 0);
                        break;
                    }
            }
        }
        public static void SetStretch_Left(this RectTransform source, float left)
        {
            source.offsetMin = new Vector2(left, source.offsetMin.y); // new Vector2(left, bottom);
        }

        public static void SetStretch_Right(this RectTransform source, float right)
        {
            source.offsetMax = new Vector2(-right, source.offsetMax.y); // new Vector2(-right, -top);
        }

        public static void SetStretch_Top(this RectTransform source, float top)
        {
            source.offsetMax = new Vector2(source.offsetMax.x, -top); // new Vector2(-right, -top);
        }
        public static void SetStretch_Bottom(this RectTransform source, float bottom)
        {
            source.offsetMin = new Vector2(source.offsetMin.x, bottom); // new Vector2(left, bottom);
        }
        public static void SetStretch_LeftBottom(this RectTransform source, float left, float bottom)
        {
            source.offsetMin = new Vector2(left, bottom);
        }
        public static void SetStretch_RightTop(this RectTransform source, float right, float top)
        {
            source.offsetMax = new Vector2(-right, -top);
        }

        public static RectTransformLRTB Get_LRTB(this RectTransform source)
        {
            return new RectTransformLRTB(source.offsetMin.x, -source.offsetMax.x, -source.offsetMax.y, source.offsetMin.y);
        }
        public static void Set_LRTB(this RectTransform source, RectTransformLRTB lrtb)
        {
            source.SetStretch_LeftBottom(lrtb.Left, lrtb.Bottom);
            source.SetStretch_RightTop(lrtb.Right, lrtb.Top);
        }

        /// Applies the scale with the mouse pointer in mind
        /// </summary>
        /// <param name="newScaleValue"></param>
        /// <param name="position"></param>
        public static void ScaleAround(this RectTransform target, float newScale,
                                        Vector2 position, Camera camera)
        {
            // Calculate the local point in the rectangle using the event position
            RectTransformUtility.ScreenPointToLocalPointInRectangle(target, position, camera, out var localPointInRect);
            // Set the pivot based on the local point in the rectangle
            SetPivot(target, localPointInRect);

            // Set the new scale
            // scale = newScale;
            // Apply the new scale
            target.localScale = new Vector3(newScale, newScale, newScale);
        }

        public static void ScaleAroundBy(this RectTransform target, float scaleStep,
                                        Vector2 position, Camera camera,
                                        float scaleMin, float scaleMax)
        {

            var newScale = Mathf.Clamp(target.localScale.x + scaleStep, 1 - scaleMin, 1 + scaleMax);
            // If the scale did not change, don't do anything
            if (newScale.Equals(target.localScale)) return;
            ScaleAround(target, newScale, position, camera);
        }

    }

}
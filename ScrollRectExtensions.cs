using UnityEngine;
using UnityEngine.UI;

namespace GameUtil.Extensions
{
    public static class ScrollRectExtensions
    {
        private static Bounds m_ContentBounds;
        private static Bounds m_ViewBounds;
        private static readonly Vector3[] m_Corners = new Vector3[4];
        
        public static void CenterOnItem(this ScrollRect scrollRect, Vector3 pos)
        {
            SetPosition(scrollRect, CalculateCenterOnItem(scrollRect, pos));
        }
        
        public static void CenterOnItem(this ScrollRect scrollRect, GameObject go)
        {
            SetPosition(scrollRect, CalculateCenterOnItem(scrollRect, go));
        }
        
        public static void CenterOnItem(this ScrollRect scrollRect, Component component)
        {
            SetPosition(scrollRect, CalculateCenterOnItem(scrollRect, component));
        }

        private static void SetPosition(ScrollRect scrollRect, Vector2 normalizedPosition)
        {
            if (scrollRect.horizontal)
            {
                normalizedPosition.x = Mathf.Clamp01(normalizedPosition.x);
                scrollRect.horizontalNormalizedPosition = normalizedPosition.x;
            }
            if (scrollRect.vertical)
            {
                normalizedPosition.y = Mathf.Clamp01(normalizedPosition.y);
                scrollRect.verticalNormalizedPosition = normalizedPosition.y;
            }
        }
    
        public static Vector2 CalculateCenterOnItem(this ScrollRect scrollRect, Vector3 pos)
        {
            Vector2 result = new Vector2(scrollRect.horizontalNormalizedPosition, scrollRect.verticalNormalizedPosition);
            UpdateBounds(scrollRect);
            var toLocal = scrollRect.viewport.InverseTransformPoint(pos);
            scrollRect.velocity = Vector2.zero;
            m_ContentBounds.center += m_ViewBounds.center - toLocal;
            if (scrollRect.vertical)
            {
                if (m_ContentBounds.size.y <= m_ViewBounds.size.y)
                {
                    result.y = 0;
                }
                else
                {
                    float val = (m_ViewBounds.min.y - m_ContentBounds.min.y) / (m_ContentBounds.size.y - m_ViewBounds.size.y);
                    result.y = val;
                }
            }
            if (scrollRect.horizontal)
            {
                if (m_ContentBounds.size.x <= m_ViewBounds.size.x)
                {
                    result.x = 0;
                }
                else
                {
                    float val = (m_ViewBounds.min.x - m_ContentBounds.min.x) / (m_ContentBounds.size.x - m_ViewBounds.size.x);
                    result.x = val;
                }
            }
            return result;
        }
        public static Vector2 CalculateCenterOnItem(this ScrollRect scrollRect, GameObject go)
        {
            return CalculateCenterOnItem(scrollRect, go.transform.position);
        }
    
        public static Vector2 CalculateCenterOnItem(this ScrollRect scrollRect, Component component)
        {
            return CalculateCenterOnItem(scrollRect, component.transform.position);
        }

        private static void UpdateBounds(ScrollRect scrollRect)
        {
            m_ViewBounds = new Bounds(scrollRect.viewport.rect.center, scrollRect.viewport.rect.size);
            m_ContentBounds = GetBounds(scrollRect);

            if (scrollRect.content == null)
                return;

            Vector3 contentSize = m_ContentBounds.size;
            Vector3 contentPos = m_ContentBounds.center;
            var contentPivot = scrollRect.content.pivot;
            AdjustBounds(ref m_ViewBounds, ref contentPivot, ref contentSize, ref contentPos);
            m_ContentBounds.size = contentSize;
            m_ContentBounds.center = contentPos;
        }

        private static Bounds GetBounds(ScrollRect scrollRect)
        {
            if (scrollRect.content == null)
                return new Bounds();
            scrollRect.content.GetWorldCorners(m_Corners);
            var viewWorldToLocalMatrix = scrollRect.viewport.worldToLocalMatrix;
            return InternalGetBounds(m_Corners, ref viewWorldToLocalMatrix);
        }

        private static Bounds InternalGetBounds(Vector3[] corners, ref Matrix4x4 viewWorldToLocalMatrix)
        {
            var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int j = 0; j < 4; j++)
            {
                Vector3 v = viewWorldToLocalMatrix.MultiplyPoint3x4(corners[j]);
                vMin = Vector3.Min(v, vMin);
                vMax = Vector3.Max(v, vMax);
            }

            var bounds = new Bounds(vMin, Vector3.zero);
            bounds.Encapsulate(vMax);
            return bounds;
        }
        
        private static void AdjustBounds(ref Bounds viewBounds, ref Vector2 contentPivot, ref Vector3 contentSize, ref Vector3 contentPos)
        {
            // Make sure content bounds are at least as large as view by adding padding if not.
            // One might think at first that if the content is smaller than the view, scrolling should be allowed.
            // However, that's not how scroll views normally work.
            // Scrolling is *only* possible when content is *larger* than view.
            // We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
            // E.g. if pivot is at top, bounds are expanded downwards.
            // This also works nicely when ContentSizeFitter is used on the content.
            Vector3 excess = viewBounds.size - contentSize;
            if (excess.x > 0)
            {
                contentPos.x -= excess.x * (contentPivot.x - 0.5f);
                contentSize.x = viewBounds.size.x;
            }
            if (excess.y > 0)
            {
                contentPos.y -= excess.y * (contentPivot.y - 0.5f);
                contentSize.y = viewBounds.size.y;
            }
        }
    }
}
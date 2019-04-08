
namespace VRUIInput
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    [RequireComponent(typeof(Canvas))]
    public class VRGraphicRaycaster : GraphicRaycaster
    {
        private Canvas m_canvas;
        private Canvas canvas { get { if (m_canvas == null) m_canvas = GetComponent<Canvas>(); return m_canvas; } }

        public override Camera eventCamera => canvas.worldCamera;

        public LayerMask blockingMask { get { return m_BlockingMask; } set { m_BlockingMask = value; } }

#if UNITY_EDITOR
        override protected void Reset()
        {
            base.Reset();

            blockingObjects = BlockingObjects.All;
        }
#endif

        [NonSerialized] private List<Graphic> m_RaycastResults = new List<Graphic>();
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if (canvas.renderMode != RenderMode.WorldSpace) return;
            if (eventCamera == null)
            {
                Debug.LogWarning("Please add VR camera to <b>Event Camera</b> field in your canvas.");
                return;
            }

            var vrData = eventData as VRPointerEventData;
            if (vrData == null) return;
            var ray = vrData.ray;

            m_RaycastResults.Clear();
            GraphicRaycast(canvas, ray, m_RaycastResults);

            var hitDistance = GetPhysicsRaycastDepth(ray);

            for (var index = 0; index < m_RaycastResults.Count; index++)
            {
                var go = m_RaycastResults[index].gameObject;
                bool appendGraphic = true;

                if (ignoreReversedGraphics)
                {
                    var dir = go.transform.rotation * Vector3.forward;
                    appendGraphic = Vector3.Dot(ray.direction, dir) > 0;
                }

                if (appendGraphic)
                {
                    Transform trans = go.transform;
                    Vector3 transForward = trans.forward;
                    // http://geomalgorithms.com/a06-_intersect-2.html
                    float distance = (Vector3.Dot(transForward, trans.position - ray.origin) / Vector3.Dot(transForward, ray.direction));

                    if (distance < 0 || distance >= hitDistance) continue;

                    var castResult = new RaycastResult
                    {
                        gameObject = go,
                        module = this,
                        distance = distance,
                        screenPosition = eventCamera.WorldToScreenPoint(vrData.ray.GetPoint(distance)),
                        index = resultAppendList.Count,
                        depth = m_RaycastResults[index].depth,
                        sortingLayer = canvas.sortingLayerID,
                        sortingOrder = canvas.sortingOrder
                    };
                    resultAppendList.Add(castResult);
                }
            }
        }

        [NonSerialized] static readonly List<Graphic> s_SortedGraphics = new List<Graphic>();
        private void GraphicRaycast(Canvas canvas, Ray ray, List<Graphic> results)
        {
            var foundGraphics = GraphicRegistry.GetGraphicsForCanvas(canvas);
            s_SortedGraphics.Clear();

            for (int i = 0; i < foundGraphics.Count; ++i)
            {
                Graphic graphic = foundGraphics[i];

                // -1 means it hasn't been processed by the canvas, which means it isn't actually drawn
                if (graphic.depth == -1 || !graphic.raycastTarget)
                    continue;

                Vector3 worldPos;
                if (RayIntersectsRectTransform(graphic.rectTransform, ray, out worldPos))
                {
                    var screenPos = eventCamera.WorldToScreenPoint(worldPos);

                    // mask/image intersection - See Unity docs on eventAlphaThreshold for when this does anything
                    if (graphic.Raycast(screenPos, eventCamera))
                        s_SortedGraphics.Add(graphic);
                }
            }

            s_SortedGraphics.Sort((g1, g2) => g2.depth.CompareTo(g1.depth));
            results.AddRange(s_SortedGraphics);
        }

        private float GetPhysicsRaycastDepth(Ray ray)
        {
            if (blockingObjects != BlockingObjects.ThreeD && blockingObjects != BlockingObjects.All)
                return float.MaxValue;

            float dist = 1000;

            if (eventCamera != null)
                dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, dist, m_BlockingMask))
            {
                return hit.distance;
            }

            return float.MaxValue;
        }

        static bool RayIntersectsRectTransform(RectTransform rectTransform, Ray ray, out Vector3 worldPos)
        {
            var plane = new Plane(-rectTransform.forward, rectTransform.position);

            float distance;
            if (!plane.Raycast(ray, out distance))
            {
                worldPos = Vector3.zero;
                return false;
            }

            worldPos = ray.GetPoint(distance);

            var localPos = rectTransform.InverseTransformPoint(worldPos);
            var inRect = rectTransform.rect.Contains(localPos);

            if (!inRect) worldPos = Vector3.zero;

            return inRect;
        }
    }
}
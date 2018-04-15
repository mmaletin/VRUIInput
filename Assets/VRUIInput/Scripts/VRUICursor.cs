
namespace VRUIInput
{
    using UnityEngine;

    [RequireComponent(typeof(LineRenderer))]
    public abstract class VRUICursor<T> : MonoBehaviour where T : Component
    {
        private LineRenderer m_lineRenderer;
        private LineRenderer lineRenderer { get { if (m_lineRenderer == null) m_lineRenderer = GetComponent<LineRenderer>(); return m_lineRenderer; } }

        private VRInputModule<T> m_vrInputModule;
        protected VRInputModule<T> vrInputModule { get { if (m_vrInputModule == null) m_vrInputModule = FindObjectOfType<VRInputModule<T>>(); return m_vrInputModule; } }

        public Renderer dot;
        public int controllerId;
        public float maxRayDistance = 100;
        public float maxScaledDistance { get { return maxRayDistance * transform.lossyScale.x; } }

        private bool initialized = false;

        public bool showForCanvasOnly = false;

        private void Init()
        {
            if (vrInputModule == null) return;

            var controllerTransform = vrInputModule.controllers[controllerId].transform;

            transform.parent = controllerTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            initialized = true;
        }

        protected void OnNewPosesApplied()
        {
            if (!initialized) Init();
            if (!initialized) return;

            var controllerTransform = vrInputModule.controllers[controllerId].transform;

            vrInputModule.UpdateDistance(controllerId);
            float distance = vrInputModule.GetDistance(controllerId);

            if (showForCanvasOnly)
            {
                dot.gameObject.SetActive(lineRenderer.enabled = !float.IsPositiveInfinity(distance) && !GetTeleportationButtonPressed());

                if (distance > maxScaledDistance) distance = maxScaledDistance;
                ApplyDistance(controllerTransform, distance);

                return;
            }

            RaycastHit hit;

            if (Physics.Raycast(new Ray(controllerTransform.position, controllerTransform.forward), out hit))
            {
                if (hit.distance < distance)
                {
                    distance = hit.distance;
                }
            }

            dot.gameObject.SetActive(distance <= maxScaledDistance);
            SetEnabledBasedOnTeleportationButton();

            if (distance > maxScaledDistance) distance = maxScaledDistance;
            ApplyDistance(controllerTransform, distance);
        }

        private void ApplyDistance(Transform controllerTransform, float distance)
        {
            dot.transform.position = controllerTransform.position + controllerTransform.forward * distance;

            lineRenderer.SetPosition(1, Vector3.forward * distance / transform.lossyScale.x);
        }

        private void SetEnabledBasedOnTeleportationButton()
        {
            bool teleportationButtonPressed = GetTeleportationButtonPressed();

            dot.gameObject.SetActive(lineRenderer.enabled = !teleportationButtonPressed);
        }

        protected abstract bool GetTeleportationButtonPressed();
    }
}
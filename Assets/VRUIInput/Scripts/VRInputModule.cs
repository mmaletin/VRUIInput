
namespace VRUIInput
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;

    // cursor uses controllers array, GetDistance and UpdateDistance
    public abstract class VRInputModule<T> : StandaloneInputModule where T : Component
    {
        private readonly Dictionary<int, VRPointerEventData> pointerDataDictionary = new Dictionary<int, VRPointerEventData>();

        private VRPointerEventData m_distanceAnalisysEventData; // This pointer event data is used when distance needs to be updated after poses are applied
        private VRPointerEventData distanceAnalisysEventData
        {
            get
            {
                if (m_distanceAnalisysEventData == null)
                    m_distanceAnalisysEventData = new VRPointerEventData(eventSystem);
                return m_distanceAnalisysEventData;
            }
        }

        public T[] controllers;
        protected float[] distances;
        protected GameObject[] gameObjects;

        protected override void Start()
        {
            base.Start();

            distances = new float[controllers.Length];
            gameObjects = new GameObject[controllers.Length];
        }

        public float GetDistance(int id) { return distances[id]; }
        public GameObject GetGameObject(int id) { return gameObjects[id]; }

        public void UpdateRaycast(int id)
        {
            var controllerTransform = controllers[id].transform;
            distanceAnalisysEventData.ray = new Ray(controllerTransform.position, controllerTransform.forward);

            eventSystem.RaycastAll(distanceAnalisysEventData, m_RaycastResultCache);
            var raycast = FindFirstRaycast(m_RaycastResultCache);

            distances[id] = m_RaycastResultCache.Count == 0 ? float.PositiveInfinity : raycast.distance;
            gameObjects[id] = raycast.gameObject;

            m_RaycastResultCache.Clear();
        }

        public override void Process()
        {
            SendUpdateEventToSelectedObject();

            if (eventSystem.sendNavigationEvents)
                SendMoveEventToSelectedObject();

            ProcessControllerEvents();
        }

        private void ProcessControllerEvents()
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                bool released;
                bool pressed;
                var pointer = GetVRPointerEventData(i, out pressed, out released);

                distances[i] = pointer.pointerCurrentRaycast.distance;
                gameObjects[i] = pointer.pointerCurrentRaycast.gameObject;

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
            }
        }

        protected override void ProcessDrag(PointerEventData pointerEvent)
        {
            var press = pointerEvent.pointerPress;
            var pressRaw = pointerEvent.rawPointerPress;
            var eligibleForClick = pointerEvent.eligibleForClick;

            base.ProcessDrag(pointerEvent);

            pointerEvent.pointerPress = press;
            pointerEvent.rawPointerPress = pressRaw;
            pointerEvent.eligibleForClick = eligibleForClick;
        }

        private VRPointerEventData GetVRPointerEventData(int controllerId, out bool pressed, out bool released)
        {
            T controller = controllers[controllerId];

            VRPointerEventData pointerData;
            GetVRPointerData(controllerId, out pointerData, true);

            pointerData.Reset();
            pointerData.ray = new Ray(controller.transform.position, controller.transform.forward);

            if (!GetPressedReleased(controller, out pressed, out released))
            {
                return pointerData;
            }

            pointerData.button = PointerEventData.InputButton.Left;

            Vector2 prevPos = pointerData.position;

            // Event system orders results by depth and sorting layers
            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);

            // Find first raycast where gameobject if not null
            var raycast = FindFirstRaycast(m_RaycastResultCache);

            pointerData.position = raycast.screenPosition;
            PointerDataPositionToDraggedObject(pointerData);

            pointerData.pointerCurrentRaycast = raycast;
            pointerData.delta = pointerData.position - prevPos;

            m_RaycastResultCache.Clear();
            return pointerData;
        }

        /// <summary>
        /// Gets pressed/released from a corresponding VR api. Returns true if controller is valid.
        /// </summary>
        protected abstract bool GetPressedReleased(T controller, out bool pressed, out bool released);

        private static void PointerDataPositionToDraggedObject(VRPointerEventData pointerData)
        {
            if (pointerData.pointerDrag != null)
            {
                var vrData = pointerData as VRPointerEventData;
                var plane = new Plane(vrData.pointerDrag.transform.forward, vrData.pointerDrag.transform.position);
                float distance;

                if (plane.Raycast(vrData.ray, out distance))
                {
                    var worldPos = vrData.ray.GetPoint(distance);
                    pointerData.position = vrData.pressEventCamera.WorldToScreenPoint(worldPos);
                }
            }
        }

        bool GetVRPointerData(int controllerId, out VRPointerEventData data, bool create)
        {
            if (!pointerDataDictionary.TryGetValue(controllerId, out data) && create)
            {
                data = new VRPointerEventData(eventSystem)
                {
                    pointerId = controllerId
                };
                pointerDataDictionary.Add(controllerId, data);
                return true;
            }
            return false;
        }
    }
}
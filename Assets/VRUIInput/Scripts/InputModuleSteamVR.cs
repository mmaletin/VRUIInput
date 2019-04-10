
namespace VRUIInput
{
    using System.Linq;
    using UnityEngine;
    using Valve.VR;

    public class InputModuleSteamVR : VRInputModule<SteamVR_Behaviour_Pose>
    {
        public SteamVR_Action_Boolean interactUIAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "InteractUI");

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            controllers = (from trackedObject in FindObjectsOfType<SteamVR_Behaviour_Pose>() where trackedObject.GetComponent<Camera>() == null select trackedObject).ToArray();
        }
#endif

        public float GetDistance(SteamVR_Behaviour_Pose controller)
        {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] == controller)
                    return distances[i];
            }
            return float.NaN;
        }

        public GameObject GetGameObject(SteamVR_Behaviour_Pose controller) {
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i] == controller)
                    return gameObjects[i];
            }
            return null;
        }

        protected override bool GetPressedReleased(SteamVR_Behaviour_Pose controller, out bool pressed, out bool released)
        {
            int controllerIndex = controller.GetDeviceIndex();
            if (controllerIndex == -1)
            {
                pressed = false;
                released = false;
                return false;
            }

            var action = interactUIAction[controller.inputSource];

            pressed = action.stateDown;
            released = action.stateUp;

            return true;
        }
    }
}
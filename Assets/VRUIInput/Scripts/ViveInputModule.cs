
namespace VRUIInput
{
    using System.Linq;
    using UnityEngine;

    public class ViveInputModule : VRInputModule<SteamVR_TrackedObject>
    {
#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            controllers = (from trackedObject in FindObjectsOfType<SteamVR_TrackedObject>() where trackedObject.GetComponent<Camera>() == null select trackedObject).ToArray();
        }
#endif

        protected override bool GetPressedReleased(SteamVR_TrackedObject controller, out bool pressed, out bool released)
        {
            int controllerIndex = (int)controller.index;
            if (controllerIndex == -1)
            {
                pressed = false;
                released = false;
                return false;
            }

            pressed = SteamVR_Controller.Input(controllerIndex).GetHairTriggerDown();
            released = SteamVR_Controller.Input(controllerIndex).GetHairTriggerUp();

            return true;
        }
    }
}
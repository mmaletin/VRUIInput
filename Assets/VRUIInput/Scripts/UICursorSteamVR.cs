
using Valve.VR;

namespace VRUIInput
{
    public class UICursorSteamVR : VRUICursor<SteamVR_Behaviour_Pose>
    {
        private void Awake()
        {
            SteamVR_Events.NewPosesApplied.Listen(OnNewPosesApplied);
        }

        private void OnDestroy()
        {
            SteamVR_Events.NewPosesApplied.Remove(OnNewPosesApplied);
        }

        override protected bool GetTeleportationButtonPressed()
        {
            var controller = vrInputModule.controllers[controllerId];

            bool touchpadPressed = false;
            int openVRId = controller.GetDeviceIndex();
            if (openVRId != -1)
            {
                var action = SteamVR_Actions.default_Teleport[controller.inputSource];
                touchpadPressed = action.state;
            }

            return touchpadPressed;
        }
    }
}
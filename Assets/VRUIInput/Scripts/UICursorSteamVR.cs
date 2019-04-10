
using Valve.VR;

namespace VRUIInput
{
    public class UICursorSteamVR : VRUICursor<SteamVR_Behaviour_Pose>
    {
        private SteamVR_Behaviour_Pose controller;

        public SteamVR_Action_Boolean teleportAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "Teleport");

        private void Awake()
        {
            controller = vrInputModule.controllers[controllerId];            

            SteamVR_Events.NewPosesApplied.Listen(OnNewPosesApplied);
        }

        private void OnDestroy()
        {
            SteamVR_Events.NewPosesApplied.Remove(OnNewPosesApplied);
        }

        override protected bool GetTeleportationButtonPressed()
        {
            if (teleportAction != null && controller.GetDeviceIndex() != -1)
            {
                return teleportAction[controller.inputSource].state;
            }

            return false;
        }
    }
}
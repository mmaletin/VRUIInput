
namespace VRUIInput
{
    public class ViveUICursor : VRUICursor<SteamVR_TrackedObject>
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
            int openVRId = (int)controller.index;
            if (openVRId != -1)
            {
                touchpadPressed = SteamVR_Controller.Input(openVRId).GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
            }

            return touchpadPressed;
        }
    }
}
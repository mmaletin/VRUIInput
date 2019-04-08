# VRUIInput

VRUIInput is a small set of scripts for Unity that allows your world space canvases to receive events from controller rays.

SteamVR is the only supported platform out of the box, but it should be easy enough to add support for other platforms.

Attention! This project will contain errors if you open it in unity. This is expected behaviour, please follow setup process described below:

Setup process:
* Add SteamVR to your project and configure default actions. Don't add it to this project, it won't compile and you won't be able to configure SteamVR actions
* Copy Assets/VRUIInput folder from this repository into your assets folder
* (Optional) Test UI interactions in example scene located in VRUIInput/Example
* Replace standalone input module on event system with vive input module
* Replace graphic raycaster on every world canvas with vr graphic raycaster
* Add camera from VR camera rig to canvases as event camera
* Optionally add vr ui cursor prefabs to your scene, set id values to 0 and 1
* If you are using unity dropdowns, add vr graphic raycaster to template object of every dropdown

This implementation supports every possible ui element, they don’t have to be flat, and rays don’t lag behind controllers.

This version is compatible with Unity 2018.3 and SteamVR 2.2.0. See tags for earlier versions of Unity and SteamVR 1.2.3.
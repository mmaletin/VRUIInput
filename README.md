# VRUIInput

VRUIInput is a small set of scripts for Unity that allows your world space canvases to receive events from controller rays.

Vive is the only supported platform out of the box, but it should be easy enough to add support for other platforms.

Setup is simple:
* Add SteamVR to your project
* Replace standalone input module on event system with vive input module
* Replace graphic raycaster on every world canvas with vr graphic raycaster
* Optionally add vr ui cursor prefabs to your scene, set id values to 0 and 1
* If you are using unity dropdowns, add vr graphic raycaster to template object of every dropdown

That’s it! This implementation supports every possible ui element, they don’t have to be flat, and rays don’t lag behind controllers.
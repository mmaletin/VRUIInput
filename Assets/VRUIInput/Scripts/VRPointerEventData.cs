
namespace VRUIInput
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class VRPointerEventData : PointerEventData
    {
        public Ray ray;

        public VRPointerEventData(EventSystem eventSystem) : base(eventSystem) { }

        public override void Reset()
        {
            base.Reset();
        }
    }
}
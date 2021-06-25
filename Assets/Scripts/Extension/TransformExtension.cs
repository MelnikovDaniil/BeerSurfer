using UnityEngine;

namespace Assets.Scripts.Extension
{
    public static class TransformExtension
    {
        public static Transform FindParentWithTag(this Transform childObject, string tag)
        {
            var t = childObject.transform;
            while (t.parent != null)
            {
                if (t.parent.tag == tag)
                {
                    return t.parent;
                }
                t = t.parent.transform;
            }
            return null; // Could not find a parent with given tag.
        }
    }
}

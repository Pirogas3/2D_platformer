using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class LayerMaskExtensions
    {
        public static bool IsInLayer(this GameObject gameObject, LayerMask layerMask)
        {
            return (layerMask & (1 << gameObject.layer)) != 0;
        }
    }
}

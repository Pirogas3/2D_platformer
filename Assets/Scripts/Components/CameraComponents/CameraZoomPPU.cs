using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Assets.Scripts.Components.CameraComponents
{
    public class CameraZoomPPU : MonoBehaviour
    {
        [SerializeField] private PixelPerfectCamera _pixelPerfectCamera;
        [SerializeField] private float _zoomDuration = 0.5f;
        [SerializeField] private int _zoomedPPU = 32; // новое значение PPU

        private int _defaultPPU;

        private void Awake()
        {
            if (_pixelPerfectCamera == null)
                _pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

            _defaultPPU = _pixelPerfectCamera.assetsPPU;
        }

        [ContextMenu("ZoomIn")]
        public void ZoomIn()
        {
            StartCoroutine(ChangePPU(_defaultPPU, _zoomedPPU));
        }

        [ContextMenu("ZoomOut")]
        public void ZoomOut()
        {
            StartCoroutine(ChangePPU(_zoomedPPU, _defaultPPU));
        }

        private IEnumerator ChangePPU(int from, int to)
        {
            float elapsed = 0f;
            while (elapsed < _zoomDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _zoomDuration;
                int currentPPU = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
                _pixelPerfectCamera.assetsPPU = currentPPU;
                yield return null;
            }
            _pixelPerfectCamera.assetsPPU = to;
        }
    }
}

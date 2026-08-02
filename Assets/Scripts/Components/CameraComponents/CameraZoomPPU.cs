using System.Collections;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Components.CameraComponents
{
    public class CameraZoomPPU : MonoBehaviour
    {
        [SerializeField] private PixelPerfectCamera _pixelPerfectCamera;
        [SerializeField] private float _zoomDuration = 0.5f;
        [SerializeField] private int _zoomedPPU = 32; // новое значение PPU

        [Header("Vignette")]
        [SerializeField] private Volume _volume;
        [SerializeField] private float _maxVignetteIntensity = 0.8f;

        private int _defaultPPU;
        private Vignette _vignette;

        private void Awake()
        {
            if (_pixelPerfectCamera == null)
                _pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

            _defaultPPU = _pixelPerfectCamera.assetsPPU;

            if (_volume != null && _volume.profile.TryGet<Vignette>(out var vignette))
                _vignette = vignette;
        }

        [ContextMenu("ZoomIn")]
        public void ZoomIn()
        {
            StartCoroutine(AnimateZoom(_defaultPPU, _zoomedPPU, 0f, _maxVignetteIntensity));
        }

        [ContextMenu("ZoomOut")]
        public void ZoomOut()
        {
            StartCoroutine(AnimateZoom(_zoomedPPU, _defaultPPU, _maxVignetteIntensity, 0f));
        }

        private IEnumerator AnimateZoom(int fromPPU, int toPPU, float fromVignette, float toVignette)
        {
            float elapsed = 0f;
            while (elapsed < _zoomDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _zoomDuration;

                // Плавное изменение PPU
                int currentPPU = Mathf.RoundToInt(Mathf.Lerp(fromPPU, toPPU, t));
                _pixelPerfectCamera.assetsPPU = currentPPU;

                // Плавное изменение виньетки
                float currentVignette = Mathf.Lerp(fromVignette, toVignette, t);
                if (_vignette != null)
                    _vignette.intensity.value = currentVignette;

                yield return null;
            }

            // Фиксируем конечные значения
            _pixelPerfectCamera.assetsPPU = toPPU;
            if (_vignette != null)
                _vignette.intensity.value = toVignette;
        }
    }
}

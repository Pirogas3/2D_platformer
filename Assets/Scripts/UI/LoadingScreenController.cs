using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.UI
{
    public class LoadingScreenController : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private float _displayDuration = 2f;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _loadingPanel.SetActive(false);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Игнорируем служебные сцены
            if (scene.name == "MainMenu" || scene.name == "Hud" || scene.name == "LoadingScreen")
                return;

            StartCoroutine(ShowAndHideAfterDelay(_displayDuration));
        }

        private IEnumerator ShowAndHideAfterDelay(float delay)
        {
            _loadingPanel.SetActive(true);
            yield return new WaitForSeconds(delay);
            _loadingPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}

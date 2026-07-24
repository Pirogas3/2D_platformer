using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.Hud
{
    public class EscController : MonoBehaviour
    {
        [SerializeField] protected GameObject _windowsContainer;
        [SerializeField] protected GameObject _menuPrefab;

        private List<GameObject> _openedWindows = new List<GameObject>(); // стек окон
        private bool _isMenuOpen = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleEscape();
            }
        }

        public void OnShowMenu()
        {
            if (_isMenuOpen)
            {
                // Закрыть все окна и скрыть контейнер
                var windowsToDestroy = new List<GameObject>(_openedWindows);
                _openedWindows.Clear();
                foreach (var window in windowsToDestroy)
                {
                    Destroy(window);
                }
                _windowsContainer.SetActive(false);
                _isMenuOpen = false;
                return;
            }

            _windowsContainer.SetActive(true);
            GameObject menu = Instantiate(_menuPrefab, _windowsContainer.transform);
            RegisterWindow(menu);
            _isMenuOpen = true;
        }

        /// <summary>
        /// Регистрирует окно в стеке (вызывается при создании любого окна меню).
        /// </summary>
        public void RegisterWindow(GameObject window)
        {
            if (!_openedWindows.Contains(window))
                _openedWindows.Add(window);

            // Подписываемся на уничтожение окна, чтобы удалить его из стека
            var closer = window.GetComponent<WindowCloser>();
            if (closer == null)
                closer = window.AddComponent<WindowCloser>();
            closer.OnWindowDestroyed += UnregisterWindow;
        }

        /// <summary>
        /// Удаляет окно из стека (вызывается при его уничтожении).
        /// </summary>
        public void UnregisterWindow(GameObject window)
        {
            if (_openedWindows.Contains(window))
                _openedWindows.Remove(window);

            // Если стек опустел, значит меню закрыто
            if (_openedWindows.Count == 0)
            {
                _isMenuOpen = false;
                _windowsContainer.SetActive(false);
            }
        }

        /// <summary>
        /// Закрывает последнее открытое окно (по Esc).
        /// </summary>
        private void HandleEscape()
        {
            if (_openedWindows.Count == 0) return;

            int lastIndex = _openedWindows.Count - 1;
            GameObject lastWindow = _openedWindows[lastIndex];
            _openedWindows.RemoveAt(lastIndex);

            Destroy(lastWindow);

            // Если стек опустел, скрываем контейнер
            if (_openedWindows.Count == 0)
            {
                _isMenuOpen = false;
                _windowsContainer.SetActive(false);
            }
        }

        /// <summary>
        /// Закрыть конкретное окно (вызывается из кнопки "Закрыть").
        /// </summary>
        public void CloseWindow(GameObject window)
        {
            if (_openedWindows.Contains(window))
            {
                _openedWindows.Remove(window);
                Destroy(window);
                if (_openedWindows.Count == 0)
                {
                    _isMenuOpen = false;
                    _windowsContainer.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Компонент, который уведомляет EscController об уничтожении окна.
    /// Автоматически добавляется на окно при регистрации.
    /// </summary>
    public class WindowCloser : MonoBehaviour
    {
        public event System.Action<GameObject> OnWindowDestroyed;

        private void OnDestroy()
        {
            OnWindowDestroyed?.Invoke(gameObject);
        }
    }
}

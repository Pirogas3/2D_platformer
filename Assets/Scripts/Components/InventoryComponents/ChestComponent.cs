using Assets.Scripts.UI.Hud.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Components.InventoryComponents
{
    public class ChestComponent : InteractableComponent
    {
        [SerializeField] private InventoryComponent _inventoryComponent;
        [SerializeField] private float _closeDistance;
        [SerializeField] private UnityEvent _onOpened;
        [SerializeField] SpriteAnimationComponent _animationComponent;
        [SerializeField] private string _uniqueId;

        private bool _isOpened;
        private ChestInventoryController _chestUIController;
        private GameObject _hero;

        public InventoryComponent InventoryComponent => _inventoryComponent;
        public string UniqueId => _uniqueId;

        private void Awake()
        {
            if (string.IsNullOrEmpty(_uniqueId))
            {
                // Генерируем ID на основе позиции (округляем до 1 знака)
                _uniqueId = $"Chest_{transform.position.x:F1}_{transform.position.y:F1}";
            }
        }

        private void Start()
        {
            _chestUIController = FindObjectOfType<ChestInventoryController>();
            if (_chestUIController == null)
                Debug.LogError("ChestInventoryController не найден в сцене!");

            _isOpened = false;
        }

        private void Update()
        {
            if (_chestUIController != null && _chestUIController.IsOpen)
            {
                if (_hero != null)
                {
                    float distance = Vector2.Distance(transform.position, _hero.transform.position);
                    if (distance > _closeDistance)
                        CloseChest();
                }
            }

            if (_chestUIController != null && !_chestUIController.IsOpen)
                CloseChest();
        }

        public override void Interact(GameObject target)
        {
            _hero = target;
            _action?.Invoke(target);
        }

        public void OpenChest()
        {
            if (_isOpened == false)
                _animationComponent.Play("Opening");

            ToggleUIInv();

            _isOpened = true;
            _onOpened?.Invoke();
        }

        public void CloseChest()
        {
            if (_isOpened == true)
                _animationComponent.Play("Closing");

            _isOpened = false;
            _hero = null;
            if (_chestUIController.IsOpen)
                _chestUIController.Close();
        }

        public void ToggleUIInv()
        {
            if (_chestUIController == null) return;

            if (_chestUIController.IsOpen)
            {
                _chestUIController.Close();
                return;
            }

            _chestUIController.Open(_inventoryComponent.Data);
        }
    }
}

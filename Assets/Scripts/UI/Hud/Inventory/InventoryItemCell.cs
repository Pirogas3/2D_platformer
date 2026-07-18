using Assets.Scripts.Model.Data;
using Assets.Scripts.Model.Definitions;
using Assets.Scripts.UI.Hud.QucikInventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Hud.Inventory
{
    public class InventoryItemCell : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Main")]
        [SerializeField] private Image _icon;
        [SerializeField] private Text _countText;
        [SerializeField] private CanvasGroup _canvasGroup; // для прозрачности всей ячейки

        [Header("Selection")]
        [SerializeField] private Image _selectionImage;
        [SerializeField] private float _hoverAlpha = 0.5f;

        private InventoryItemData _itemData;
        private ItemDef _itemDef;
        private int _slotIndex;
        private IInventoryController _controller;
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private GameObject _dragGhost;
        private bool _isDragging;
        //private bool _isPointerOver = false;

        public InventoryItemData GetItemData() => _itemData;

        public void Initialize(int slotIndex, IInventoryController controller)
        {
            _slotIndex = slotIndex;
            _controller = controller;
            _canvas = GetComponentInParent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Setup(InventoryItemData itemData)
        {
            if (itemData == null)
            {
                Clear();
                return;
            }

            _itemData = itemData;
            _itemDef = DefsFacade.Instance.Items.Get(itemData.Id);
            if (_itemDef.IsVoid)
            {
                Clear();
                return;
            }

            _icon.sprite = _itemDef.Icon;
            _icon.enabled = true;

            if (_itemData.Value > 1)
            {
                _countText.text = _itemData.Value.ToString();
                _countText.gameObject.SetActive(true);
            }
            else
            {
                _countText.gameObject.SetActive(false);
            }

            // Сброс подсветки
            if (_selectionImage != null)
                _selectionImage.gameObject.SetActive(false);

            // Сброс прозрачности
            SetAlpha(1f);
        }

        public void Clear()
        {
            _icon.sprite = null;
            _icon.enabled = false;
            _countText.gameObject.SetActive(false);
            _itemData = null;
            _itemDef = null;
            if (_selectionImage != null)
                _selectionImage.gameObject.SetActive(false);
            SetAlpha(1f);
        }

        public void SetSelected(bool selected)
        {
            if (_selectionImage != null)
                _selectionImage.gameObject.SetActive(selected);
        }

        private void SetAlpha(float alpha)
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = alpha;
            }
            else
            {
                // Fallback: меняем альфа иконки и текста
                if (_icon != null)
                {
                    Color c = _icon.color;
                    c.a = alpha;
                    _icon.color = c;
                }
                if (_countText != null)
                {
                    Color c = _countText.color;
                    c.a = alpha;
                    _countText.color = c;
                }
            }
        }

        // --- Клик ---
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (_itemData != null && _controller != null)
                {
                    ContextMenuManager.ShowMenu(
                        eventData.position,
                        _itemData,
                        this,
                        _slotIndex,
                        _controller,
                        null
                    );
                }
                return;
            }
        }

        // --- Подсветка при наведении ---
        public void OnPointerEnter(PointerEventData eventData)
        {
            //_isPointerOver = true;
            if (_selectionImage == null) return;

            if (!_isDragging)
            {
                // Обычное наведение (не во время перетаскивания)
                Color c = _selectionImage.color;
                c.a = _hoverAlpha;
                _selectionImage.color = c;
                _selectionImage.gameObject.SetActive(true);
            }
            else if (_isDragging)
            {
                // Подсветка целевой ячейки при перетаскивании
                Color c = _selectionImage.color;
                c.a = _hoverAlpha; // можно использовать другое значение, если хотите, но пока оставим одинаковое
                _selectionImage.color = c;
                _selectionImage.gameObject.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //_isPointerOver = false;
            if (_selectionImage != null)
                _selectionImage.gameObject.SetActive(false);
        }

        // --- Drag & Drop ---
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_itemData == null || _controller == null) return;

            _isDragging = true;
            _controller.OnBeginDrag(_slotIndex);
            SetAlpha(0.3f); // делаем ячейку полупрозрачной

            // Создаём призрак
            _dragGhost = new GameObject("DragGhost");
            _dragGhost.transform.SetParent(_canvas.transform, false);
            _dragGhost.transform.SetAsLastSibling();

            var ghostImage = _dragGhost.AddComponent<Image>();
            ghostImage.sprite = _icon.sprite;
            ghostImage.raycastTarget = false;
            ghostImage.maskable = false;

            var ghostRect = _dragGhost.GetComponent<RectTransform>();
            ghostRect.sizeDelta = _rectTransform.sizeDelta;
            ghostRect.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging || _dragGhost == null) return;
            _dragGhost.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            SetAlpha(1f);
            _isDragging = false;

            if (_dragGhost != null)
            {
                Destroy(_dragGhost);
                _dragGhost = null;
            }

            GameObject targetObject = eventData.pointerEnter;
            if (targetObject == null)
            {
                _controller.OnEndDrag();
                if (_selectionImage != null) _selectionImage.gameObject.SetActive(false);
                return;
            }

            // Поиск компонентов
            InventoryItemCell targetCell = targetObject.GetComponent<InventoryItemCell>();
            if (targetCell == null) targetCell = targetObject.GetComponentInParent<InventoryItemCell>();

            QuickInventorySlot quickSlot = targetObject.GetComponent<QuickInventorySlot>();
            if (quickSlot == null) quickSlot = targetObject.GetComponentInParent<QuickInventorySlot>();

            ChestInventoryController chestController = targetObject.GetComponentInParent<ChestInventoryController>();
            InventoryWindowController playerController = targetObject.GetComponentInParent<InventoryWindowController>();

            QuickInventoryController quickController = quickSlot?.GetComponentInParent<QuickInventoryController>();

            // Определяем, над каким окном находится мышь
            bool isOverChest = chestController != null &&
                               (targetObject == chestController.Window ||
                                targetObject.transform.IsChildOf(chestController.Window.transform));

            bool isOverPlayer = playerController != null &&
                                (targetObject == playerController.Window ||
                                 targetObject.transform.IsChildOf(playerController.Window.transform));

            // Приоритет: ячейка внутри инвентаря > быстрый слот > сундук > инвентарь игрока
            if (targetCell != null && targetCell != this)
            {
                // Обмен внутри одного инвентаря
                _controller.OnDrop(_slotIndex, targetCell._slotIndex);
            }
            else if (quickSlot != null && quickController != null)
            {
                // Быстрый слот (только из инвентаря игрока)
                if (!(_controller is ChestInventoryController) && _itemData != null)
                {
                    quickController.TryAssignItem(quickSlot.GetSlotIndex(), _itemData);
                }
            }
            else if (isOverChest)
            {
                // Перемещение в сундук
                if (_itemData != null)
                {
                    chestController.MoveFromOutside(_controller.GetInventoryData(), _slotIndex);
                }
            }
            else if (isOverPlayer)
            {
                // Перемещение в инвентарь игрока (из сундука или другого источника)
                if (!ReferenceEquals(_controller, playerController) && _itemData != null)
                {
                    playerController.MoveFromOutside(_controller.GetInventoryData(), _slotIndex);
                }
            }
            // Иначе ничего не делаем

            _controller.OnEndDrag();
            if (_selectionImage != null)
                _selectionImage.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_dragGhost != null)
                Destroy(_dragGhost);
        }
    }
}

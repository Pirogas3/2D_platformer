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
        private InventoryWindowController _controller;
        private Canvas _canvas;
        private RectTransform _rectTransform;
        private GameObject _dragGhost;
        private bool _isDragging;
        private bool _isPointerOver = false;

        public void Initialize(int slotIndex, InventoryWindowController controller)
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
            _isPointerOver = true;
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
            _isPointerOver = false;
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

            // Восстанавливаем прозрачность
            SetAlpha(1f);
            _isDragging = false;

            // Уничтожаем призрак
            if (_dragGhost != null)
            {
                Destroy(_dragGhost);
                _dragGhost = null;
            }

            // Поиск целевого объекта (ячейка инвентаря или слот быстрого доступа)
            InventoryItemCell targetCell = null;
            QuickInventorySlot quickSlot = null;
            QuickInventoryController quickController = null;

            foreach (var go in eventData.hovered)
            {
                // Проверяем, не является ли объект или его родитель ячейкой инвентаря
                if (targetCell == null)
                {
                    var cell = go.GetComponent<InventoryItemCell>();
                    if (cell == null) cell = go.GetComponentInParent<InventoryItemCell>();
                    if (cell != null && cell != this) targetCell = cell;
                }

                // Проверяем, не является ли объект или его родитель слотом быстрого доступа
                if (quickSlot == null)
                {
                    var slot = go.GetComponent<QuickInventorySlot>();
                    if (slot == null) slot = go.GetComponentInParent<QuickInventorySlot>();
                    if (slot != null)
                    {
                        quickSlot = slot;
                        quickController = slot.GetComponentInParent<QuickInventoryController>();
                    }
                }

                // Если нашли и то, и другое – можно выйти раньше
                if (targetCell != null && quickSlot != null) break;
            }

            // Если нашли ячейку инвентаря – вызываем обработку в контроллере инвентаря
            if (targetCell != null)
            {
                _controller.OnDrop(_slotIndex, targetCell._slotIndex);
            }
            // Иначе если нашли слот быстрого доступа – пытаемся назначить предмет в него
            else if (quickSlot != null && quickController != null)
            {
                if (_itemData != null)
                {
                    quickController.TryAssignItem(quickSlot.GetSlotIndex(), _itemData);
                }
            }

            // Уведомляем контроллер инвентаря, что перетаскивание завершено
            _controller.OnEndDrag();

            // Сбрасываем подсветку (если была включена)
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

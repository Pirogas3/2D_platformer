using Assets.Scripts.Model.Data;
using Assets.Scripts.UI.Hud.Inventory;
using Assets.Scripts.UI.Hud.QucikInventory;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI.Hud
{
    public static class ContextMenuManager
    {
        private static InventoryContextMenu _currentMenu;
        private static GameObject _menuPrefab;
        private static Canvas _dynamicCanvas;

        public static void Initialize(GameObject menuPrefab, Canvas dynamicCanvas)
        {
            _menuPrefab = menuPrefab;
            _dynamicCanvas = dynamicCanvas;
        }

        public static void ShowMenu(Vector2 screenPosition, InventoryItemData itemData, object source, int slotIndex,
                            InventoryWindowController inventoryController,
                            QuickInventoryController quickController)
        {
            CloseMenu();

            if (_menuPrefab == null || _dynamicCanvas == null)
            {
                Debug.LogError("ContextMenuManager not initialized or prefab missing!");
                return;
            }

            GameObject menuGO = Object.Instantiate(_menuPrefab, _dynamicCanvas.transform, false);
            _currentMenu = menuGO.GetComponent<InventoryContextMenu>();
            if (_currentMenu == null)
            {
                Debug.LogError("InventoryContextMenu component not found on prefab!");
                Object.Destroy(menuGO);
                return;
            }

            RectTransform panelRect = _currentMenu.PanelRect;
            if (panelRect == null)
                panelRect = _currentMenu.GetComponent<RectTransform>();

            // Pivot и anchor — левый верхний угол (0,1)
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);

            RectTransform canvasRect = _dynamicCanvas.transform as RectTransform;
            Canvas.ForceUpdateCanvases();
            Vector2 menuSize = panelRect.rect.size;

            // Локальные координаты курсора от верхнего левого угла Canvas
            float canvasWidth = canvasRect.rect.width;
            float canvasHeight = canvasRect.rect.height;
            float cursorLocalX = (screenPosition.x / Screen.width) * canvasWidth;
            float cursorLocalY = (1 - screenPosition.y / Screen.height) * canvasHeight;

            float offset = 10;

            // Расчёт X
            float posX = cursorLocalX + offset;
            if (posX + menuSize.x > canvasWidth)
                posX = cursorLocalX - menuSize.x - offset;
            posX = Mathf.Clamp(posX, 0, canvasWidth - menuSize.x);

            // Расчёт Y
            float posY = cursorLocalY + offset; // пытаемся разместить ниже курсора
            if (posY + menuSize.y > canvasHeight)
                posY = cursorLocalY - menuSize.y - offset; // если не хватает места, размещаем выше
            posY = Mathf.Clamp(posY, 0, canvasHeight - menuSize.y);

            // Устанавливаем позицию: для pivot (0,1) anchoredPosition.y = -posY (так как Y положителен вниз)
            panelRect.anchoredPosition = new Vector2(posX, -posY);

            _currentMenu.Setup(itemData, source, slotIndex, inventoryController, quickController);
        }

        public static void CloseMenu()
        {
            if (_currentMenu != null)
            {
                Object.Destroy(_currentMenu.gameObject);
                _currentMenu = null;
            }
        }

        public static bool IsPointerOverMenu()
        {
            if (_currentMenu == null) return false;
            var eventSystem = EventSystem.current;
            if (eventSystem == null) return false;

            var pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            var results = new List<RaycastResult>();
            eventSystem.RaycastAll(pointerEventData, results);

            foreach (var result in results)
            {
                // Проверяем, принадлежит ли объект меню (или его родитель) InventoryContextMenu
                if (result.gameObject.GetComponentInParent<InventoryContextMenu>() != null)
                    return true;
            }
            return false;
        }

        public static void HandleGlobalClick()
        {
            if (!IsMenuOpen) return;
            if (!IsPointerOverMenu())
            {
                CloseMenu();
            }
        }

        public static bool IsMenuOpen => _currentMenu != null;
    }
}

using Assets.Scripts.Model.Data;

namespace Assets.Scripts.UI.Hud.Inventory
{
    public interface IInventoryController
    {
        void OnBeginDrag(int fromIndex);
        void OnEndDrag();
        void OnDrop(int fromIndex, int toIndex);
        InventoryData GetInventoryData();
        void RefreshUI();
    }
}

namespace Assets.Scripts.Model.Data
{
    public interface ICanAddInInventory
    {
        public void AddInInventory(string id, int amount);

        public void SmartAddInInventory(string id, int amount);
    }
}

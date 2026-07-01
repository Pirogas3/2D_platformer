namespace Assets.Scripts.Model.Data.Properties
{
    public abstract class PrefsPersistentProperty <TPropertyType> : PersistentProperty<TPropertyType>
    {
        protected string _key;

        protected PrefsPersistentProperty(TPropertyType defaultValue, string key) : base(defaultValue)
        {
            _key = key;
        }
    }
}

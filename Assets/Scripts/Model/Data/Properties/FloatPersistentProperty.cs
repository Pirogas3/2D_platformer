using System;
using UnityEngine;

namespace Assets.Scripts.Model.Data.Properties
{
    [Serializable]
    public class FloatPersistentProperty : PrefsPersistentProperty<float>
    {
        public FloatPersistentProperty(float defaultValue, string key) : base(defaultValue, key)
        {
            Init();
        }

        protected override void Write(float value)
        {
            PlayerPrefs.SetFloat(_key, value);
            PlayerPrefs.Save();
        }

        protected override float Read(float defaultValue)
        {
            return PlayerPrefs.GetFloat(_key, defaultValue);
        }
    }
}

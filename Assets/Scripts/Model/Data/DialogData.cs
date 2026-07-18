using SheetXExample;
using System;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class DialogData
    {
        [SerializeField] private string[] _sentences; // fallback (старые строки)
        [SerializeField] private string[] _keys;      // ключи для локализации

        /// <summary>
        /// Возвращает массив строк для отображения с учётом локализации.
        /// Если заданы ключи – использует перевод через LocalizationDialogue.
        /// Иначе использует _sentences как fallback.
        /// </summary>
        public string[] GetSentences()
        {
            if (_keys != null && _keys.Length > 0)
            {
                var result = new string[_keys.Length];
                for (int i = 0; i < _keys.Length; i++)
                {
                    // Пытаемся получить перевод
                    var translated = LocalizationDialogue.Get(_keys[i]).ToString();
                    // Если перевод пустой или ключ не найден, используем сам ключ
                    result[i] = !string.IsNullOrEmpty(translated) ? translated : _keys[i];
                }
                return result;
            }

            // Fallback: используем старые строки
            return _sentences ?? new string[0];
        }
    }
}

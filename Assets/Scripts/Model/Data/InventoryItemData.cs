using Assets.Scripts.Model.Definitions;
using System;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class InventoryItemData
    {
        [InventoryId] public string Id; // Уникальный идентификатор предмета (соответствует Id в ItemDef)
        public int Value; // Количество предметов в слоте (для контейнеров всегда 1)

        // Идентификатор контейнера (если предмет является контейнером), иначе пустая строка
        [SerializeField] private string _containerId;
        public string ContainerId => _containerId;

        public bool IsContainer => !string.IsNullOrEmpty(_containerId); // Является ли данный предмет контейнером (сумкой, колчаном и т.п.)

        /// <summary>
        /// Конструктор для обычного предмета (не контейнера).
        /// </summary>
        /// <param name="id">Идентификатор предмета.</param>
        /// <param name="value">Начальное количество (по умолчанию 0).</param>
        public InventoryItemData(string id, int value = 0)
        {
            Id = id;
            Value = value;
            _containerId = string.Empty;
        }

        /// <summary>
        /// Конструктор для предмета-контейнера.
        /// </summary>
        /// <param name="id">Идентификатор предмета.</param>
        /// <param name="containerId">Уникальный идентификатор экземпляра контейнера.</param>
        /// <param name="value">Количество (всегда 1 для контейнера).</param>
        public InventoryItemData(string id, string containerId, int value = 1)
        {
            Id = id;
            Value = value;
            _containerId = containerId;
        }

        /// <summary>
        /// Инициализировать предмет как контейнер, присвоив ему уникальный идентификатор.
        /// Используется при создании контейнера (например, при подборе сумки).
        /// </summary>
        /// <param name="containerId">Уникальный идентификатор контейнера.</param>
        public void InitializeContainer(string containerId)
        {
            if (string.IsNullOrEmpty(containerId)) return;
            _containerId = containerId;
            Value = 1; // контейнер всегда нестакаемый
        }

        /// <summary>
        /// Получить вес самого предмета (без учёта содержимого, если это контейнер).
        /// </summary>
        /// <returns>Вес в условных единицах.</returns>
        public float GetSelfWeight()
        {
            var def = DefsFacade.Instance.Items.Get(Id);
            return def.IsVoid ? 0f : def.Weight * Value;
        }
    }
}

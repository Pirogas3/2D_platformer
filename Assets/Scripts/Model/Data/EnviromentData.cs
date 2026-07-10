using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class EnviromentData
    {
        [Header("CheckPoint")]
        [SerializeField] private List<int> _checkPointIds = new List<int>();

        /// <summary>
        /// Проверить, активирован ли чекпоинт с указанным ID.
        /// </summary>
        public bool IsCheckPointActivated(int id)
        {
            return _checkPointIds.Contains(id);
        }

        /// <summary>
        /// Активировать чекпоинт (добавить ID в список).
        /// </summary>
        public void ActivateCheckPoint(int id)
        {
            if (!_checkPointIds.Contains(id))
            {
                _checkPointIds.Add(id);
            }
        }
    }
}

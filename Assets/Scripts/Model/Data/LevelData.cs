using System;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class LevelData
    {
        [SerializeField] private int _currentExp = 0;
        [SerializeField] private int _level = 1;
        [SerializeField] private int _availablePoints = 0;

        public int CurrentExp => _currentExp;
        public int Level => _level;
        public int AvailablePoints => _availablePoints;

        public event Action<int> OnLevelUp;        // новый уровень
        public event Action<int> OnExpChanged;     // текущий опыт
        public event Action<int> OnPointsChanged;  // доступные очки параметров (если нужно)

        // Константы для прогрессии
        private const int BaseExpToLevel = 500;
        private const float ExpGrowthRate = 1.1f; // 10% роста

        private int GetExpToNextLevel()
        {
            return (int)(BaseExpToLevel * Mathf.Pow(ExpGrowthRate, _level - 1));
        }

        public void AddExp(int amount)
        {
            if (amount <= 0) return;
            _currentExp += amount;
            OnExpChanged?.Invoke(_currentExp);

            // Проверка на повышение уровня
            while (_currentExp >= GetExpToNextLevel())
            {
                int required = GetExpToNextLevel(); // сохраняем требуемый опыт для текущего уровня
                _currentExp -= required;
                _level++;
                _availablePoints++;
                OnLevelUp?.Invoke(_level);
                OnPointsChanged?.Invoke(_availablePoints);
                OnExpChanged?.Invoke(_currentExp);
            }
        }

        public bool SpendPoints(int count)
        {
            if (count <= 0 || _availablePoints < count) return false;
            _availablePoints -= count;
            OnPointsChanged?.Invoke(_availablePoints);
            return true;
        }

        public int GetExpRequiredForNextLevel()
        {
            return GetExpToNextLevel();
        }

        public float GetProgressToNextLevel()
        {
            int required = GetExpToNextLevel();
            if (required <= 0) return 0f;
            return Mathf.Clamp01((float)_currentExp / required);
        }

        // Для отладки/сброса
        public void Reset()
        {
            _currentExp = 0;
            _level = 1;
            _availablePoints = 0;
            OnExpChanged?.Invoke(0);
            OnLevelUp?.Invoke(1);
            OnPointsChanged?.Invoke(0);
        }
    }
}

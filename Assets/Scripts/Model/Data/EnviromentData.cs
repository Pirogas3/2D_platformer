using Assets.Scripts.Components;
using Assets.Scripts.Components.InventoryComponents;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model.Data
{
    [Serializable]
    public class ChestSaveData
    {
        public string Id;
        public InventoryData Inventory = new InventoryData();
    }

    [Serializable]
    public class ObjectStateData
    {
        public string Id;
        public bool IsActive;
        public float PosX;
        public float PosY;
    }

    [Serializable]
    public class EnviromentData
    {
        [Header("CheckPoint")]
        [SerializeField] private List<string> _checkPointIds = new List<string>();

        public bool IsCheckPointActivated(string id)
        {
            return _checkPointIds.Contains(id);
        }

        public void ActivateCheckPoint(string id)
        {
            if (!_checkPointIds.Contains(id))
            {
                _checkPointIds.Add(id);
            }
        }

        [Header("Chests")]
        [SerializeField] private List<ChestSaveData> _chests = new List<ChestSaveData>();

        public void SaveChests(IEnumerable<ChestComponent> chests)
        {
            _chests.Clear();
            foreach (var chest in chests)
            {
                var data = new ChestSaveData
                {
                    Id = chest.UniqueId,
                    Inventory = chest.InventoryComponent.Data
                };
                _chests.Add(data);
            }
        }

        public void LoadChests()
        {
            var chests = UnityEngine.Object.FindObjectsOfType<ChestComponent>();
            foreach (var chest in chests)
            {
                var saved = _chests.Find(c => c.Id == chest.UniqueId);
                if (saved != null)
                {
                    chest.InventoryComponent.Data.CopyFrom(saved.Inventory);
                }
            }
        }

        [Header("Persistent Objects")]
        [SerializeField] private List<ObjectStateData> _objectStates = new List<ObjectStateData>();

        public void RegisterObjectState(string id, bool isActive)
        {
            var existing = _objectStates.Find(s => s.Id == id);
            if (existing != null)
            {
                existing.IsActive = isActive;
            }
            else
            {
                _objectStates.Add(new ObjectStateData { Id = id, IsActive = isActive });
            }
        }

        public void RegisterObjectState(string id, bool isActive, float posX, float posY)
        {
            var existing = _objectStates.Find(s => s.Id == id);
            if (existing != null)
            {
                existing.IsActive = isActive;
                existing.PosX = posX;
                existing.PosY = posY;
            }
            else
            {
                _objectStates.Add(new ObjectStateData { Id = id, IsActive = isActive, PosX = posX, PosY = posY });
            }
        }

        public Vector2 GetPosObjectState(string id)
        {
            var existing = _objectStates.Find(s => s.Id == id);
            if (existing != null)
            {
                return new Vector2(existing.PosX, existing.PosY);
            }
            return Vector2.zero;
        }

        public void ApplyObjectStates()
        {
            var objects = UnityEngine.Object.FindObjectsOfType<PersistentObjectState>(true);
            foreach (var obj in objects)
            {
                var saved = _objectStates.Find(s => s.Id == obj.UniqueId);
                if (saved != null)
                {
                    if (saved.IsActive)
                        obj.gameObject.SetActive(true);
                    else
                    {
                        if (obj.DestroyOnDeactivate)
                        {
                            var destObj = obj.GetComponent<DestroyAndRegistry>();
                            if (destObj != null)
                                destObj.DestroyObject();
                            else
                                UnityEngine.Object.Destroy(obj.gameObject);
                        }
                        else
                            obj.gameObject.SetActive(false);
                    }
                }
                // Если объект не найден в сохранении, он остаётся в том состоянии, в котором создан в сцене
            }
        }

        [Header("Destroyed Objects")]
        [SerializeField] private List<string> _destroyedIds = new List<string>();

        public void AddDestroyedObject(string id)
        {
            if (!_destroyedIds.Contains(id))
                _destroyedIds.Add(id);
        }

        public bool IsObjectDestroyed(string id)
        {
            return _destroyedIds.Contains(id);
        }

        public void ApplyDestroyedObjects()
        {
            var objects = UnityEngine.Object.FindObjectsOfType<PersistentObjectState>(true);
            foreach (var obj in objects)
            {
                if (IsObjectDestroyed(obj.UniqueId))
                {
                    var destObj = obj.GetComponent<DestroyAndRegistry>();
                    if (destObj != null)
                        destObj.DestroyObject();
                    else
                        UnityEngine.Object.Destroy(obj.gameObject);
                }
            }
        }

        public void ClearAll()
        {
            _checkPointIds.Clear();
            _chests.Clear();
            _objectStates.Clear();
            _destroyedIds.Clear();
        }
    }
}

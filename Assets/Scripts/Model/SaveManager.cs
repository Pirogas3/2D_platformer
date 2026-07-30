using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public static class SaveManager
    {
        private const string AutoSavePrefix = "AutoSave_";
        private const int AutoSaveCount = 3;
        private const string NextAutoSaveIndexKey = "NextAutoSaveIndex";

        // Корневая папка сохранений: Documents/Golden Wake/
        public static string RootFolder
        {
            get
            {
                string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string gameFolder = Path.Combine(documents, "Golden Wake");
                if (!Directory.Exists(gameFolder))
                    Directory.CreateDirectory(gameFolder);
                return gameFolder;
            }
        }

        // Папка для сохранений: Documents/MyGame/Saves/
        public static string SavesFolder
        {
            get
            {
                string saves = Path.Combine(RootFolder, "Saves");
                if (!Directory.Exists(saves))
                    Directory.CreateDirectory(saves);
                return saves;
            }
        }

        // Полный путь к файлу слота
        public static string GetSlotPath(string slotName)
        {
            return Path.Combine(SavesFolder, $"{slotName}.json");
        }

        // Сохранить объект в JSON-файл
        public static void SaveToFile(object data, string slotName)
        {
            string json = JsonUtility.ToJson(data, true); // true для красивого форматирования
            string path = GetSlotPath(slotName);
            File.WriteAllText(path, json);
        }

        // Загрузить объект из JSON-файла
        public static T LoadFromFile<T>(string slotName) where T : class
        {
            string path = GetSlotPath(slotName);
            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        // Получить самый свежий файл (последнее сделанное сохранение)
        public static string GetLatestSlot()
        {
            string[] files = Directory.GetFiles(SavesFolder, "*.json");
            if (files.Length == 0) return null;

            string latestFile = null;
            DateTime latestTime = DateTime.MinValue;

            foreach (string file in files)
            {
                DateTime lastWrite = File.GetLastWriteTime(file);
                if (lastWrite > latestTime)
                {
                    latestTime = lastWrite;
                    latestFile = file;
                }
            }

            return Path.GetFileNameWithoutExtension(latestFile);
        }

        // Проверить, существует ли слот
        public static bool SlotExists(string slotName)
        {
            return File.Exists(GetSlotPath(slotName));
        }

        // Удалить слот
        public static void DeleteSlot(string slotName)
        {
            string path = GetSlotPath(slotName);
            if (File.Exists(path))
                File.Delete(path);
        }

        // Получить список всех слотов
        public static string[] GetAllSlots()
        {
            string[] files = Directory.GetFiles(SavesFolder, "*.json");
            for (int i = 0; i < files.Length; i++)
                files[i] = Path.GetFileNameWithoutExtension(files[i]);
            return files;
        }

        /// <summary>
        /// Возвращает имя следующего автослота для перезаписи (циклически).
        /// </summary>
        public static string GetNextAutoSaveSlot()
        {
            int index = PlayerPrefs.GetInt(NextAutoSaveIndexKey, 0);
            string slotName = $"{AutoSavePrefix}{index + 1}";
            // Увеличиваем индекс по кругу
            int nextIndex = (index + 1) % AutoSaveCount;
            PlayerPrefs.SetInt(NextAutoSaveIndexKey, nextIndex);
            PlayerPrefs.Save();
            return slotName;
        }

        /// <summary>
        /// Возвращает список всех существующих автослотов с их временем последнего изменения.
        /// </summary>
        public static List<(string slotName, DateTime lastWriteTime)> GetAutoSaveSlotsWithTime()
        {
            var result = new List<(string, DateTime)>();
            for (int i = 1; i <= AutoSaveCount; i++)
            {
                string slotName = $"{AutoSavePrefix}{i}";
                string path = GetSlotPath(slotName);
                if (File.Exists(path))
                {
                    DateTime time = File.GetLastWriteTime(path);
                    result.Add((slotName, time));
                }
            }
            return result;
        }
    }
}

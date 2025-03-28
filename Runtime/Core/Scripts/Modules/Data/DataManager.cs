using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using OneDay.Core.Debugging;
using OneDay.Core.Extensions;
using OneDay.Core.Modules.Assets;
using UnityEngine;


namespace OneDay.Core.Modules.Data
{
    public interface IDataObject
    {
        int Id { get; set; }
    }

    [Serializable]
    public abstract class BaseDataObject : IDataObject
    {
        public int Id { get; set; }
    }
  
    public interface IDataManager : IStorage
    {
        void RegisterStorage<T>(IStorage storage);
        void UnregisterStorage<T>(IStorage storage);
     }
    
    [LogSection("Data")]
    public class DataManager :  MonoBehaviour, IService, IDataManager
    {
        private Dictionary<Type, IStorage> storages;

        public UniTask Initialize() => UniTask.CompletedTask;
        public UniTask PostInitialize() => UniTask.CompletedTask;

        public void RegisterStorage<T>(IStorage storage)
        {
            storages ??= new Dictionary<Type, IStorage>(); 
            storages.Add(typeof(T), storage);
        }

        public void UnregisterStorage<T>(IStorage storage)
        {
            storages.Remove(typeof(T));
        }

        public void RegisterTypeToKeyBinding<T>(string key)
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                storage.RegisterTypeToKeyBinding<T>(key);
            }
            else
            {
                Debug.LogError($"No Storage fo type {typeof(T)} found");
            }
        }

        public async UniTask<int> Add<T>(T data) where T:IDataObject
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                return await storage.Add<T>(data);
            }
            LogNotExistingStorage<T>();
            return -1;
        }

        public async UniTask<bool> Actualize<T>(T data) where T : IDataObject
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                return await storage.Actualize<T>(data);
            }
            LogNotExistingStorage<T>();
            return false;
        }

        public async UniTask<T> Get<T>(int id) where T:IDataObject
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                return await storage.Get<T>(id);
            }

            LogNotExistingStorage<T>();
            return default;
        }

        public async UniTask<IEnumerable<T>> GetAll<T>() where T : IDataObject
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                return await storage.GetAll<T>();
            }

            LogNotExistingStorage<T>();
            return Enumerable.Empty<T>();
        }
        
        public async UniTask Remove<T>(int id) where T:IDataObject
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                await storage.Remove<T>(id);
                return;
            }

            LogNotExistingStorage<T>();
        }

        public async UniTask RemoveAll<T>()
        {
            if (storages.TryGetValue(typeof(T), out var storage))
            {
                await storage.RemoveAll<T>();
                return;
            }

            LogNotExistingStorage<T>();
        }

        private static void LogNotExistingStorage<T>() => 
            Debug.LogError($"No storage for type {typeof(T)} exists");
    }

 
    public interface IStorage
    {
        void RegisterTypeToKeyBinding<T>(string key);
        UniTask<int> Add<T>(T data) where T : IDataObject;
        UniTask<bool> Actualize<T>(T data) where T : IDataObject;
        UniTask<T> Get<T>(int id) where T : IDataObject;
        UniTask<IEnumerable<T>> GetAll<T>() where T : IDataObject;
        UniTask Remove<T>(int id) where T : IDataObject;
        UniTask RemoveAll<T>();
    }

    public class LocalStorage : IStorage
    {
        private Dictionary<Type, string> typeToKeyBindings = new();
        public void RegisterTypeToKeyBinding<T>(string key)
        {
            typeToKeyBindings.Add(typeof(T), key);
        }

        public UniTask<int> Add<T>(T data) where T : IDataObject
        {
            var storageName = GetStorageNameForType<T>();
            var storageContentStr = PlayerPrefs.GetString(storageName, null);
            var storageContent = string.IsNullOrEmpty(storageContentStr) 
                ? new List<T>() 
                : JsonConvert.DeserializeObject<List<T>>(storageContentStr);

            int newId = storageContent.Count == 0 ? 0 : storageContent.Last().Id + 1;
            data.Id = newId;
            storageContent.Add(data);
            
            PlayerPrefs.SetString(storageName, JsonConvert.SerializeObject(storageContent));
            return new UniTask<int>(newId);
        }

        public async UniTask<bool> Actualize<T>(T data) where T : IDataObject
        {
            var storageName = GetStorageNameForType<T>();
            var storageContentStr = PlayerPrefs.GetString(storageName, null);
            var storageContent = string.IsNullOrEmpty(storageContentStr)
                ? new List<T>()
                : JsonConvert.DeserializeObject<List<T>>(storageContentStr);
            int index = storageContent.FindIndex(x => x.Id == data.Id);
            if (index != -1)
            {
                storageContent[index] = data;
                PlayerPrefs.SetString(storageName, JsonConvert.SerializeObject(storageContent));
                return true;
            }

            Debug.LogError($"No data with {data.Id} exists to be updated");
            return false;
        }

        public UniTask<T> Get<T>(int id) where T : IDataObject
        {
            var storageContent = LoadStorage<T>();
            return new UniTask<T>(storageContent.FirstOrDefault(x => x.Id == id));
        }

        public UniTask<IEnumerable<T>> GetAll<T>() where T : IDataObject
        {
            var storageContent = LoadStorage<T>();
            return new UniTask<IEnumerable<T>>(storageContent);
        }

        public UniTask Remove<T>(int id) where T : IDataObject
        {
            var storageContent = LoadStorage<T>();
            int index = storageContent.FindIndex(x => x.Id == id);
            if (index == -1)
            {
                D.LogError($"No such data object with id {id} exists", "DataManager");
                return new UniTask<T>(default);
            }
            
            storageContent.RemoveAt(index);
            SaveStorage<T>(storageContent);
            return UniTask.CompletedTask;
        }

        #if UNITY_EDITOR
        public static void RemoveAllEditor(params string[] keys)
        {
            keys.ForEach(PlayerPrefs.DeleteKey);
        }

        public static void Dump<T>(string key)
        {
            var content = PlayerPrefs.GetString(key);
            Debug.Log($"=== Dump of {typeof(T)} ====\n{content}");
        }
        
        #endif
        
        public UniTask RemoveAll<T>()
        {
            PlayerPrefs.DeleteKey(GetStorageNameForType<T>());
            return UniTask.CompletedTask;
        }

        private string GetStorageNameForType<T>() => typeToKeyBindings[typeof(T)];

        private List<T> LoadStorage<T>()
        {
            var storageName = GetStorageNameForType<T>();
            var storageContentStr = PlayerPrefs.GetString(storageName, null);
            var storageContent = string.IsNullOrEmpty(storageContentStr) 
                ? new List<T>() 
                : JsonConvert.DeserializeObject<List<T>>(storageContentStr);
            return storageContent;
        }

        private void SaveStorage<T>(List<T> storageContent) => 
            PlayerPrefs.SetString(GetStorageNameForType<T>(), JsonConvert.SerializeObject(storageContent));
    }
}
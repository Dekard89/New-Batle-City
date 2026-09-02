using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Scripts.Public
{
    public class PrefabLoader : IPrefabLoad
    {
  
        private Dictionary<AssetReferenceGameObject, GameObject> _cashedPrefabs = new();

        private Dictionary<AssetReferenceGameObject, int> _refCounts = new();

    
        public async UniTask<GameObject> GetPrefab(AssetReferenceGameObject assetReference)
        {
            if(_cashedPrefabs.TryGetValue(assetReference, out GameObject prefab))
            {
                return prefab;
            }
            var go = await LoadPrefabAsync(assetReference);

            return go;
        }

        private async UniTask<GameObject> LoadPrefabAsync(AssetReferenceGameObject assetReference)
        {
            if (assetReference.OperationHandle.IsValid())
            {
                Debug.Log("AssetReference уже загружается, ждем...");
                await assetReference.OperationHandle.Task;
                return assetReference.OperationHandle.Result as GameObject;
            }
            Debug.Log("Загружем префаб .....");
            var handle = assetReference.LoadAssetAsync<GameObject>();

            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log($" prefab is ready {handle.Result}");

                _cashedPrefabs.Add(assetReference, handle.Result);

                _refCounts[assetReference] = 1;

                return handle.Result;
            }
            return null;
            
        }
        public void ReleasePrefab(AssetReferenceGameObject assetReference)
        {
            if (!_refCounts.ContainsKey(assetReference)) return;

            _refCounts[assetReference]--;

            if (_refCounts[assetReference] <= 0)
            {
                Addressables.Release(_cashedPrefabs[assetReference]);
                _cashedPrefabs.Remove(assetReference);
                _refCounts.Remove(assetReference);
            }
                   
        }
        
    }
}

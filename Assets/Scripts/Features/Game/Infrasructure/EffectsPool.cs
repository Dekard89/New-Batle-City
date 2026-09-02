using Assets.Scripts.Views;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Zenject;

namespace Assets.Scripts.Public.Infrasructure
{
    public class EffectsPool : MonoBehaviour, ILocalPool
    {
        [Inject]
        private IPrefabLoad _loader;

        [Inject]
        private DiContainer _container;

        [Inject]
        private List<LocalPoolConfig> _poolConfigs;

        private Dictionary<string, ObjectPool<GameObject>> _pools =new();

        private HashSet<GameObject> _activeObjects=new();

        private Dictionary<string,GameObject> _loadedPrefabs= new();

        private async void Start()
        {
            foreach (var config in _poolConfigs)
            {
                await CreatePoolAsync(config.ReferenceGameObject, config.Capacity);
            }
        }
        private void OnDestroy()
        {
            foreach(var prefab in _activeObjects)
            {
                var asset = _loadedPrefabs.FirstOrDefault(x => x.Value == prefab).Key;
                _loadedPrefabs[asset] = null;
                _pools[asset].Clear();
            }
        }
        public GameObject Get(AssetReferenceGameObject assetReference, Vector2 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(assetReference.AssetGUID)) return null;

            var obj= _pools[assetReference.AssetGUID].Get();

            obj.transform.SetPositionAndRotation(position, rotation);

            if(obj.TryGetComponent<PooledEffect>(out var effect))
            {
                effect.Setup(this, assetReference);
            }

            return obj;
        }

        public void Return(AssetReferenceGameObject assetReference, GameObject gameObject)
        {
            if (!_pools.ContainsKey(assetReference.AssetGUID)) return;

            _pools[assetReference.AssetGUID].Release(gameObject);
        }
        private async UniTask CreatePoolAsync(AssetReferenceGameObject prefabReferance, int Capacity)
        {
            if (_pools.ContainsKey(prefabReferance.AssetGUID)) return;

            var prefab= await _loader.GetPrefab(prefabReferance);

            _loadedPrefabs[prefabReferance.AssetGUID] = prefab;

            var pool = new ObjectPool<GameObject>(
                createFunc: () => CreatePooledObject(prefab),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false),
                actionOnDestroy:(obj)=> Destroy(obj),
                collectionCheck:true,
                defaultCapacity:Capacity,
                maxSize:Capacity*2
                );
            _pools[prefabReferance.AssetGUID]=pool;
            _activeObjects.Add(prefab);

            for(int i = 0; i < Capacity; i++)
            {
                var obj = CreatePooledObject(prefab);
                pool.Release(obj);
            }
        }
        private GameObject CreatePooledObject(GameObject prefab)
        {
            var obj= Instantiate(prefab);

           
            obj.SetActive(false);

            return obj;
        }
    }
}

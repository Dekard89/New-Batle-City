using Assets.Scripts.Handlers;
using Assets.Scripts.Public.Data;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Zenject;

namespace Assets.Scripts.Public.Infrasructure
{
    public class NetworkPool : NetworkBehaviour, INetworkPool
    {
        public static NetworkPool Singleton {  get; private set; }

        [Inject]
        private List<NetworkPoolConfig> _poolConfigs;

        [Inject]
        private DiContainer _container;

        [Inject]
        private IPrefabLoad _prefabLoader;

        private Dictionary<string, ObjectPool<NetworkObject>> _pool = new();

        private HashSet<GameObject> _activeObjects=new();

        private Dictionary<string,GameObject> _loadedPrefabs = new();

        private void Awake()
        {
            if (Singleton != null && Singleton!= this)
            {
                Destroy(gameObject);
                return;
            }
            Singleton = this;
           
        }
        public override async void OnNetworkSpawn()
        {
            foreach(var config in _poolConfigs)
            {
                await RegisterHandlerAsync(config.ReferenceGameObject,config.Capacity);
            }
        }
        public override void OnNetworkDespawn()
        {
            foreach(var prefab in _activeObjects)
            {
                NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefab);
                var asset = _loadedPrefabs.FirstOrDefault(x=>x.Value==prefab).Key;
                _loadedPrefabs[asset] = null;
                _pool[asset].Clear(); 
            }
        }
        public void Return(AssetReferenceGameObject prefabReferance, NetworkObject obj)
        {
            if(!_pool.ContainsKey(prefabReferance.AssetGUID)) return;

            _pool[prefabReferance.AssetGUID].Release(obj);
        }

        public NetworkObject Get(AssetReferenceGameObject prefabReferance, Vector3 position, Quaternion rotation)
        {
            if (!_pool.ContainsKey(prefabReferance.AssetGUID)) return null;

            var obj=_pool[prefabReferance.AssetGUID].Get();

            obj.transform.SetPositionAndRotation(position, rotation);

            return obj;
        }
        private async UniTask RegisterHandlerAsync(AssetReferenceGameObject prefabReferance, int preloadCount)
        {
            if (_pool.ContainsKey(prefabReferance.AssetGUID)) return;

            var prefab= await _prefabLoader.GetPrefab(prefabReferance);

            _loadedPrefabs[prefabReferance.AssetGUID] = prefab;

            var pool = new ObjectPool<NetworkObject>(
                createFunc: () => CreatePooledObject(prefab),
                actionOnGet: (obj) => obj.gameObject.SetActive(true),
                actionOnRelease:(obj) => obj.gameObject.SetActive(false),
                actionOnDestroy:(obj) => Destroy(obj.gameObject),
                collectionCheck:true,
                defaultCapacity:preloadCount,
                maxSize: preloadCount * 2
                );

            _pool[prefabReferance.AssetGUID]= pool;
            _activeObjects.Add(prefab);

            for(int i = 0; i < preloadCount; i++)
            {
                var obj= CreatePooledObject(prefab);
            
                pool.Release(obj);
       
            }

            var handler = new PooledPrefabInstanceHandler(prefabReferance, this);

            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, handler);
        }

     

        private NetworkObject CreatePooledObject(GameObject prefab)
        {
            var go = Instantiate(prefab);
            _container.InjectGameObject(go);
            go.SetActive(false);
            var no = go.GetComponent<NetworkObject>();
            no.AutoObjectParentSync = false;
            return no;
        }
    }
}

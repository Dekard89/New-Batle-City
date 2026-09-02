using Assets.Scripts.Public.Infrasructure;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Assets.Scripts.Handlers
{
    public class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        private AssetReferenceGameObject prefabReferance;

        [Inject(Optional = true)]
        private INetworkPool pool;

        public PooledPrefabInstanceHandler(AssetReferenceGameObject prefabReferance, INetworkPool pool)
        {
                this.pool= pool;
            this.prefabReferance= prefabReferance;
        }

        public void Initialize(AssetReferenceGameObject prefabReferance)
        {
            this.prefabReferance= prefabReferance;
        }

        public void Destroy(NetworkObject networkObject)
        {
            pool.Return(prefabReferance,networkObject);
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            return pool.Get(prefabReferance,position,rotation);
        }
    }
}

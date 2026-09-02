using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Assets.Scripts.Public.Infrasructure
{
    public class CharacterInstanceHandler : INetworkPrefabInstanceHandler
    {
        private readonly DiContainer _container;

        private readonly AssetReferenceGameObject _prefabReferance;

        private readonly IPrefabLoad _loader;

        public CharacterInstanceHandler(DiContainer diContainer,
            AssetReferenceGameObject gameObject,
            IPrefabLoad prefabLoad)
        {
               _container = diContainer;
            _prefabReferance = gameObject;
            _loader = prefabLoad;
        }
        public void Destroy(NetworkObject networkObject)
        {
            _loader.ReleasePrefab(_prefabReferance);

            Object.Destroy(networkObject.gameObject);
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            GameObject prefab = _loader.GetPrefab(_prefabReferance).GetAwaiter().GetResult();

            GameObject instance = UnityEngine.GameObject.Instantiate(prefab, position, rotation, null);

            _container.InjectGameObject(instance);

            return instance.GetComponent<NetworkObject>();
        }
    }
}

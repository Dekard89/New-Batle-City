using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Public.Infrasructure
{
    public interface INetworkPool
    {

        NetworkObject Get(AssetReferenceGameObject prefabReferance, Vector3 position, Quaternion rotation);

        void Return(AssetReferenceGameObject prefabReferance, NetworkObject obj);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Public.Data
{
    [CreateAssetMenu(fileName ="PoolConfig",menuName = "Scriptable Objects/PoolConfig")]
    public class NetworkPoolConfig : ScriptableObject
    {
        public string Id;

        [Header("Pool")]
        public AssetReferenceGameObject ReferenceGameObject;

        public int Capacity;

       
    }
}

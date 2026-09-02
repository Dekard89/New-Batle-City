using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Scripts.Public.Infrasructure
{
    public interface ILocalPool
    {
        GameObject Get(AssetReferenceGameObject assetReference, Vector2 position, Quaternion rotation);

        void Return(AssetReferenceGameObject assetReference, GameObject gameObject);
    }
}

using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface IPrefabLoad
{
    UniTask<GameObject> GetPrefab(AssetReferenceGameObject assetReference);

    void ReleasePrefab(AssetReferenceGameObject assetReference);

}


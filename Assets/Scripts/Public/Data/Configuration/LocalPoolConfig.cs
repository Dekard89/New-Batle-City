using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "LocalPoolConfig", menuName = "Scriptable Objects/LocalPoolConfig")]
public class LocalPoolConfig : ScriptableObject
{
    public string Id;

    public AssetReferenceGameObject ReferenceGameObject;

    public int Capacity;

    public bool IsCycle;
 
}

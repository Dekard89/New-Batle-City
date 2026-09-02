using Assets.Scripts.Public.Data.Stats;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "PlayerTankConfig", menuName = "Scriptable Objects/PlayerTankConfig")]
public class PlayerTankConfig : ScriptableObject
{
    [Header("Name")]
    public string Name;

    public int Id;

    [Header("Visual")]
    public AssetReferenceGameObject Prefab;

    public Sprite Avatar;

    [Header("Stats")]
   
    public float Health;

    public float Armor;

    public float RotationSpeedFactor;

    public int MineCapacity;

    public float SetColdown;

    [Header("ShootStats")]

    public ShootStats shootStats;
}

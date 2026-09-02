using Assets.Scripts.Public.Data.Stats;
using Assets.Scripts.Public.Infrasructure;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class CombatController : InjectedNetworkBehaviour
{
    //Model
    [NonSerialized]
    public NetworkVariable<int> CurrentAmmo = new(0);

    private Transform _fireSpot;

    [SerializeField]
    private ShootStats _stats;

    private float _lastFireTime;

    private TankVisuals _visuals;
    private bool IsReady => Time.time >= _lastFireTime + _stats.FireRate && CurrentAmmo.Value > 0;
    //Model
    [Inject(Id = "projectile")]
    private AssetReferenceGameObject _projReference;

    [Inject(Id = "mine")]
    private AssetReferenceGameObject _mineReferance;

    private bool _isReloading;

    [Inject]
    private INetworkPool _pool;

    protected override void Awake()
    {
        base.Awake();
        _fireSpot=GetComponentInChildren<FireMarker>().GetComponent<Transform>();
        _visuals=GetComponent<TankVisuals>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsServer)
          CurrentAmmo.Value = _stats.Ammo;
    }

    [ServerRpc]
    private void FireServerRpc()
    {
        if(!IsReady) return;

         CurrentAmmo.Value--;

        _lastFireTime = Time.time;
         var proj = _pool.Get(_projReference, _fireSpot.position, _fireSpot.rotation);
        _visuals.PlayShootEffect();
        proj.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        
        if(CurrentAmmo.Value<=0 && !_isReloading)
            StartReloadAsync().Forget();
        
    }
    private async UniTask StartReloadAsync()
    {
        _isReloading = true;
        await UniTask.Delay((int)_stats.ReloadTime * 1000);
        CurrentAmmo.Value = _stats.Ammo;
        Debug.Log("Перезаряжаем");
        _isReloading=false;
    }
    public void FireRequest()
    {
      
        FireServerRpc();
   
    }
    public void MineSetRequest()
    {
        MineSetServerRpc();
    }
    [ServerRpc]
    private void MineSetServerRpc()
    {
        if (CheckAnotherRadius())
        {
            Debug.Log("Невозможно установить мину");
            return;
        }
        var mine= _pool.Get(_mineReferance,transform.position,transform.rotation);

        mine.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }
    private bool CheckAnotherRadius()
    {
        var mines = Physics2D.OverlapCircleAll(transform.position, 5f);

        var check = mines.Any(x=>x.TryGetComponent<MineController>(out var mine));

        return check;
    }

}

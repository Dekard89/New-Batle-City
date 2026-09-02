using Assets.Scripts.Abstraction;
using Assets.Scripts.Views;
using System;
using Unity.Netcode;
using UnityEngine;

public class WallHealtController : NetworkBehaviour, IDamagable
{
    [NonSerialized]
    public NetworkVariable<float> CurrentHealth = new();

    [SerializeField]
    private float _maxHelth = 100f;

    private WallVisuals _visuals;

    private void Awake()
    {
        _visuals = GetComponent<WallVisuals>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            CurrentHealth.Value = _maxHelth;
        }
        CurrentHealth.OnValueChanged += _visuals.SyncTakeDamageHandler;

        _visuals.SyncTakeDamageHandler(CurrentHealth.Value, CurrentHealth.Value);
    }

   

    public void TakeDamage(float damage)
    {
        if (!IsServer) return;

        CurrentHealth.Value= Mathf.Max(0,CurrentHealth.Value - damage);
        if(CurrentHealth.Value<=0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (!IsServer) return;
        _visuals.SyncDeath();
        GetComponent<NetworkObject>().Despawn();
    }
    public float GetMaxHealth() => _maxHelth;

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        CurrentHealth.OnValueChanged += _visuals.SyncTakeDamageHandler;
    }

}

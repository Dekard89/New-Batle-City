using Assets.Scripts.Abstraction;
using Assets.Scripts.Public.Data.Enums;
using Assets.Scripts.Public.Data.Model;
using Assets.Scripts.Public.Data.Signals;
using Assets.Scripts.Public.Data.Stats;
using Assets.Scripts.Views;
using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class HealthController : InjectedNetworkBehaviour, IDamagable
{
    //Model
    [NonSerialized]
    public NetworkVariable<float> CurrentHealth = new();

    [NonSerialized]
    public NetworkVariable<int> TeamId = new();

    private NetworkVariable<FixedString32Bytes> _networkPlayerName = new(new FixedString32Bytes(""));
 
    [SerializeField]
    private HealthStats stats;

    private HealthBarWorldView _healthBar;

    [Inject]
    private SignalBus _signalBus;

    [Inject]
    private LocalSessionModel _localSessionModel;

    private TankVisuals _visuals;

    private static HealthController Instance;

    private float _lastPunch;

    private float _lastHealth;

    public string PlayerName => _networkPlayerName.Value.ToString();

    public event Action<string> OnNameChanged;

    protected override void Awake()
    {
        base.Awake();
        _healthBar = GetComponentInChildren<HealthBarWorldView>();
        _visuals = GetComponent<TankVisuals>();
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer) CurrentHealth.Value = stats.MaxHealth;

        if (IsLocalPlayer) Instance = this;

        CurrentHealth.OnValueChanged += HealthChenged;

        ApplyVisuals();

        HealthChenged(0, CurrentHealth.Value);

        _networkPlayerName.OnValueChanged += HandleNetworkNameChanged;

        OnNameChanged?.Invoke(PlayerName);
    }

    private void HandleNetworkNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        OnNameChanged?.Invoke(newValue.ToString());
    }

    private void HealthChenged(float previousValue, float newValue)
    {
       _healthBar.UpdateValue(newValue, stats.MaxHealth);

        if (IsOwner)
        {
            _signalBus.Fire(new LocalHealthChangedSignal
            {
                Max=stats.MaxHealth,
                Current= newValue
                
            });
        }
        if(previousValue> newValue)
            _visuals.FlashRoutine().Forget();
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth.Value -= damage;
        if (CurrentHealth.Value <= 0) Die();
        _lastPunch=Time.time;
    }
    

    public override void OnNetworkDespawn()
    {
        CurrentHealth.OnValueChanged-= HealthChenged;

        _networkPlayerName.OnValueChanged -= HandleNetworkNameChanged;
    }
    private void Die()
    {
        _signalBus.Fire(new EffectSignal
        {
            ExplosionType= EffectType.BigExplosion,

            Position=transform.position
        });
        
        DeathSequence().Forget();
       
    }
    private void SelfHealing()
    {
        if(CurrentHealth.Value == stats.MaxHealth) return;

        if(Time.time < _lastPunch + stats.SelfHealDelay) return;

        if(Time.time > _lastHealth + stats.HealingTick)
        {
            CurrentHealth.Value += stats.SelfHealPercent;
            _lastHealth = Time.time;
        }
        Debug.Log("Исцеление полученно");
    }
    private void Update()
    {
        if(IsServer)
             SelfHealing();
    }
    private void ApplyVisuals()
    {
        var check = TeamId.Value == _localSessionModel.LocalTeamId;

        _healthBar.Setup(IsOwner, check);

        _healthBar.UpdateValue(CurrentHealth.Value, stats.MaxHealth);
    }
    public void SetPlayerNameServer(string playerName)
    {
        if(IsServer) _networkPlayerName.Value = playerName;
    }
   
    private async UniTaskVoid DeathSequence()
    {
        if (!IsServer) return;
 
        GetComponent<MovementController>().StopMovementServerRpc();

        GetComponent<InputController>().enabled = false;

        await _visuals.SyncDeath();

        _signalBus.Fire(new EffectSignal
        {
            Position = transform.position,

            ExplosionType = EffectType.Fire
        });
        _signalBus.Fire(new EffectSignal
        {
            Position = transform.position,

            ExplosionType = EffectType.Smoke
        });
        
        await UniTask.WaitForSeconds(5);

        _signalBus.Fire(new EffectSignal
        {
            Position = transform.position,

            ExplosionType = EffectType.Crator
        });

        GetComponent<NetworkObject>().Despawn();
    }
   
}

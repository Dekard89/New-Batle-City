using Assets.Scripts.Abstraction;
using Assets.Scripts.Public.Data.Stats;
using Assets.Scripts.Views;
using Cysharp.Threading.Tasks;
using System;
using Unity.Netcode;
using UnityEngine;

public class MineController : InjectedNetworkBehaviour
{
    //Model
    [SerializeField]
    private MineStats stats;

    [NonSerialized]
    public readonly NetworkVariable<bool> IsActive =
        new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [NonSerialized]
    private NetworkVariable<float> SyncDamage = new();

    [NonSerialized]
    private NetworkVariable<bool> IsReady = new(false);

    [NonSerialized]
    private NetworkVariable<float> ActivateTime= new();

    private bool _isPlayerInside;

    private bool _timerFinished;

    private MineVisuals _visuals;

    //Model
    protected override void Awake()
    {
        _visuals = GetComponent<MineVisuals>();
        base.Awake();
    }

    public override void OnNetworkSpawn()
    {

        if (IsServer)
        {
            IsActive.Value = false;
            SyncDamage.Value = stats.Damage;
            ActivateTime.Value = stats.ActivationTime;
            _isPlayerInside = true;
            StartActivationTimer().Forget();
        }
    }
    public override void OnNetworkPreDespawn()
    {

        base.OnNetworkPreDespawn();
    }

    public float GetActivationTime() => stats.ActivationTime;

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsServer) return;

        
            if (other.CompareTag("Player"))
            {
                _isPlayerInside = false;

                TryActivate();

      
            }
        
 
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsActive.Value == false) return;

        if(other.CompareTag("Player") && IsActive.Value)
            Explode();
    }
    private async UniTask StartActivationTimer()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(stats.ActivisionDelay));

        _timerFinished= true;

        TryActivate();
    }
    private void TryActivate()
    {
        if(_timerFinished && !_isPlayerInside && !IsActive.Value)
            IsActive.Value= true;
        Debug.Log($"Мина - - {IsActive.Value} ");
    }
    private void Explode()
    {
        if(!IsServer) return;

        if (!IsSpawned) return;

        Vector2 explpodPos = transform.position;

        var targets = Physics2D.OverlapCircleAll(explpodPos, stats.ExplodeRadius);

        foreach(var target in targets)
        {
            if(target.TryGetComponent<Rigidbody2D>(out var rb))
            {
                Vector2 direction = (rb.position - explpodPos).normalized;

                float distance = Vector2.Distance(explpodPos, rb.position);

                float force = stats.ExplodeRadius * (1- distance/stats.ExplodeRadius);

                rb.AddForce(direction * force * stats.ExplosionForce, ForceMode2D.Impulse);
            }
            if(target.TryGetComponent<IDamagable>(out var health))
            {
                health.TakeDamage(SyncDamage.Value);
            }
        }
        SyncExplosionClientRpc();
        GetComponent<NetworkObject>().Despawn();
    }
    [ClientRpc]
    private void SyncExplosionClientRpc()
    {
        _visuals.PlayExplosionVFX();
    }
}

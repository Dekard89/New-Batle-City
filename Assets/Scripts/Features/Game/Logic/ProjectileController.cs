using Assets.Scripts.Abstraction;
using Assets.Scripts.Public.Data.Stats;
using Assets.Scripts.Views;
using Unity.Netcode;
using UnityEngine;

public class ProjectileController :InjectedNetworkBehaviour
    
{
    [SerializeField]
    private ProjectileStats _stats;

    private Rigidbody2D _rb;

    public readonly NetworkVariable<float> Damage = 
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public readonly NetworkVariable<float> Speed =
        new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private ProjectileVisuals _visuals;

    private float _damageFactor;

    private float _speedFactor;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _visuals = GetComponent<ProjectileVisuals>();
    }

    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsServer)
            CalculateParameters();
    
        Launch();
    }
    
    public override void OnNetworkDespawn()
    {
       
    }
    
    private void ResetProjectile()
    {
        _rb.linearVelocity=Vector2.zero;
        _rb.angularVelocity=0;
    }
    public override void OnNetworkPreDespawn()
    {
        ResetProjectile();
       
    }
    public void SetParameters(float speedFactor, float damageFactor)
    {
        _speedFactor = speedFactor;
        _damageFactor = damageFactor;
    }
    private void CalculateParameters()
    {
        Speed.Value = _speedFactor>0 ? _stats.Speed * _speedFactor : _stats.Speed;

        Damage.Value = _damageFactor>0 ? _stats.Damage * _damageFactor : _stats.Damage;
    }

    public void ResetParameters()
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0;
    }

    private void Launch()
    {
   
        _rb.linearVelocity = transform.up * Speed.Value * Time.fixedDeltaTime;
        
 
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsSpawned) return;

       if(!IsServer) return;

       if(collision.TryGetComponent<IDamagable>(out var health))
            health.TakeDamage(Damage.Value);

       SyncVisualsClientRpc();

       NetworkObject.Despawn();
     
    }
    [ClientRpc]
    private void SyncVisualsClientRpc()
    {
        _visuals.PlayExplosionFX();
    }

}

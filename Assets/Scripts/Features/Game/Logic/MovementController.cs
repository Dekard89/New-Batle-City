using Assets.Scripts.Public.Data.Stats;
using System;
using Unity.Netcode;
using UnityEngine;

public class MovementController : InjectedNetworkBehaviour
{
    //Model
    [NonSerialized]
    public NetworkVariable<float> SyncSpeed =
        new ();

    [NonSerialized]
    public NetworkVariable<float> TowerAngle =
        new (0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Vector2 _currentVector;

    private float _towerAxis;

    [SerializeField]
    private MovementStats movementStats;

    //Model
    //Logic
    private Rigidbody2D _rigidbody;
    private Transform _towerTransform;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponent<Rigidbody2D>();
        
    }
    [ServerRpc]
    public void SetMovementServerRpc(Vector2 input)
    {
        _currentVector= input;
    }
    [ServerRpc]
    public void SetTowerAxisServerRpc(float axis)
    {
        _towerAxis=axis;
    }
    private void FixedUpdate()
    {
        if (!IsServer) return;

        ApplyMovement(_currentVector);

        ApplyTowerAxis(_towerAxis);
    }
    //public void ProcessInput(Vector2 input, float axis)
    //{
    //    MoveServerRpc(input);

    //    if(axis!=0)
    //        RotateTowerServerRpc(axis);
    //}
    public void RequestResetTower()=> ResetTowerRotateServerRpc();

    
    private void ApplyMovement(Vector2 input)
    {

        if (Mathf.Abs(input.x) > 0.1f)
        {
            float torque = -input.x * movementStats.RotationSpeed;
            _rigidbody.AddTorque(torque * Time.fixedDeltaTime);
        }
        if (Mathf.Abs(input.y) > 0.1f)
        {
            SyncSpeed.Value = input.y;

            Vector2 force = _rigidbody.transform.up * (SyncSpeed.Value * movementStats.MovementSpeed * movementStats.SpeedFactor);

            _rigidbody.AddForce(force * Time.fixedDeltaTime );
        }
        if (input == Vector2.zero)
        {
            SyncSpeed.Value = 0;

            _rigidbody.linearVelocity = Vector2.Lerp(_rigidbody.linearVelocity, Vector2.zero, Time.fixedDeltaTime * 2f);

            _rigidbody.angularVelocity = Mathf.Lerp(_rigidbody.angularVelocity, 0f, 3f * Time.fixedDeltaTime);
        }
    }

    [ServerRpc]
    private void ResetTowerRotateServerRpc() => TowerAngle.Value = 0f;
    

    
    private void ApplyTowerAxis(float axis)
    {
        TowerAngle.Value += axis * movementStats.TowerRotationSpeed * Time.fixedDeltaTime;
    }

    [ServerRpc]
    public void StopMovementServerRpc()
    {
        _rigidbody.linearVelocity=Vector2.zero;

        _rigidbody.bodyType=RigidbodyType2D.Kinematic;

        this.enabled= false;
    }
}

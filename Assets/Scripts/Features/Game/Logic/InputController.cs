using Assets.Scripts.Abstraction;
using Assets.Scripts.Public.Data;
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class InputController : InjectedNetworkBehaviour
{

    private CombatController _combatController;
    private MovementController _movementController;
    [InjectOptional]
    private  IInputService _inputService;
  
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
     
        if (IsOwner)
        {
            _inputService.EnablePlayerInput();
            _inputService.ResetTowerAction.performed += OnResetTower;
            _inputService.FireAction.performed += OnFire;
            _inputService.MineSetAction.performed += OnMineSet;
        }
        
    }

    private void OnMineSet(InputAction.CallbackContext context)
    {
        if(context.performed)
        _combatController.MineSetRequest();
    }

    protected override void Awake()
    {
        base.Awake();
        _movementController=GetComponent<MovementController>();
        _combatController=GetComponent<CombatController>();
    }
   

    private void Update()
    {
        if (!IsOwner || !IsSpawned) return;
        
        _inputService.UpdateInput();
        _movementController.SetMovementServerRpc(_inputService.MoveVector);
        _movementController.SetTowerAxisServerRpc(_inputService.TowerAxis);
        
    }
    
    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
            return;
        _inputService.DisablePlayerInput();
        _inputService.FireAction.performed -= OnFire;
        _inputService.MineSetAction.performed-= OnMineSet;
        _inputService.ResetTowerAction.performed -= OnResetTower;
        base.OnNetworkDespawn();
    }
   
    private void OnFire(InputAction.CallbackContext context)
    {
        if(!IsOwner)return;

        if (context.performed)
            _combatController.FireRequest();
    }
 
    private void OnResetTower(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        if (context.performed)
            _movementController.RequestResetTower();
    }
    
}

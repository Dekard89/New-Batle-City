using UnityEngine;
using UnityEngine.InputSystem;

public class InputService : IInputService
{
    private readonly InputActionAsset _actionAsset;

    private InputAction _towerRotateAction;

    private InputAction _moveAction;
    public InputService(InputActionAsset inputActions) 
    {
        _actionAsset = inputActions;

        Initialize();

        Debug.Log("Input Service is work");
    }
    public InputAction FireAction {  get; private set; }

    public InputAction MineSetAction {  get; private set; }

    public InputAction ResetTowerAction {  get; private set; }

    public Vector2 MoveVector {  get; private set; }

    public float TowerAxis {  get; private set; }

    public void DisablePlayerInput()
    {
        _actionAsset.FindActionMap("Player").Disable();
    }

    public void EnablePlayerInput()
    {
        _actionAsset.FindActionMap("Player").Enable();
    }
    public void Initialize()
    {

        _moveAction = _actionAsset.FindAction("Move");

        FireAction = _actionAsset.FindAction("Fire");

        MineSetAction = _actionAsset.FindAction("MineSet");

        _towerRotateAction = _actionAsset.FindAction("TowerRotate");

        ResetTowerAction = _actionAsset.FindAction("ResetTowerRotate");

        Debug.Log("Input Actions ready");
    }

    public void UpdateInput()
    {
        MoveVector=_moveAction.ReadValue<Vector2>();
        TowerAxis = _towerRotateAction.ReadValue<float>();
    }
}

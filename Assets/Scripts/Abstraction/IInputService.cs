using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputService
{
    Vector2 MoveVector {  get; }

    float TowerAxis { get; }
   
    InputAction FireAction { get; }

    InputAction MineSetAction { get; }

    InputAction ResetTowerAction { get; }

    void UpdateInput();
    void EnablePlayerInput();

    void DisablePlayerInput();
}

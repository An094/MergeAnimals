using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    public static Vector2 MoveInput { get; set; }

    public static bool IsThrowPressed { get; set; }

    private InputAction _moveAction;
    private InputAction _throwAction;

    //for mobile
    private InputAction TouchPostionAtion;
    private InputAction TouchAction;

    public static Vector2 TouchPosition;

    public static bool WasTouchedThisFrame;
    public static bool WasReleasedThisFrame;
    public static bool IsTouched;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        _moveAction = PlayerInput.actions["Move"];
        _throwAction = PlayerInput.actions["Throw"];

        //for mobile
        TouchPostionAtion = PlayerInput.actions["TouchPosition"];
        TouchAction = PlayerInput.actions["Touch"];
    }

    private void Update()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();

        IsThrowPressed = _throwAction.WasPressedThisFrame();

        //for mobile
        TouchPosition = TouchPostionAtion.ReadValue<Vector2>();

        WasTouchedThisFrame = TouchAction.WasPerformedThisFrame();
        WasReleasedThisFrame = TouchAction.WasReleasedThisFrame();
        IsTouched = TouchAction.IsPressed();
    }
}

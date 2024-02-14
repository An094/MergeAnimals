using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchInputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;

    private InputAction TouchPostionAtion;
    private InputAction TouchAction;

    public static Vector2 TouchPosition;

    public static bool WasTouchedThisFrame;
    public static bool WasReleasedThisFrame;
    public static bool IsTouched;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        TouchPostionAtion = PlayerInput.actions["TouchPosition"];
        TouchAction = PlayerInput.actions["Touch"];
    }

    private void Update()
    {
        TouchPosition = TouchPostionAtion.ReadValue<Vector2>();

        WasTouchedThisFrame = TouchAction.WasPerformedThisFrame();
        WasReleasedThisFrame = TouchAction.WasReleasedThisFrame();
        IsTouched = TouchAction.IsPressed();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputAction;

public class PlayerInputEventSystem : MonoBehaviour, IPlayerInputActions
{
    public UnityEvent<InputAction.CallbackContext> PlayerInputEvent = new();

    public void OnAttack(InputAction.CallbackContext context)
    {
        PlayerInputEvent?.Invoke(context);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        PlayerInputEvent?.Invoke(context);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        PlayerInputEvent?.Invoke(context);
    }

    public void OnSpellD(InputAction.CallbackContext context)
    {
        PlayerInputEvent?.Invoke(context);
    }

    public void OnSpellF(InputAction.CallbackContext context)
    {
        PlayerInputEvent?.Invoke(context);
    }
}

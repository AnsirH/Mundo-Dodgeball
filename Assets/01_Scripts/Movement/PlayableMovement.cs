using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayableMovement : Movement, IPlayable
{
    public void GetPlayerInputEvent(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}

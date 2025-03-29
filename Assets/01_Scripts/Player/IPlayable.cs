using UnityEngine.InputSystem;

public interface IPlayable
{
    public void GetPlayerInputEvent(InputAction.CallbackContext context);
}

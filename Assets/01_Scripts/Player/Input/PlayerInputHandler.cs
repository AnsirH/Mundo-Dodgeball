using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public bool LeftClick { get; private set; }
    public bool RightClick { get; private set; }
    public bool ButtonQ { get; private set; }
    public bool ButtonD { get; private set; }
    public bool ButtonF { get; private set; }

    private PlayerInputAction inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputAction();
        inputActions.Gameplay.LeftClick.performed += ctx => LeftClick = true;
        inputActions.Gameplay.RightClick.performed += ctx => RightClick = true;
        inputActions.Gameplay.Q.performed += ctx => ButtonQ = true;
        inputActions.Gameplay.D.performed += ctx => ButtonD = true;
        inputActions.Gameplay.F.performed += ctx => ButtonF = true;

        inputActions.Enable();
    }

    private void OnDestroy()
    {
        inputActions.Disable();
    }

    public void ResetInputValue()
    {
        LeftClick = false;
        RightClick = false;
        ButtonQ = false;
        ButtonD = false;
        ButtonF = false;
    }
}

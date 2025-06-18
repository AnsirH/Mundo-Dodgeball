using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public bool LeftClick { get; private set; }
    public bool RightClick { get; private set; }
    public bool ButtonQ { get; private set; }
    public bool ButtonD { get; private set; }
    public bool ButtonF { get; private set; }
    public bool ButtonStopMove { get; private set; }

    private InputActionMap mainActionMap;
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        mainActionMap = playerInput.actions.FindActionMap("Gameplay");//ActionMapÃßÃâ
        mainActionMap.FindAction("LeftClick").performed += ctx => LeftClick = true;
        mainActionMap.FindAction("RightClick").performed += ctx => RightClick = true;
        mainActionMap.FindAction("Q").performed += ctx => ButtonQ = true;
        mainActionMap.FindAction("D").performed += ctx => ButtonD = true;
        mainActionMap.FindAction("F").performed += ctx => ButtonF = true;
        mainActionMap.FindAction("StopMove").performed += ctx => ButtonStopMove = true;

    }

    private void OnDestroy()
    {
        playerInput.actions.Disable();
    }

    public void ResetInputValue()
    {
        LeftClick = false;
        RightClick = false;
        ButtonQ = false;
        ButtonD = false;
        ButtonF = false;
        ButtonStopMove = false;
    }
}

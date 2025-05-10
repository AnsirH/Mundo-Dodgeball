using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputEventSystem : MonoBehaviourPunCallbacks
{
    [SerializeField] private PlayerInput playerInput;
    public UnityEvent<InputAction.CallbackContext> PlayerInputEvent = new();

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public override void OnEnable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            var actions = playerInput.actions;
            actions["Attack"].performed += OnInput;
            actions["Click"].performed += OnInput;
            actions["Move"].performed += OnInput;
            actions["D"].performed += OnInput;
            actions["F"].performed += OnInput;
        }
    }

    public override void OnDisable()
    {
        if (playerInput != null && playerInput.actions != null)
        {
            var actions = playerInput.actions;
            actions["Attack"].performed -= OnInput;
            actions["Click"].performed -= OnInput;
            actions["Move"].performed -= OnInput;
            actions["D"].performed -= OnInput;
            actions["F"].performed -= OnInput;
        }
    }

    private void OnInput(InputAction.CallbackContext context)
    {
        PlayerInputEvent?.Invoke(context);
    }
}

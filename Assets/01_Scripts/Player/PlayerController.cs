using Photon.Pun;
using PlayerCharacterControl;
using PlayerCharacterControl.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private PlayerStateMachine playerStateMachine;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerAttack playerAttack;
    [SerializeField] private Animator playerAnim;
    [SerializeField] private PlayerAnimEventHandler playerAnimEventHandler;
    [SerializeField] private PlayerHealth playerHealth;

    public PlayerStateMachine StateMachine => playerStateMachine;
    public PlayerMovement Movement => playerMovement;
    public PlayerAttack Attack => playerAttack;
    public Animator Anim => playerAnim;
    public PlayerHealth Health => playerHealth;

    void Awake()
    {
        playerAnimEventHandler.OnAnimationEventActions.AddListener(GetAnimationEvent);

        playerStateMachine = new(this);
        playerStateMachine.ChangeState(EPlayerState.Idle);
    }

    void Update()
    {
        playerStateMachine.UpdateCurrentState();
    }

    public static Vector3 GetMousePosition(Transform entity)
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Vector3 result = hit.point;
            result.y = entity.position.y;
            return result;
        }
        else
            return Vector3.zero;
    }

    private void QuitAllAction()
    {
        playerMovement.StopMove();
        playerAttack.CancelAttack();
    }

    private void GetAnimationEvent(string tag)
    {
        switch (tag)
        {
            case "Throw Axe":
                playerAttack.SpawnAxe();
                break;

            case "Reset Axe":
                playerAttack.ResetAxe();
                break;
        }
    }

    public void GetPlayerInputEvent(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;
        switch (context.action.name)
        {
            case "Move":
                if (context.started)
                {
                    playerMovement.StartMove();
                    playerAttack.CancelReady();
                }
                else if (context.canceled) playerMovement.CancelHold();
                break;

            case "Attack":
                if (context.started) playerAttack.ReadyToAttack();
                break;

            case "Click":
                if (context.started) playerAttack.SetAttack(true);
                break;
            case "StopMove":
                if (context.started) playerMovement.StopMove();
                break;
        }
    }
}
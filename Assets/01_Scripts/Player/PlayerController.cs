using PlayerCharacterControl.State;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputAction;

namespace PlayerCharacterControl
{
    public class PlayerController : MonoBehaviour, IPlayerInputActions
    {
        private PlayerStateMachine playerStateMachine;
        private PlayerMovement playerMovement;
        [SerializeField] private PlayerAttack playerAttack;
        [SerializeField] private Animator playerAnim;
        [SerializeField] private PlayerAnimEventHandler playerAnimEventHandler;

        public PlayerStateMachine StateMachine => playerStateMachine;
        public PlayerMovement Movement => playerMovement;
        public PlayerAttack Attack => playerAttack;
        public Animator Anim => playerAnim;

        void Awake()
        {
            playerAnimEventHandler.OnAnimationEventActions.AddListener(ActEvent);

            playerMovement = GetComponent<PlayerMovement>();

            playerStateMachine = new(this);
            playerStateMachine.ChangeState(EPlayerState.Idle);
        }

        void Update()
        {
            playerStateMachine.UpdateCurrentState();

            if (Mouse.current.rightButton.isPressed)
            {
                RaycastHit hit;
                if (Physics.Raycast(CameraManager.Instance.firstPlayerCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                {
                    playerMovement.StartMove(hit.point);
                }
            }
        }

        private void ActEvent(string tag)
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

        private void QuitAllAction()
        {
            playerMovement.StopMove();
            playerAttack.CancelAttack();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                QuitAllAction();
                RaycastHit hit;
                if (Physics.Raycast(CameraManager.Instance.firstPlayerCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                {
                    playerMovement.StartMove(hit.point);
                }
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            playerAttack.OnAttack();
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            QuitAllAction();
            playerAttack.OnSelect();
        }

        public void OnSpellD(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void OnSpellF(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
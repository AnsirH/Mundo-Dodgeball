using PlayerCharacterControl.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl
{
    public class PlayerController : MonoBehaviour
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
    }
}
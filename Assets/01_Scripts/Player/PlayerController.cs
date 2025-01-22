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
        private PlayerAttack playerAttack;
        private AxeShooter axeShooter;
        [SerializeField] private Animator playerAnim;
        [SerializeField] private PlayerAnimEventHandler playerAnimEventHandler;

        public PlayerStateMachine StateMachine => playerStateMachine;
        public PlayerMovement Movement => playerMovement;
        public PlayerAttack Attack => playerAttack;
        public Animator Anim => playerAnim;
        public AxeShooter AxeShooter => axeShooter;

        void Awake()
        {
            playerAnimEventHandler.OnAnimationEventActions.AddListener(ActEvent);

            playerMovement = GetComponent<PlayerMovement>();
            axeShooter = GetComponent<AxeShooter>();

            playerStateMachine = new(this);
            playerStateMachine.ChangeState(EPlayerState.Idle);
            playerAttack = new();
        }

        void Update()
        {
            playerStateMachine.UpdateCurrentState();
            playerAttack.Cooldown();
        }

        private void ActEvent(string tag)
        {
            switch (tag)
            {
                case "Throw Axe":
                    axeShooter.SpawnAxe();
                    break;
            }
        }
    }
}
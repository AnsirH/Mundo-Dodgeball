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
        [SerializeField] private Animator anim;

        public PlayerStateMachine StateMachine => playerStateMachine;
        public PlayerMovement Movement => playerMovement;
        public Animator Anim => anim;

        void Awake()
        {
            playerStateMachine = new(this);
            playerStateMachine.ChangeState(EPlayerState.Idle);

            playerMovement = GetComponent<PlayerMovement>();
        }

        void Update()
        {
            playerStateMachine.Updated();
        }
    }
}
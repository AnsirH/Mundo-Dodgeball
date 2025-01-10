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

        public PlayerStateMachine StateMachine => playerStateMachine;
        public PlayerMovement Movement => playerMovement;

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
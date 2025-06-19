using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mundo_dodgeball.Player.StateMachine
{
    public class PlayerDieState : PlayerStateBase
    {
        public PlayerDieState(IPlayerContext playerContext) : base(playerContext)
        {
            base.playerContext = playerContext;
        }

        public override void EnterState(StateTransitionInputData inputData)
        {
            playerContext.Anim.SetTrigger("Die");

            ServerManager.Instance.matchManager.RPC_RequestAddScore(playerContext.Health.Killer); // 플레이어가 죽으면 게임 종료
            playerContext.Movement.StopMove();
        }

        public override void ExitState()
        {
        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
        }

        public override void UpdateState()
        {
        }
        //[Networked]
    }
}
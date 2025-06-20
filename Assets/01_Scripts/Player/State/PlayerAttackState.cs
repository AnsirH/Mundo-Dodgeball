using UnityEngine;

namespace Mundo_dodgeball.Player.StateMachine
{
    public class PlayerAttackState : PlayerStateBase
    {
        public PlayerAttackState(IPlayerContext playerContext) : base(playerContext)
        {
            attack = playerContext.Attack;
        }

        public override void EnterState(StateTransitionInputData inputData)
        {
            playerContext.Movement.StopMove();
            Vector3 targetPoint = inputData.mousePosition;
            targetPoint.y = 0.0f;
            direction = (targetPoint - attack.transform.position).normalized;
            attack.StartAttack(targetPoint);
        }
        public override void ExitState()
        {
        }

        public override void UpdateState()
        {
        }

        public override void NetworkUpdateState(float runnerDeltaTime)
        {
            playerContext.Movement.RotateForDeltaTime(playerContext.Movement.transform.rotation, direction, attack.RotationSpeed);
            if (!attack.Attacking)
            {
                attack.Fire(direction);
                playerContext.ChangeState(EPlayerState.Idle);
            }
        }

        private PlayerAttack attack;
        Vector3 direction = Vector3.zero;
    }
}
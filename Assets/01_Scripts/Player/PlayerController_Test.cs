using Fusion;
using PlayerCharacterControl.State;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Utils;

/// <summary>
/// ���콺 ��/�� Ŭ���� q,d,f,s Ű�� �Է� �ް� �׿� ���� �׼��� �ϴ� fusion ������Ʈ
/// </summary>
public class PlayerController_Test : NetworkBehaviour
{
    //public override void FixedUpdateNetwork()
    //{
    //    if (GetInput(out NetworkInputData data))
    //    {
    //        if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
    //        {
    //            Debug.Log("right click!");
    //            // ���콺 ��ġ ����
    //            ClickPoint = GetMousePosition();
    //            if (!ClickPoint.HasValue || !IngameController.Instance.ground.GetAdjustedPoint(GroundSectionNum, transform.position, ClickPoint.Value, out Vector3 adjustedPoint)) return;

    //            ClickPoint = adjustedPoint;
    //            StartCoroutine(SpawnEffect("ClickPointer", ClickPoint.Value));

    //            if (!attack.IsActionInProgress)
    //                stateMachine.ChangeState(EPlayerState.Move);
    //        }
    //        if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
    //        {
    //            Debug.Log("left click!");
    //            ClickPoint = GetMousePosition();
    //            if (!attack.IsActionInProgress && attack.CanExecuteAction)
    //            {
    //                stateMachine.ChangeState(EPlayerState.Attack);
    //            }
    //        }
    //        if (data.buttons.IsSet(NetworkInputData.BUTTONF))
    //        {
    //            Debug.Log("F Button pressed!");
    //            // ���콺 ��ġ ����
    //            ClickPoint = GetMousePosition();
    //            if (!ClickPoint.HasValue || !IngameController.Instance.ground.GetAdjustedPoint(GroundSectionNum, transform.position, ClickPoint.Value, out Vector3 adjustedPoint)) return;

    //            ClickPoint = adjustedPoint;
    //        }
    //    }
    //}
}

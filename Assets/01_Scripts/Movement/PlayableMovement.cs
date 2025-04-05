using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MyGame.Utils;

public class PlayableMovement : Movement, IPlayerComponent
{
    /// <summary>
    /// Input System Move �׼��� �߻��ϸ� ȣ��. context���� ���� �ൿ
    /// Move: ���콺 ��ġ�� �̵�
    /// StopMove: �̵� ����
    /// </summary>
    /// <param name="context"></param>
    public void HandleInput(InputAction.CallbackContext context)
    {
        if (!photonView.IsMine) return;

        switch (context.action.name)
        {
            case "Move":
                Vector3 mousePosition = Utility.GetMousePosition(Camera.main, "Ground").Value;

                if (mousePosition != null)
                {
                    mousePosition.y = transform.position.y;
                    StartMoveToNewTarget(mousePosition);
                }
                break;

            case "StopMove":
                StopMove();
                break;
        }        
    }

    public void Initialize(IPlayerContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnDisabled()
    {
        throw new System.NotImplementedException();
    }

    public void OnEnabled()
    {
        throw new System.NotImplementedException();
    }

    public void Updated()
    {
        throw new System.NotImplementedException();
    }
}

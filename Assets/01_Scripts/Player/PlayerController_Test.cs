using Fusion;
using UnityEngine;
using MyGame.Utils;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// 마우스 좌/우 클릭과 q,d,f,s 키를 입력 받고 그에 따른 액션을 하는 fusion 컴포넌트
/// </summary>
public class PlayerController_Test : NetworkBehaviour
{
    private NetworkTransform _nt;

    // 플레이어 스크립트 리스트
    private List<IPlayerComponent> components = new List<IPlayerComponent>();
    private List<IUpdatedPlayerComponent> updatedComponents = new List<IUpdatedPlayerComponent>();

    private Action<NetworkInputData> OnInputAction;

    private void Awake()
    {
        _nt = GetComponent<NetworkTransform>();

        components = GetComponents<IPlayerComponent>().ToList();

        if (components.Count > 0)
        {
            foreach (IPlayerComponent component in components)
            {
                OnInputAction += component.HandleInput;
                if (component is IUpdatedPlayerComponent)
                    updatedComponents.Add(component as IUpdatedPlayerComponent);
            }
        }
    }
    public override void FixedUpdateNetwork()
    {
        foreach (IUpdatedPlayerComponent component in updatedComponents)
        {
            component.Updated();
        }

        if (GetInput(out NetworkInputData data))
        {
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON1))
            {
                _nt.Teleport(data.movePoint);
            }
            if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
            {
                Debug.Log("right click!");
            }
            if (data.buttons.IsSet(NetworkInputData.BUTTOND))
            {
                Debug.Log("D Button pressed!");
            }
            if (data.buttons.IsSet(NetworkInputData.BUTTONF))
            {
                Debug.Log("F Button pressed!");
            }
            if (data.buttons.IsSet(NetworkInputData.BUTTONQ))
            {
                Debug.Log("Q Button pressed!");
            }
        }
    }
}

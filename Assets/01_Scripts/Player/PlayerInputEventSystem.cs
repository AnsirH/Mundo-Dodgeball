using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Photon.Realtime;
using static PlayerInputAction;

public class PlayerInputEventSystem : MonoBehaviourPunCallbacks, IPlayerInputActions
{
    public UnityEvent<InputAction.CallbackContext> PlayerInputEvent = new();

    private PhotonView playerPhotonView;

    private void Awake()
    {
        playerPhotonView = GetComponent<PhotonView>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (playerPhotonView.IsMine)
            PlayerInputEvent?.Invoke(context);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (playerPhotonView.IsMine)
        {
            PlayerInputEvent?.Invoke(context);
            Debug.Log("photonView.IsMine is true");
        }
        else
        {
            Debug.Log("photonView.IsMine is false");
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (playerPhotonView.IsMine)
        {
            PlayerInputEvent?.Invoke(context);
            Debug.Log("photonView.IsMine is true");
        }
        else
        {
            Debug.Log("photonView.IsMine is false");
        }
    }

    public void OnSpellD(InputAction.CallbackContext context)
    {
        if (playerPhotonView.IsMine)
            PlayerInputEvent?.Invoke(context);
    }

    public void OnSpellF(InputAction.CallbackContext context)
    {
        if (playerPhotonView.IsMine)
            PlayerInputEvent?.Invoke(context);
    }
}

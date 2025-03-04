using DG.Tweening;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviourPunCallbacks, IPunObservable
{
    private void Update()
    {
        if (canAttackable)
        {
            axeShooter.DisplayRange(PlayerController.GetMousePosition(transform));
        }
    }

    public void ReadyToAttack()
    {
        if (!canAttackable && axeShooter.CanShoot)
        {
            axeShooter.ShowRange(true);
            canAttackable = true;
        }

        else if (canAttackable)
        {
            CancelReady();
        }
    }

    public void CancelReady()
    {
        canAttackable = false;
        axeShooter.ShowRange(false);
    }

    [PunRPC]
    public void StartAttack()
    {
        if (!photonView.IsMine) return;
        if (attackTrigger)
        {
            Vector3 direction = axeShooter.targetPoint - transform.position;
            direction.y = 0.0f;
            transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f);

            attackTrigger = false;
        }
    }

    public void CancelAttack()
    {
        if (CanAttackable)
        {
            axeShooter.ShowRange(false);
            transform.DOKill();
        }
    }

    public void SetAttack(bool value)
    {
        if (value == true)
        {
            if (canAttackable)
            {
                attackTrigger = true;
                CancelReady();
                axeShooter.targetPoint = PlayerController.GetMousePosition(transform);
            }
        }
        else
        {
            attackTrigger = false;
            canAttackable = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(axeShooter.targetPoint);
        else
            axeShooter.targetPoint = (Vector3)stream.ReceiveNext();
    }

    #region Animation Event Func
    public void SpawnAxe()
    {
        axeShooter.ShootAxe(this);
        axeObj.SetActive(false);
    }

    public void ResetAxe()
    {
        axeObj.SetActive(true);
    }
    #endregion

    [Header("References")]
    [SerializeField] AxeShooter axeShooter;
    [SerializeField] GameObject axeObj;

    private bool canAttackable = true;
    private bool attackTrigger = false;

    public bool CanAttackable => canAttackable;
    public bool AttackTrigger => attackTrigger;

    public float attackPower = 80.0f;
}
using DG.Tweening;
using MoreMountains.Feedbacks;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviourPunCallbacks
{
    private void Update()
    {
        if (canAttackable)
        {
            axeShooter.DisplayRange(PlayerController.GetMousePosition(CameraManager.Instance.firstPlayerCamera, transform));
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
                axeShooter.targetPoint = PlayerController.GetMousePosition(CameraManager.Instance.firstPlayerCamera, transform);
            }
        }
        else
        {
            attackTrigger = false;
            canAttackable = false;
        }
    }

    #region Animation Event Func
    public void SpawnAxe()
    {
        axeShooter.ShootAxe();
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
}
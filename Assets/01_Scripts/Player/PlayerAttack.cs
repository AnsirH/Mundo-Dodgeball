using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
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

    public void StartAttack()
    {
        if (canAttackable)
        {
            Vector3 direction = axeShooter.targetPoint - transform.position;
            direction.y = 0.0f;
            transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f);

            CancelReady();
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

    public void SetAttack(bool value)
    {
        if (canAttackable)
        {
            attackTrigger = value;
            if (value)
                axeShooter.targetPoint = PlayerController.GetMousePosition(CameraManager.Instance.firstPlayerCamera, transform);
        }
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
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerCharacterControl
{
    public class PlayerAttack : MonoBehaviour
    {
        private void Update()
        {
            if (!canAttackable)
            {
                Cooldown();
            }
            else
            {
                axeShooter.DisplayRange(GetMousePosition - transform.position);
            }
        }

        Vector3 GetMousePosition
        {
            get
            {
                RaycastHit hit;
                if (Physics.Raycast(CameraManager.Instance.firstPlayerCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                {
                    Vector3 result = hit.point;
                    result.y = transform.position.y;
                    return result;
                }
                else
                    return Vector3.zero;
            }
        }

        private void Cooldown()
        {
            if (currentCoolTime > 0.0f)
            {
                currentCoolTime -= Time.deltaTime;
            }
            else
            {
                currentCoolTime = 0.0f;
                canAttackable = true;
            }
        }

        public void OnAttack()
        {
            if (canAttackable)
            {
                axeShooter.ShowRange(true);
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

        public void OnSelect()
        {
            if (canAttackable)
            {
                CancelAttack();
                attackTrigger = true;

                axeShooter.ShowRange(false);

                axeShooter.targetPoint = GetMousePosition;
            }
        }

        public void StartAttack()
        {
            Vector3 direction = axeShooter.targetPoint - transform.position;
            direction.y = 0.0f;
            transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f);

            canAttackable = false;
            attackTrigger = false;
            currentCoolTime = maxCoolTime;
        }

        public void SpawnAxe()
        {
            axeShooter.SpawnAxe();
            axeObj.SetActive(false);
        }

        public void ResetAxe()
        {
            axeObj.SetActive(true);
        }

        [Header("References")]
        [SerializeField] AxeShooter axeShooter;
        [SerializeField] GameObject axeObj;

        private bool canAttackable = true;
        private bool attackTrigger = false;

        public bool CanAttackable => canAttackable;
        public bool AttackTrigger => attackTrigger;

        float currentCoolTime = 0.0f;
        public float maxCoolTime = 2.5f;

#if UNITY_EDITOR

        public void OnGUI()
        {
            GUILayout.TextField($"Current Cool Time : {currentCoolTime}");
        }

#endif
    }
}
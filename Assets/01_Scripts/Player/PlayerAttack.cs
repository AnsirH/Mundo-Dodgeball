using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerCharacterControl
{
    public class PlayerAttack
    {
        public bool CheckAttack()
        {
            return canAttack && Input.GetKeyDown(KeyCode.Q);
        }

        public void RotateTowardTarget()
        {

        }

        public void Cooldown()
        {
            if (!canAttack && currentCoolTime > 0.0f)
            {
                currentCoolTime -= Time.deltaTime;
            }
            else
            {
                currentCoolTime = 0.0f;
                canAttack = true;
            }
        }

        public void StartAttack()
        {
            canAttack = false;
            currentCoolTime = maxCoolTime;
        }

        private bool canAttack = true;

        public bool CanAttack => canAttack;

        float currentCoolTime = 0.0f;
        public float maxCoolTime = 2.5f;
    }
}
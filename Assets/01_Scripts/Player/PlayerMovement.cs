using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl
{
    public class PlayerMovement : MonoBehaviour
    {
        private void LateUpdate()
        {
            if (IsMove)
                MoveToTarget();
        }

        public void MoveToTarget()
        {
            Vector3 direction = targetPoint - transform.position;

            transform.position += moveSpeed * Time.deltaTime * direction.normalized;
            RotateToTarget(direction);

            if (Vector3.Distance(transform.position, targetPoint) <= 0.1f)
                isMove = false;
        }

        public void RotateToTarget(Vector3 direction)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), rotateSpeed * Time.deltaTime);
        }

        public void StartMove(Vector3 targetPoint)
        {
            this.targetPoint = targetPoint;
            isMove = true;
        }

        public void StopMove()
        {
            isMove = false;
        }

        private bool isMove = false;
        private Vector3 targetPoint;

        public bool IsMove => isMove;

        public float moveSpeed = 1.0f;
        public float rotateSpeed = 180.0f;
    }
}
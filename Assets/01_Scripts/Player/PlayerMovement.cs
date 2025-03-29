using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerCharacterControl
{    public class PlayerMovement : MonoBehaviour
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

        public void StartMove()
        {
            targetPoint = PlayerController.GetMousePosition(transform);
            isMove = true;
            isHold = true;

            if (holdingMoveCoroutine != null) StopCoroutine(holdingMoveCoroutine);
            holdingMoveCoroutine = StartCoroutine(HoldingMove());
        }

        public void CancelHold()
        {
            isHold = false;

            if (holdingMoveCoroutine != null) StopCoroutine(holdingMoveCoroutine);
            holdingMoveCoroutine = null;
        }

        public void StopMove()
        {
            isMove = false;
            isHold = false;

            if (holdingMoveCoroutine != null) StopCoroutine(holdingMoveCoroutine);
            holdingMoveCoroutine = null;
        }

        private IEnumerator HoldingMove()
        {
            WaitForSeconds targetSetDelay = new(0.2f);

            while (isHold)
            {
                yield return targetSetDelay;

                targetPoint = PlayerController.GetMousePosition(transform);
            }
        }

        private bool isMove = false; // 이동 체크
        private bool isHold = false; // 마우스 홀드 체크
        private Vector3 targetPoint;
        private Coroutine holdingMoveCoroutine;

        public bool IsMove => isMove;

        public float moveSpeed = 1.0f;
        public float rotateSpeed = 180.0f;
    }
}
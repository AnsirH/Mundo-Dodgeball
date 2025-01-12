using System.Collections;
using System.Collections.Generic;
namespace PlayerCharacterControl.Camera
{
    using UnityEngine;

    public class PlayerCamMovement : MonoBehaviour
    {
        private void Awake()
        {
            playerCamera = GetComponent<Camera>();
            transform.rotation = Quaternion.Euler(60, 0, 0);
        }

        private void LateUpdate()
        {
            MoveCamera();
            ClampPosition();
        }

        // 카메라 이동
        private void MoveCamera()
        {
            Vector2 mousePoint = Input.mousePosition;
            Vector3 cameraMovement = Vector3.zero;

            for (int i = 0; i < directions.Length; i++)
            {
                Vector2 direction = directions[i];
                if (IsMouseInOutline(mousePoint, direction))
                {
                    cameraMovement += new Vector3(direction.x, 0, direction.y);
                }
            }

            playerCamera.transform.Translate(moveSpeed * Time.deltaTime * cameraMovement.normalized, Space.World);
        }

        // 위치 제한
        private void ClampPosition()
        {
            float clampedX = Mathf.Clamp(transform.position.x, clampCenter.x - clampOffset.x * 0.5f, clampCenter.x + clampOffset.x * 0.5f);
            float clampedZ = Mathf.Clamp(transform.position.z, clampCenter.z - clampOffset.z * 0.5f, clampCenter.z + clampOffset.z * 0.5f);

            transform.position = new Vector3(clampedX, transform.position.y, clampedZ);
        }

        // 테두리에 마우스 위치 확인
        private bool IsMouseInOutline(Vector2 mousePosition, Vector2 direction)
        {
            bool horizontalCheck = (direction.x < 0 && mousePosition.x <= margin) || (direction.x > 0 && mousePosition.x >= Screen.width - margin);

            bool verticalCheck = (direction.y < 0 && mousePosition.y <= margin) || (direction.y > 0 && mousePosition.y >= Screen.height - margin);

            return horizontalCheck || verticalCheck;
        }

        [Header("수치")]
        public float moveSpeed = 5.0f;
        public float margin = 20.0f;
        public Vector3 clampCenter;
        public Vector3 clampOffset;

        private Camera playerCamera;

        private readonly Vector2[] directions =
        {
            new ( 0,  1),  // UP
            new ( 1,  1),  // RIGHT UP
            new ( 1,  0),  // RIGHT
            new ( 1, -1),  // RIGHT DOWN
            new ( 0, -1),  // DOWN
            new (-1, -1),  // LEFT DOWN
            new (-1,  0),  // LEFT
            new (-1,  1)   // LEFT UP
        };

#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(clampCenter, clampOffset);
        }
#endif
    }
}
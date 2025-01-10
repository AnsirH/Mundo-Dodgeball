using System.Collections;
using System.Collections.Generic;
namespace Player.Camera
{
    using UnityEngine;

    public class PlayerCamMovement : MonoBehaviour
    {
        private void Awake()
        {
            playerCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            MoveCamera();
        }

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

        private bool IsMouseInOutline(Vector2 mousePosition, Vector2 direction)
        {
            bool horizontalCheck = (direction.x < 0 && mousePosition.x <= margin) || (direction.x > 0 && mousePosition.x >= Screen.width - margin);

            bool verticalCheck = (direction.y < 0 && mousePosition.y <= margin) || (direction.y > 0 && mousePosition.y >= Screen.height - margin);

            return horizontalCheck || verticalCheck;
        }

        [Header("¼öÄ¡")]
        public float moveSpeed = 5.0f;
        public float margin = 20.0f;

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
    }
}
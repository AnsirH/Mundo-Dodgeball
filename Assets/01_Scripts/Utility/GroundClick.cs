using UnityEngine;

namespace MyGame.Utils
{
    public interface IMousePositionGetter
    {
        public Vector3? ClickPoint { get; }
        public Vector3? GetMousePosition();
    }

    public class GroundClick
    {
        // 카메라와 레이어를 받아 마우스의 월드 좌표를 리턴
        public static Vector3? GetMousePosition(Camera camera, LayerMask groundLayer)
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, groundLayer))
                return hit.point;
            else
                return null;
        }

        //public static 
    }
}

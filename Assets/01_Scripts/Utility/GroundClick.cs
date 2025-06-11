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
        // ī�޶�� ���̾ �޾� ���콺�� ���� ��ǥ�� ����
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

using UnityEngine;

namespace MyGame.Utils
{
    public interface IMousePositionGetter
    {
        public Vector3? ClickPoint { get; }
        public Vector3? GetMousePosition();
    }
}

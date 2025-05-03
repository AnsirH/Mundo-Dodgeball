using UnityEngine;

namespace MyGame.Utils
{
    public interface IMousePositionGetter
    {
        public void SetClickableGroundLayer(string groundLayer);

        public string GroundLayer { get; }

        public Vector3? ClickPoint { get; }
        public Vector3? GetMousePosition(LayerMask groundLayer);
    }

    //public enum ClickableLayer
    //{
    //    All = 0,
    //    Player_1,
    //    Player_2
    //}
}

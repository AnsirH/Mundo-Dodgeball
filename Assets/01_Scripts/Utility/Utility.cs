using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MyGame.Utils
{
    public class Utility : MonoBehaviour
    {
        /// <summary>
        /// Sprite를 Base64 문자열로 변환
        /// </summary>
        public static string SpriteToBase64(Sprite sprite)
        {
            Texture2D texture = SpriteToTexture2D(sprite);
            byte[] imageBytes = texture.EncodeToPNG();
            return Convert.ToBase64String(imageBytes);
        }

        /// <summary>
        /// Base64 문자열을 Sprite로 변환
        /// </summary>
        public static Sprite Base64ToSprite(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            return Texture2DToSprite(texture);
        }

        /// <summary>
        /// Sprite를 Texture2D로 변환
        /// </summary>
        private static Texture2D SpriteToTexture2D(Sprite sprite)
        {
            Rect rect = sprite.rect;
            Texture2D texture = new Texture2D((int)rect.width, (int)rect.height);
            Color[] pixels = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Texture2D를 Sprite로 변환
        /// </summary>
        private static Sprite Texture2DToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }


        public static IEnumerator DownloadImage(string url, Image targetImage)
        {
            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(www);
                    targetImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.LogError("이미지 다운로드 실패: " + www.error);
                }
            }
        }

        public static Vector3? GetMousePosition(Camera camera, string layerName = "Ground")
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask(layerName)))
                return hit.point;
            else
                return null;
        }
    }
}

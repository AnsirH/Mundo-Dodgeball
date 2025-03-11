using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    // 싱글톤 인스턴스를 저장할 정적 변수
    private static SteamManager instance;

    // Steam API 초기화 성공 여부
    private bool initialized = false;

    // 외부에서 Steam API가 현재 활성화(초기화)되었는지 간단히 확인할 수 있는 프로퍼티
    public static bool Initialized
    {
        get
        {
            // 싱글톤 오브젝트가 존재하고, 초기화도 끝났는지 검사
            return instance != null && instance.initialized;
        }
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        try
        {
            initialized = SteamAPI.Init();
            if (!initialized)
            {
                Debug.LogError("SteamAPI.Init() failed.");
            }
        }
        catch (System.DllNotFoundException e)
        {
            Debug.LogError("[Steamworks] Could not load steam_api: " + e);
            return;
        }
    }

    void Update()
    {
        if (initialized)
        {
            SteamAPI.RunCallbacks();
        }
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            if (initialized)
            {
                SteamAPI.Shutdown();
            }
            instance = null;
        }
    }
    #region 플레이어 네임 불러오기
    /// <summary>
    /// 현재 로그인된 스팀 유저의 닉네임을 반환한다.
    /// 스팀이 초기화되지 않았다면 에러 로그를 남기고 "Unknown" 반환.
    /// </summary>
    public static string GetSteamName()
    {
        if (!Initialized)
        {
            return "Unknown";
        }

        // SteamFriends.GetPersonaName() 로컬 유저의 프로필 이름 반환
        return SteamFriends.GetPersonaName();
    }

    //public static Sprite GetSteamAvatar()
    //{
    //    if (!Initialized)
    //    {
    //        return null;
    //    }

    //    // 로컬 유저의 SteamID
    //    CSteamID steamId = SteamUser.GetSteamID();

    //    // 큰 사이즈 아바타 이미지 핸들
    //    int avatarInt = SteamFriends.GetLargeFriendAvatar(steamId);
    //    if (avatarInt == 0)
    //    {
    //        Debug.LogWarning("Failed to get large avatar handle. (Avatar might not be loaded yet)");
    //        return null;
    //    }

    //    // 이미지 크기(Width, Height) 가져오기
    //    uint width, height;
    //    bool success = SteamUtils.GetImageSize(avatarInt, out width, out height);
    //    if (!success || width == 0 || height == 0)
    //    {
    //        Debug.LogWarning("Invalid avatar dimensions.");
    //        return null;
    //    }

    //    // RGBA 형식으로 픽셀 데이터를 가져올 버퍼 생성
    //    byte[] imageData = new byte[width * height * 4];

    //    // 픽셀 데이터 로드
    //    bool gotImage = SteamUtils.GetImageRGBA(avatarInt, imageData, (int)(width * height * 4));
    //    if (!gotImage)
    //    {
    //        Debug.LogWarning("Could not get avatar RGBA data.");
    //        return null;
    //    }

    //    // <--- 추가된 부분: 위아래 뒤집기 --->
    //    imageData = FlipVerticallyRGBA(imageData, width, height);

    //    // 바이트 배열 -> Texture2D 변환
    //    Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
    //    texture.LoadRawTextureData(imageData);
    //    texture.Apply();

    //    // Texture2D -> Sprite 변환
    //    Rect rect = new Rect(0, 0, (int)width, (int)height);
    //    Vector2 pivot = new Vector2(0.5f, 0.5f); // 스프라이트 중심
    //    Sprite newSprite = Sprite.Create(texture, rect, pivot);

    //    return newSprite;
    //}

    ///// <summary>
    ///// RGBA 바이트 배열을 세로 방향으로 뒤집어주는 메서드
    ///// </summary>
    //private static byte[] FlipVerticallyRGBA(byte[] original, uint width, uint height)
    //{
    //    int rowSize = (int)(width * 4);
    //    byte[] flipped = new byte[original.Length];

    //    for (int y = 0; y < height; y++)
    //    {
    //        int srcIndex = y * rowSize;
    //        int dstIndex = (int)((height - 1 - y) * rowSize);

    //        // 한 줄(rowSize 바이트)을 통째로 복사
    //        System.Buffer.BlockCopy(original, srcIndex, flipped, dstIndex, rowSize);
    //    }

    //    return flipped;
    //}
    #endregion

    // 플레이어 저장 파일 이름 가져오기
    private static string GetPlayerFileName()
    {
        if (!SteamManager.Initialized) return "playerData_default.dat"; // Steam API가 비활성화된 경우
        ulong steamID = SteamUser.GetSteamID().m_SteamID; // 현재 로그인한 유저의 Steam ID
        return $"playerData_{steamID}.dat"; // 유저별 고유한 파일명
    }
}

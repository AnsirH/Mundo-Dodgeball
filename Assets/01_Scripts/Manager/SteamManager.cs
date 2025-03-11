using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamManager : MonoBehaviour
{
    // �̱��� �ν��Ͻ��� ������ ���� ����
    private static SteamManager instance;

    // Steam API �ʱ�ȭ ���� ����
    private bool initialized = false;

    // �ܺο��� Steam API�� ���� Ȱ��ȭ(�ʱ�ȭ)�Ǿ����� ������ Ȯ���� �� �ִ� ������Ƽ
    public static bool Initialized
    {
        get
        {
            // �̱��� ������Ʈ�� �����ϰ�, �ʱ�ȭ�� �������� �˻�
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
    #region �÷��̾� ���� �ҷ�����
    /// <summary>
    /// ���� �α��ε� ���� ������ �г����� ��ȯ�Ѵ�.
    /// ������ �ʱ�ȭ���� �ʾҴٸ� ���� �α׸� ����� "Unknown" ��ȯ.
    /// </summary>
    public static string GetSteamName()
    {
        if (!Initialized)
        {
            return "Unknown";
        }

        // SteamFriends.GetPersonaName() ���� ������ ������ �̸� ��ȯ
        return SteamFriends.GetPersonaName();
    }

    //public static Sprite GetSteamAvatar()
    //{
    //    if (!Initialized)
    //    {
    //        return null;
    //    }

    //    // ���� ������ SteamID
    //    CSteamID steamId = SteamUser.GetSteamID();

    //    // ū ������ �ƹ�Ÿ �̹��� �ڵ�
    //    int avatarInt = SteamFriends.GetLargeFriendAvatar(steamId);
    //    if (avatarInt == 0)
    //    {
    //        Debug.LogWarning("Failed to get large avatar handle. (Avatar might not be loaded yet)");
    //        return null;
    //    }

    //    // �̹��� ũ��(Width, Height) ��������
    //    uint width, height;
    //    bool success = SteamUtils.GetImageSize(avatarInt, out width, out height);
    //    if (!success || width == 0 || height == 0)
    //    {
    //        Debug.LogWarning("Invalid avatar dimensions.");
    //        return null;
    //    }

    //    // RGBA �������� �ȼ� �����͸� ������ ���� ����
    //    byte[] imageData = new byte[width * height * 4];

    //    // �ȼ� ������ �ε�
    //    bool gotImage = SteamUtils.GetImageRGBA(avatarInt, imageData, (int)(width * height * 4));
    //    if (!gotImage)
    //    {
    //        Debug.LogWarning("Could not get avatar RGBA data.");
    //        return null;
    //    }

    //    // <--- �߰��� �κ�: ���Ʒ� ������ --->
    //    imageData = FlipVerticallyRGBA(imageData, width, height);

    //    // ����Ʈ �迭 -> Texture2D ��ȯ
    //    Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false);
    //    texture.LoadRawTextureData(imageData);
    //    texture.Apply();

    //    // Texture2D -> Sprite ��ȯ
    //    Rect rect = new Rect(0, 0, (int)width, (int)height);
    //    Vector2 pivot = new Vector2(0.5f, 0.5f); // ��������Ʈ �߽�
    //    Sprite newSprite = Sprite.Create(texture, rect, pivot);

    //    return newSprite;
    //}

    ///// <summary>
    ///// RGBA ����Ʈ �迭�� ���� �������� �������ִ� �޼���
    ///// </summary>
    //private static byte[] FlipVerticallyRGBA(byte[] original, uint width, uint height)
    //{
    //    int rowSize = (int)(width * 4);
    //    byte[] flipped = new byte[original.Length];

    //    for (int y = 0; y < height; y++)
    //    {
    //        int srcIndex = y * rowSize;
    //        int dstIndex = (int)((height - 1 - y) * rowSize);

    //        // �� ��(rowSize ����Ʈ)�� ��°�� ����
    //        System.Buffer.BlockCopy(original, srcIndex, flipped, dstIndex, rowSize);
    //    }

    //    return flipped;
    //}
    #endregion

    // �÷��̾� ���� ���� �̸� ��������
    private static string GetPlayerFileName()
    {
        if (!SteamManager.Initialized) return "playerData_default.dat"; // Steam API�� ��Ȱ��ȭ�� ���
        ulong steamID = SteamUser.GetSteamID().m_SteamID; // ���� �α����� ������ Steam ID
        return $"playerData_{steamID}.dat"; // ������ ������ ���ϸ�
    }
}

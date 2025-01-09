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

    // 스크립트가 생성(오브젝트가 로드)될 때 자동으로 호출
    void Awake()
    {
        // 만약 이미 instance가 존재한다면(= 다른 SteamManager가 있다면),
        // 중복 생성을 막기 위해 이 오브젝트는 제거
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // 그렇지 않다면, 이 오브젝트를 싱글톤으로 설정
        instance = this;

        // 씬 전환 시에도 파괴되지 않도록 설정 (Steam API는 게임 전체에서 유지가 필요)
        DontDestroyOnLoad(gameObject);

        try
        {
            // Steamworks.Net 사용: Steam 클라이언트와 연결 시도
            // - 정상 연결되면 true 반환, 실패 시 false
            initialized = SteamAPI.Init();
            if (!initialized)
            {
                // 실패 시 에러 메시지 출력
                Debug.LogError("SteamAPI.Init() failed.");
            }
        }
        catch (System.DllNotFoundException e)
        {
            // steam_api.dll 또는 libsteam_api.so 파일을 찾지 못할 경우
            Debug.LogError("[Steamworks] Could not load steam_api: " + e);
            return;
        }
    }

    // 매 프레임마다 Unity가 자동으로 호출
    void Update()
    {
        // Steam API가 정상적으로 초기화되었을 때만 콜백 실행
        if (initialized)
        {
            // RunCallbacks()는 Steamworks에서 이벤트(업적, 통계 업데이트 등)를
            // 전달받는 핵심 메서드. 매 프레임마다 호출해주어야 콜백이 정상 처리됨
            SteamAPI.RunCallbacks();
        }
    }

    // 오브젝트가 제거될 때(씬 종료 혹은 게임 종료 시) 자동으로 호출
    void OnDestroy()
    {
        // 싱글톤 인스턴스와 동일한 오브젝트인지 확인
        if (instance == this)
        {
            // Steam이 초기화되어 있었다면, 안전하게 Shutdown() 호출
            if (initialized)
            {
                SteamAPI.Shutdown();
            }
            // 싱글톤 참조 해제
            instance = null;
        }
    }
}

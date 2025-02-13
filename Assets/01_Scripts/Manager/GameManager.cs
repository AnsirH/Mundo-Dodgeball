using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region 싱글톤
    private static GameManager instance;

    // 외부에서 instance를 가져올 때는 이 프로퍼티를 사용
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                // 씬 내에서 ServerManager를 찾거나, 
                // 필요하다면 런타임에 새 오브젝트를 생성할 수도 있음
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    // 만약 씬에 ServerManager가 없다면 새로 생성 (선택 사항)
                    GameObject obj = new GameObject(typeof(GameManager).Name);
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // 이미 인스턴스가 있으면, 자기 자신을 파괴해 중복을 막음
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // 없으면 이 인스턴스를 사용
        instance = this;

        // 씬 전환 시 파괴되지 않도록 설정
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }
}

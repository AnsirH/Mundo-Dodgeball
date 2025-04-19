using MoreMountains.Feedbacks;
using Photon.Pun;
using Photon.Realtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayerHealth : MonoBehaviourPunCallbacks, IPunObservable, IDamageable, IPlayerComponent
{
    private IPlayerContext context;
    private bool isOfflineMode;
    [SerializeField] MMF_Player damageTestController;
    private MMF_FloatingText mMF_FloatingText;

    public bool Controllable { get; set; } = true;

    private void Start()
    {
        foreach (var feedback in damageTestController.FeedbacksList)
        {
            if (feedback is MMF_FloatingText ft)
            {
                mMF_FloatingText = ft;
                break;
            }
        }
    }
    public void Initialize(IPlayerContext context, bool isOfflineMode = false)
    {
        this.context = context;
        this.isOfflineMode = isOfflineMode;
        context.Stats.ResetHealth();
    }

    public void Damage(float damage)
    {
        if (!context.IsLocalPlayer()) return;
        
        context.Stats.ModifyCurrentHealth(-damage);
        mMF_FloatingText.Value = damage.ToString();
        damageTestController.PlayFeedbacks(this.transform.position);
        if (context.Stats.IsDead())
        {
            context.OnPlayerDeath();
        }
    }
    public void TestDamage(float damage) // 테스트 함수임. 여기에 기능 넣어서 테스트 하세요 수고링
    {
        mMF_FloatingText.Value = damage.ToString();
        damageTestController.PlayFeedbacks(this.transform.position);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 내 로컬 currentHealth 절대값 전송
            stream.SendNext(context.Stats.GetCurrentHealth());
        }
        else
        {
            // 받은 절대값으로 덮어쓰기
            float receivedHealth = (float)stream.ReceiveNext();
            context.Stats.SetCurrentHealth(receivedHealth);
        }
    }

    public float GetHealthPercentage()
    {
        return context.Stats.GetCurrentHealth() / context.Stats.GetMaxHealth();
    }

    public void OnEnabled()
    {
        //throw new System.NotImplementedException();
    }

    public void OnDisabled()
    {
        //throw new System.NotImplementedException();
    }

    public void Updated()
    {
    }
}
[CustomEditor(typeof(PlayerHealth))]
public class FloatingTextSpawnerEditor : Editor
{
    public float damage;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PlayerHealth spawner = (PlayerHealth)target;
        GUILayout.Space(10);
        // 🔧 데미지 값 입력 필드
        damage = EditorGUILayout.FloatField("Damage", damage);
        EditorGUILayout.LabelField(" 테스트 버튼", EditorStyles.boldLabel);
        if (GUILayout.Button("Damage Test Button"))
        {
            spawner.TestDamage(damage);
        }
    }
}

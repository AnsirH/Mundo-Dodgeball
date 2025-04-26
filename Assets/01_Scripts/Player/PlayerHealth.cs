using MoreMountains.Feedbacks;
using Photon.Pun;
using UnityEditor;
using UnityEngine;

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
    // 실제 Damage 호출할 때 (senderContext 가지고 있을 때)
    public void TakeDamage(IPlayerContext senderContext)
    {
        if (!photonView.IsMine)
            return; // 내 것만 호출하게 막기

        float attackPower = senderContext.Stats.GetAttackPower();
        photonView.RPC(nameof(Damage), RpcTarget.All, attackPower);
    }
    [PunRPC]
    public void Damage(float attackPower)
    {
        // 1. 체력 감소
        context.Stats.ModifyCurrentHealth(-attackPower);

        // 2. 데미지 텍스트 띄우기
        mMF_FloatingText.Value = attackPower.ToString();

        // 3. 데미지 이펙트 재생
        damageTestController.PlayFeedbacks(this.transform.position);

        // 4. 죽음 체크
        if (context.Stats.IsDead())
        {
            context.OnPlayerDeath();
        }
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

//#if UNITY_EDITOR
//[CustomEditor(typeof(PlayerHealth))]
//public class FloatingTextSpawnerEditor : Editor
//{
//    public float damage;
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        PlayerHealth spawner = (PlayerHealth)target;
//        GUILayout.Space(10);
//        // 🔧 데미지 값 입력 필드
//        damage = EditorGUILayout.FloatField("Damage", damage);
//        EditorGUILayout.LabelField(" 테스트 버튼", EditorStyles.boldLabel);
//        if (GUILayout.Button("Damage Test Button"))
//        {
//            spawner.Damage(damage);
//        }
//    }
//}
//#endif
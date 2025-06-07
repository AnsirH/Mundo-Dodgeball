using Fusion;
using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : NetworkBehaviour, IDamageable, IPlayerComponent
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

    public void HandleInput(NetworkInputData data)
    {
    }
    // 실제 Damage 호출할 때 (senderContext 가지고 있을 때)
    public void TakeDamage(IPlayerContext senderContext)
    {
        //if (photonView.IsMine || context.Stats.IsDead())
        //    return; // 내 것만 호출하게 막기

        //float attackPower = senderContext.Stats.GetAttackPower();
        //int whoAttacker = context.p_PhotonView.ViewID;
        //photonView.RPC(nameof(Damage), RpcTarget.All, whoAttacker, attackPower);
    }
    [Rpc]
    public void Rpc_Damage(int attackerActorNumber, float attackPower)
    {
        StartCoroutine(ActiveHitEffect());
        context.Stats.ModifyCurrentHealth(-attackPower);

        mMF_FloatingText.Value = attackPower.ToString();
        damageTestController.PlayFeedbacks(this.transform.position);

        if (context.Stats.IsDead())
        {
            Debug.Log("check LOG : isDead!");
            if (HasStateAuthority)
            {
                context.OnPlayerDeath();

                // 죽을 때 이긴 사람 점수 올리기;
                sendAddScore(attackerActorNumber, 1);
            }
        }
    }
    private void sendAddScore(int idx, int vaule)
    {
        object[] content = new object[] { idx, vaule};

        //RaiseEventOptions options = new RaiseEventOptions
        //{
        //    Receivers = ReceiverGroup.MasterClient // 모든 플레이어에게 브로드캐스트
        //};

        //SendOptions sendOptions = new SendOptions
        //{
        //    Reliability = true
        //};
        //PhotonNetwork.RaiseEvent(NetworkEventCodes.AddScoreEvent, content, options, sendOptions);
    }
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        // 내 로컬 currentHealth 절대값 전송
    //        stream.SendNext(context.Stats.GetCurrentHealth());
    //    }
    //    else
    //    {
    //        // 받은 절대값으로 덮어쓰기
    //        float receivedHealth = (float)stream.ReceiveNext();
    //        context.Stats.SetCurrentHealth(receivedHealth);
    //    }
    //}

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
        context.Stats.HandleHealthRegen();
    }

    IEnumerator ActiveHitEffect()
    {
        GameObject hitEffect = ObjectPooler.Get("HitEffect");
        hitEffect.transform.position = transform.position;
        yield return new WaitForSeconds(0.5f);

        ObjectPooler.Release("HitEffect", hitEffect);
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
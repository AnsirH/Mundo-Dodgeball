using Fusion;
using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour, IDamageable
{
    private IPlayerContext context;
    [SerializeField] MMF_Player damageTestController;
    private MMF_FloatingText mMF_FloatingText;
    public bool IsDead => context.Stats.CurrentStatData.Health <= 0.0f;
    public float HealthPercentage => context.Stats.CurrentStatData.Health / context.Stats.GetMaxHealth();
    public RectTransform testHpBar;

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

    public void Initialize(IPlayerContext context)
    {
        this.context = context;
        context.Stats.ResetHealth();
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0.0f) return;

        context.Stats.ModifyCurrentHealth(-damage);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Damage_RPC(float damage)
    {
        context.Stats.ModifyCurrentHealth(-damage);
        //mMF_FloatingText.Value = attackPower.ToString();
        //damageTestController.PlayFeedbacks(this.transform.position);
    }

    public override void FixedUpdateNetwork()
    {
        if (testHpBar != null)
        {
            testHpBar.localScale = new Vector3(HealthPercentage, 1.0f, 1.0f);
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


    IEnumerator ActiveHitEffect()
    {
        GameObject hitEffect = ObjectPooler.GetLocal("HitEffect");
        hitEffect.transform.position = transform.position;
        yield return new WaitForSeconds(0.5f);

        ObjectPooler.ReleaseLocal("HitEffect", hitEffect);
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
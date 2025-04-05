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
    private float currentHealth;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRegenPerSec = 7.0f;
    [SerializeField] MMF_Player damageTestController;
    private MMF_FloatingText mMF_FloatingText;

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
        currentHealth = maxHealth;
    }

    private void HealthRegen()
    {
        currentHealth += healthRegenPerSec * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void Damage(float damage)
    {
        if (!context.IsLocalPlayer()) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);

        mMF_FloatingText.Value = damage.ToString();
        damageTestController.PlayFeedbacks(this.transform.position);
        if (currentHealth <= 0)
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
            stream.SendNext(currentHealth);
        else
            currentHealth = (float)stream.ReceiveNext();
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public void Updated()
    {
        HealthRegen();
    }

    public void OnEnabled()
    {
        throw new System.NotImplementedException();
    }

    public void OnDisabled()
    {
        throw new System.NotImplementedException();
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
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

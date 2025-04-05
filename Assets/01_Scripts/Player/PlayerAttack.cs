using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviourPunCallbacks, IPunObservable, IPlayerComponent
{
    private IPlayerContext context;
    private bool canAttackable = true;
    private bool attackTrigger = false;

    public bool CanAttackable => canAttackable;
    public bool AttackTrigger => attackTrigger;
    public float attackPower = 80.0f;

    [Header("References")]
    [SerializeField] private AxeShooter axeShooter;
    [SerializeField] private GameObject axeObj;

    public void Initialize(IPlayerContext context)
    {
        this.context = context;
    }

    public void ReadyToAttack()
    {
        if (!canAttackable && axeShooter.CanShoot)
        {
            axeShooter.ShowRange(true);
            canAttackable = true;
        }
        else if (canAttackable)
        {
            CancelReady();
        }
    }

    public void CancelReady()
    {
        canAttackable = false;
        axeShooter.ShowRange(false);
    }

    [PunRPC]
    public void StartAttack()
    {
        if (!context.IsLocalPlayer()) return;
        
        if (attackTrigger)
        {
            Vector3 direction = axeShooter.targetPoint - context.Pos;
            direction.y = 0.0f;
            transform.DORotateQuaternion(Quaternion.LookRotation(direction), 0.25f);
            attackTrigger = false;
        }
    }

    public void CancelAttack()
    {
        if (CanAttackable)
        {
            axeShooter.ShowRange(false);
            transform.DOKill();
        }
    }

    public void SetAttack(bool value)
    {
        if (value)
        {
            if (canAttackable)
            {
                attackTrigger = true;
                CancelReady();
                axeShooter.targetPoint = context.GetMousePosition().Value;
            }
        }
        else
        {
            attackTrigger = false;
            canAttackable = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(axeShooter.targetPoint);
        else
            axeShooter.targetPoint = (Vector3)stream.ReceiveNext();
    }

    #region Animation Event Functions
    public void SpawnAxe()
    {
        axeShooter.ShootAxe(this);
        axeObj.SetActive(false);
    }

    public void ResetAxe()
    {
        axeObj.SetActive(true);
    }

    public void Updated()
    {
        if (canAttackable)
        {
            Vector3? mousePosition = context.GetMousePosition();
            if (mousePosition != null)
                axeShooter.DisplayRange(mousePosition.Value);
        }
    }

    public void OnEnabled()
    {
        throw new System.NotImplementedException();
    }

    public void OnDisabled()
    {
        CancelAttack();
    }

    public void HandleInput(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
    #endregion

}
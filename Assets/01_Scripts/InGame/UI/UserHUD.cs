using NUnit.Framework;
using UnityEngine;

public class UserHUD : MonoBehaviour
{
    public HpBar _hpBar;
    public SkilllUI _spell_D_UI;
    public SkilllUI _spell_F_UI;
    public SkilllUI _q_UI;

    private IPlayerContext _playerContext;
    public void Init(IPlayerContext playerContext)
    {
        _playerContext = playerContext;

        _hpBar.Init();
        _spell_D_UI.Init();
        _spell_F_UI.Init();
        _q_UI.Init();
    }

    public void UpdateHpBar(float hpRatio)
    {
        _hpBar.UpdateHpBar(hpRatio);
    }

    public void UpdateSpellDCoolTime(float remainingCoolTime, float maxCoolTime) => _spell_D_UI.UpdateCoolTime(remainingCoolTime, maxCoolTime);

    public void UpdateSpellFCoolTime(float remainingCoolTime, float maxCoolTime) => _spell_F_UI.UpdateCoolTime(remainingCoolTime, maxCoolTime);

    public void UpdateQCoolTime(float remainingCoolTime, float maxCoolTime) => _q_UI.UpdateCoolTime(remainingCoolTime, maxCoolTime);

    public void UpdateHud()
    {
        if (_playerContext != null && _playerContext.Runner != null)
        {
            UpdateHpBar(_playerContext.Health.CurrentHealth / _playerContext.Stats.GetMaxHealth());
            UpdateSpellDCoolTime(_playerContext.Spell.CoolTimeD, _playerContext.Spell.spellD._maxCoolTime);
            UpdateSpellFCoolTime(_playerContext.Spell.CoolTimeF, _playerContext.Spell.spellF._maxCoolTime);
            UpdateQCoolTime(_playerContext.Attack.CoolTime, _playerContext.Stats.GetAttackCooldown());
        }
    }
}

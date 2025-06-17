using Fusion;
using UnityEngine;

namespace Mundo_dodgeball.Spell
{
    [CreateAssetMenu(fileName = "NewSpellData", menuName = "Spell Data/Create New Spell")]
    public class SpellData : ScriptableObject
    {
        public SpellCategory _category;
        public string _description = "";
        public float _maxCoolTime = 60.0f;
        public float _valueAmount = 0.0f;
        public NetworkPrefabRef _effectPrefab;
    }

    public enum SpellCategory
    {
        None = 0,
        Heal,
        Flash
    }
}

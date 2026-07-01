using UnityEngine;

namespace RPGCombat.Data
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "RPGCombat/Character Data")]
    public class CharacterDataSo : ScriptableObject
    {
        [Header("Identidad")]
        public string characterName;

        [Header("Stats base")]
        public int maxHP;
        public int speed;

        [Header("Ataque cuerpo a cuerpo")]
        public int meleeAttackDamage;

        [Header("Ataque de rango")]
        public bool hasRangeAttack;
        public int rangeAttackDamage;
        public int rangeAttackMaxDistance; // 99 = sin límite (Ranger)

        [Header("Curación")]
        public bool canHeal;
        public int healAmount;
        public int healMaxDistance; // 0 = solo a sí mismo, 1 = adyacente, 99 = cualquiera
        public bool canHealAllies;  // false = solo a sí mismo
    }
}


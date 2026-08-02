using UnityEngine;
using RPGCombat.Characters;
using UnityEngine.TextCore.Text;

namespace RPGCombat.Combat 
{
    // SRP ejecuta acciones. No gestiona turnos ni UI
    // DIP trabaja con ICharacter, no con clases concretas (salvo para el heal amount)
    public class CombatActions : MonoBehaviour
    {
        public bool TryMeleeAttack(ICharacter attacker, ICharacter target)
        {
            if (!attacker.CanMeleeAttack(target)) return false;
            target.TakeDamage(attacker.MeleeAttackDamage);
#if UNITY_EDITOR
            Debug.Log($"{attacker.CharacterName} -> melee -> {target.CharacterName} ({attacker.MeleeAttackDamage} dmg)");
#endif
            return true;
        }

        public bool TryRangeAttack(ICharacter attacker, ICharacter target)
        {
            if (!attacker.CanRangeAttack(target)) return false;
            target.TakeDamage(attacker.RangeAttackDamage);
#if UNITY_EDITOR
            Debug.Log($"{attacker.CharacterName} -> rango -> {target.CharacterName} ({attacker.RangeAttackDamage} dmg)");
#endif
            return true;
        }

        public bool TryHeal(ICharacter healer, ICharacter target)
        {
            if (!healer.CanHealTarget(target)) return false;

            if (healer is Characters.Character ch)
                target.Heal(healer.HealAmount);

            return true;
        }
    }
}
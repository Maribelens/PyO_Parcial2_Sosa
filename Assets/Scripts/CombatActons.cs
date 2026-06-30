using UnityEngine;
using RPGCombat.Characters;

namespace RPGCombat.Combat 
{
    // SRP: solo ejecuta acciones. No gestiona turnos ni UI
    // DIP: trabaja con ICharacter, no con clases concretas (salvo para el heal amount)
    public class CombatActons : MonoBehaviour
    {
        public bool TryMeleeAttack(ICharacter attacker, ICharacter target)
        {
            if (!attacker.CanMeleeAttack(target)) return false;
            target.TakeDamage(attacker.MeleeAttackDamage);
            Debug.Log($"{attacker.CharacterName} -> melee -> {target.CharacterName} ({attacker.MeleeAttackDamage} dmg)");
            return true;
        }

        public bool TryRangeAttack(ICharacter attacker, ICharacter target)
        {
            if (!attacker.CanRangeAttack(target)) return false;
            target.TakeDamage(attacker.RangeAttackDamage);
            Debug.Log($"{attacker.CharacterName} -> rango -> {target.CharacterName} ({attacker.RangeAttackDamage} dmg)");
            return true;
        }

        public bool TryHeal(ICharacter healer, ICharacter target)
        {
            if (!healer.CanHealTarget(target)) return false;

            // Patron Strategy: cada personaje sabe cuanto cura
            int amount = healer switch
            {
                Fighter f => f.GetHealAmount(),
                Healer h => h.GetHealAmount(),
                Ranger r => r.GetHealAmount(),
                _ => 0
            };

            if (amount == 0) return false;
            target.Heal(amount);
            Debug.Log($"{healer.CharacterName} -> cura -> {target.CharacterName} (+{amount} HP)");
            return true;
        }
    }
}


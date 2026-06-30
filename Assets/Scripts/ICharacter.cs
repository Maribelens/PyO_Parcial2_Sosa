using UnityEngine;
public interface ICharacter
{
    string CharacterName { get; }
    int CurrentHP { get; }
    int MaxHP { get; }
    int Speed { get; }
    int MeleeAttackDamage { get; }
    bool HasRangedAttack { get; }
    int RangeAttackDamage { get; }
    int RangeAttackMaxDistance { get; }
    bool CanHeal { get; }
    bool IsAlive { get; }
    Vector2Int GridPosition { get; set; }

    void TakeDamage(int damage);
    void Heal(int amount);
    bool CanMeleeAttack(ICharacter target);
    bool CanRangeAttack(ICharacter target);
    bool CanHealTarget(ICharacter target);
}

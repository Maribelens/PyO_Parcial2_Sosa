
namespace RPGCombat.Characters
{
    // Player 1: 20HP | vel 3 | melee -5HP | sin rango | cura +2HP solo a sí
    public class Fighter : Chararter
    {
        private const int HealAmount = 2;

        public override bool HasRangeAttack => false;
        public override bool CanHeal => true;

        public override bool CanHealTarget(ICharacter target)
            => CanHeal && target == (ICharacter)this && IsAlive;

        public int GetHealAmount() => HealAmount;
    }
}
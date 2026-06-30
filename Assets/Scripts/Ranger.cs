
namespace RPGCombat.Characters
{
    //Player 3: 15HP | vel 4 | melee -1HP | rango -3HP (cualquier distancia) | cura +2HP solo a sí mismo
    public class Ranger : Chararter
    {
        private const int HealAmount = 2;

        public override bool HasRangeAttack => true;
        public override bool CanHeal => true;

        // Sobreescribe: rango ilimitado (distancia > 1, sin tope)
        public override bool CanRangeAttack(ICharacter target)
        {
            if (target == null || !target.IsAlive) return false;
            return ManhattanDistance(GridPosition, target.GridPosition) > 1;
        }
        public override bool CanHealTarget(ICharacter target)
            => CanHeal && target == (ICharacter)this && IsAlive;

        public int GetHealAmount() => HealAmount;
    }
}
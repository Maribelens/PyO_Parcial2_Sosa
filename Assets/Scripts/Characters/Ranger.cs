
namespace RPGCombat.Characters
{
    //Player 3: 15HP | vel 4 | melee -1HP | rango -3HP (cualquier distancia) | cura +2HP solo a sí mismo
    public class Ranger : Chararter
    {
        // Sobreescribe: rango ilimitado (distancia > 1, sin tope)
        // Rango ilimitado: sobreescribe solo CanRangeAttack
        public override bool CanRangeAttack(ICharacter target)
        {
            if (!data.hasRangeAttack || target == null || !target.IsAlive) return false;
            return ManhattanDistance(GridPosition, target.GridPosition) > 1;
        }

        public override bool CanHealTarget(ICharacter target)
            => data.canHeal && target == (ICharacter)this && IsAlive;
    }
}
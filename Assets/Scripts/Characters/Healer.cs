
namespace RPGCombat.Characters
{
    // Player 2: 15HP | vel 2 | melee -2HP | rango -2HP (max 3) | cura +5HP
    public class Healer : Chararter
    {
        public override bool CanHealTarget(ICharacter target)
        {
            if (!data.canHeal || target == null || !target.IsAlive) return false;
            int dist = ManhattanDistance(GridPosition, target.GridPosition);
            return dist <= data.healMaxDistance;
        }
    }
}

namespace RPGCombat.Characters
{
    // Player 2: 15HP | vel 2 | melee -2HP | rango -2HP (max 3) | cura +5HP
    public class Healer : Chararter
    {
        private const int HealAmount = 5;
        private const int HealMaxDistance = 1; //distancia menor a 2

        public override bool HasRangeAttack => true;
        public override bool CanHeal => true;

        public override bool CanHealTarget(ICharacter target)
        {
            if (!CanHeal || target == null || !target.IsAlive) return false;
            int dist = ManhattanDistance(GridPosition, target.GridPosition);
            return dist <= HealMaxDistance;
        }

        public int GetHealAmount() => HealAmount;
    }
}
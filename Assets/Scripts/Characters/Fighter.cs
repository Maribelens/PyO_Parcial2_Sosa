
namespace RPGCombat.Characters
{
    // Player 1: 20HP | vel 3 | melee -5HP | sin rango | cura +2HP solo a sí
    public class Fighter : Chararter
    {
        public override bool CanHealTarget(ICharacter target)
            => data.canHeal && target == (ICharacter)this && IsAlive;
    }
}
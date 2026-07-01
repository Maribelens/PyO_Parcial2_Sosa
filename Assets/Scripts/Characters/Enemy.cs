
namespace RPGCombat.Characters
{
    // Enemigo: 10HP | vel 1 | melee -3HP | rango -1HP (max 3) | no cura
    public class Enemy : Chararter
    {
        public override bool CanHealTarget(ICharacter target) => false;
    }
}
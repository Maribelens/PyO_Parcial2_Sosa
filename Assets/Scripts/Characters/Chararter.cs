using UnityEngine;
using RPGCombat.Data;
using RPGCombat.Characters;

// OCP: abierta a extensión mediante subclases, cerrada a modificación

namespace RPGCombat.Characters
{
    public abstract class Chararter : MonoBehaviour, ICharacter
    {
        [Header("Datos del personaje")]
        [SerializeField] protected CharacterDataSo data;

        private int _currentHP;
        public string CharacterName => data.characterName;
        public int MaxHP => data.maxHP;
        public int Speed => data.speed;
        public int MeleeAttackDamage => data.meleeAttackDamage;
        public bool HasRangeAttack => data.hasRangeAttack;
        public int RangeAttackDamage => data.rangeAttackDamage;
        public int RangeAttackMaxDistance => data.rangeAttackMaxDistance;
        public bool CanHeal => data.canHeal;
        public int HealAmount => data.healAmount;
        public int CurrentHP => _currentHP;
        public bool IsAlive => _currentHP > 0;
        public Vector2Int GridPosition { get; set; }

        protected virtual void Awake() => _currentHP = data.maxHP;

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;
            _currentHP = Mathf.Max(0, _currentHP - damage);
            if (!IsAlive) OnDeath();
        }

        public void Heal(int amount)
        {
            if (!IsAlive) return;
            _currentHP = Mathf.Min(data.maxHP, _currentHP + amount);
        }

        // Cuerpo a cuerpo: celda contigua (distancia Chebyshev == 1, cubre diagonales)
        public bool CanMeleeAttack(ICharacter target)
        {
            if (target == null || !target.IsAlive) return false;
            return ChebyshevDistance(GridPosition, target.GridPosition) == 1;
        }

        // Rango: distancia > 1 y <= rango del personaje
        public virtual bool CanRangeAttack(ICharacter target)
        {
            if (!data.hasRangeAttack || target == null || !target.IsAlive) return false;
            int dist = ManhattanDistance(GridPosition, target.GridPosition);
            return dist > 1 && dist <= data.rangeAttackMaxDistance;
        }
 
        public abstract bool CanHealTarget(ICharacter target);
        protected virtual void OnDeath() => gameObject.SetActive(false);

        //Chebyshev: adyacencia en 8 direcciones (incluye diagonal)
        public static int ChebyshevDistance(Vector2Int a, Vector2Int b)
            => Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));

        //Manhattan: para calcular distancias de rango
        public static int ManhattanDistance(Vector2Int a, Vector2Int b)
            => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}

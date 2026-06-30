using UnityEngine;

// OCP: abierta a extensión mediante subclases, cerrada a modificación

namespace RPGCombat.Characters
{
    public abstract class Chararter : MonoBehaviour, ICharacter
    {
        [Header("Stats")]
        [SerializeField] private string characterName;
        [SerializeField] private int maxHP;
        [SerializeField] private int speed;
        [SerializeField] private int meleeAttackDamage;
        [SerializeField] private int rangeAttackDamage;
        [SerializeField] private int rangeAttackMaxDistance;

        private int _currentHP;

        public string CharacterName => characterName;
        public int CurrentHP => _currentHP;
        public int MaxHP => maxHP;
        public int Speed => speed;
        public int MeleeAttackDamage => meleeAttackDamage;
        public int RangeAttackDamage => rangeAttackDamage;
        public int RangeAttackMaxDistance => rangeAttackMaxDistance;
        public bool IsAlive => _currentHP > 0;
        public Vector2Int GridPosition { get; set; }

        //Subclases definen sus capacidades (LSP: son sustituibles por Character)
        public abstract bool HasRangeAttack { get; }
        public abstract bool CanHeal { get; }

        protected void Awake() => _currentHP = maxHP;

        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;
            _currentHP = Mathf.Max(0, _currentHP - damage);
            if (!IsAlive) OnDeath();
        }

        public void Heal(int amount)
        {
            if (!IsAlive) return;
            _currentHP = Mathf.Min(maxHP, _currentHP + amount);
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
            if (!HasRangeAttack || target == null || !target.IsAlive) return false;
            int distance = ManhattanDistance(GridPosition, target.GridPosition);
            return distance > 1 && distance <= RangeAttackMaxDistance;
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
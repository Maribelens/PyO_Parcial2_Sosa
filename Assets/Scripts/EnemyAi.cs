using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPGCombat.Characters;
using RPGCombat.Grid;

namespace RPGCombat.Combat
{
    // SRP: solo controla la IA enemiga
    // DIP: GridManager, CombatActions y TurnManager inyectados por Initialize()
    public class EnemyAi : MonoBehaviour
    {
        private GridManager gridManager;
        private CombatActions combatActions;
        private TurnManager turnManager;

        private static readonly List<Vector2Int> CardinalDirections = new()
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        public void Initialize(GridManager gm, CombatActions ca, TurnManager tm)
        {
            gridManager = gm;
            combatActions = ca;
            turnManager = tm;
        }

        public IEnumerator ExecuteAllEnemyTurns(List<Enemy> enemies, List<ICharacter> players)
        {
            foreach (var enemy in enemies.Where(e => e.IsAlive))
            {
                yield return StartCoroutine(ExecuteEnemyTurn(enemy, players));
                yield return new WaitForSeconds(0.5f);
            }
            turnManager.OnEnemyTurnComplete();
        }

        private IEnumerator ExecuteEnemyTurn(Enemy enemy, List<ICharacter> players)
        {
            var alivePlayers = players.Where(p => p.IsAlive).ToList();
            if (alivePlayers.Count == 0) yield break;

            MoveRandomly(enemy);
            yield return new WaitForSeconds(0.3f);

            var nearest = GetNearestPlayer(enemy, alivePlayers);
            if (nearest == null) yield break;

            // Intenta melee primero; si no alcanza, intenta rango
            if (!combatActions.TryMeleeAttack(enemy, nearest))
                combatActions.TryRangeAttack(enemy, nearest);
        }

        private void MoveRandomly(Enemy enemy)
        {
            for (int step = 0; step < enemy.Speed; step++)
            {
                // Mezcla las 4 direcciones y toma la primera disponible
                var shuffled = CardinalDirections.OrderBy(_ => Random.value).ToList();
                foreach (var dir in shuffled)
                {
                    if (gridManager.TryMove(enemy, enemy.GridPosition + dir))
                        break;
                }
            }
        }

        private ICharacter GetNearestPlayer(Enemy enemy, List<ICharacter> players)
        {
            var candidates = new List<ICharacter>();
            int minDist = int.MaxValue;

            foreach (var player in players)
            {
                int dist = Chararter.ManhattanDistance(enemy.GridPosition, player.GridPosition);
                if (dist < minDist) { minDist = dist; candidates.Clear(); }
                if (dist == minDist) candidates.Add(player);
            }

            return candidates.Count > 0
                ? candidates[Random.Range(0, candidates.Count)]
                : null;
        }
    }
}

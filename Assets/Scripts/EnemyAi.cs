using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPGCombat.Characters;
using RPGCombat.Grid;

namespace RPGCombat.Combat
{
    // SRP controla IA enemiga
    // DIP GridManager, CombatActions y TurnManager inyectados por Initialize()
    public class EnemyAI : MonoBehaviour
    {
        private GridManager gridManager;
        private CombatActions combatActions;
        private TurnManager turnManager;

        public void Initialize(GridManager gm, CombatActions ca, TurnManager tm)
        {
            gridManager = gm;
            combatActions = ca;
            turnManager = tm;
        }

        public IEnumerator ExecuteAllEnemyTurns(List<Enemy> enemies, List<ICharacter> players)
        {
            gridManager.HidePredictedMovement();

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

            MoveTowardsNearestPlayer(enemy, alivePlayers);
            yield return new WaitForSeconds(0.3f);

            var nearest = GetNearestPlayer(enemy, alivePlayers);
            if (nearest == null) yield break;

            // Intenta melee primero; si no alcanza, intenta rango
            if (!combatActions.TryMeleeAttack(enemy, nearest))
                combatActions.TryRangeAttack(enemy, nearest);
        }

        // Mueve al enemigo paso a paso hacia el jugador más cercano
        // Reemplaza MoveRandomly() — movimiento ahora determinista y predecible
        private void MoveTowardsNearestPlayer(Enemy enemy, List<ICharacter> characters)
        {
            for (int step = 0; step < enemy.Speed; step++)
            {
                // Recalcula el jugador más cercano en cada paso porque
                // la posición del enemigo cambia con cada paso
                var nearest = GetNearestPlayer(enemy, characters);
                if (nearest == null) break;

                Vector2Int nextPos = CalculateNextPosition(
                enemy.GridPosition,
                nearest.GridPosition
                );

                // Si la dirección principal está bloqueada, intenta el eje secundario
                if (!gridManager.TryMove(enemy, nextPos))
                {
                    var fallback = GetFallbackPosition(
                        enemy.GridPosition,
                        nearest.GridPosition
                    );
                    if (fallback.HasValue)
                        gridManager.TryMove(enemy, fallback.Value);
                }
            }
        }

        // Calcula la próxima celda en dirección al objetivo SIN mover al enemigo.
        // GetPredictedPositions() lo usa para la recompensa del ad.
        public Vector2Int CalculateNextPosition(Vector2Int from, Vector2Int target)
        {
            Vector2Int delta = target - from;

            // Prioriza el eje con mayor distancia para un movimiento más natural
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
                return from + new Vector2Int((int)Mathf.Sign(delta.x), 0);
            else
                return from + new Vector2Int(0, (int)Mathf.Sign(delta.y));
        }

        // Si la dirección principal está bloqueada, devuelve el eje secundario
        private Vector2Int? GetFallbackPosition(Vector2Int from, Vector2Int target)
        {
            Vector2Int delta = target - from;

            Vector2Int fallbackDir = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y)
                ? new Vector2Int(0, (int)Mathf.Sign(delta.y))
                : new Vector2Int((int)Mathf.Sign(delta.x), 0);

            if (fallbackDir == Vector2Int.zero) return null;

            Vector2Int candidate = from + fallbackDir;
            return gridManager.IsInBounds(candidate) && !gridManager.IsOccupied(candidate)
                ? candidate
                : (Vector2Int?)null;
        }

        private ICharacter GetNearestPlayer(Enemy enemy, List<ICharacter> players)
        {
            var candidates = new List<ICharacter>();
            int minDist = int.MaxValue;

            foreach (var player in players)
            {
                int dist = Characters.Character.ManhattanDistance(enemy.GridPosition, player.GridPosition);
                if (dist < minDist) { minDist = dist; candidates.Clear(); }
                if (dist == minDist) candidates.Add(player);
            }

            return candidates.Count > 0
                ? candidates[Random.Range(0, candidates.Count)]
                : null;
        }

        // Devuelve la celda destino de cada enemigo vivo sin moverlos.
        // Usado por la recompensa del rewarded ad para mostrar el highlight
        // en la celda a la que se va a mover cada enemigo en su próximo turno.
        public Dictionary<Enemy, Vector2Int> GetPredictedPositions(
        List<Enemy> enemies, List<ICharacter> characters)
        {
            var predictions = new Dictionary<Enemy, Vector2Int>();
            var alivePlayers = characters.Where(p => p.IsAlive).ToList();

            foreach (var enemy in enemies.Where(e => e.IsAlive))
            {
                // Simula los pasos de velocidad sin mover a nadie
                Vector2Int simulatedPos = enemy.GridPosition;
                for(int step = 0; step < enemy.Speed; step++) 
                {
                    //buscar jugador mas cercno desde la posicion simulada
                    ICharacter nearest = GetNearestPlayerFromPosition(simulatedPos, alivePlayers);
                    if (nearest == null) break;

                    Vector2Int nextPos = CalculateNextPosition(simulatedPos, nearest.GridPosition);

                    // Solo avanza si la celda está libre en la simulación
                    // (no llama a TryMove, no mueve nada real)
                    if (gridManager.IsInBounds(nextPos) && !gridManager.IsOccupied(nextPos))
                        simulatedPos = nextPos;
                    else
                    {
                        //intenta el eje secundario en la simulacion
                        var fallback = GetFallbackPosition(simulatedPos, nearest.GridPosition);
                        if (fallback.HasValue && !gridManager.IsOccupied(fallback.Value))
                            simulatedPos = fallback.Value;
                        else
                            break; //bloqueado en ambos ejes, se queda donde esta
                    }
                }

                predictions[enemy] = simulatedPos;
            }

            return predictions;
        }

        // Versión de GetNearestPlayer que opera desde una posición arbitraria
        // en vez de desde la posición actual del enemigo — necesaria para la simulación
        private ICharacter GetNearestPlayerFromPosition(Vector2Int from, List<ICharacter> characters) 
        {
            var candidates = new List<ICharacter>();
            int minDistance = int.MaxValue;

            foreach(var character in characters) 
            {
                int distance = Characters.Character.ManhattanDistance(from, character.GridPosition);
                if (distance < minDistance) { minDistance = distance; candidates.Clear(); }
                if (distance == minDistance) candidates.Add(character);
            }

            return candidates.Count > 0
                ? candidates[Random.Range(0, candidates.Count)]
                : null;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPGCombat.Characters;
using RPGCombat.Grid;

namespace RPGCombat.Combat
{
    public enum GameState { WaitingForInput, EnemyTurn, GameOver }

    // SRP: orquesta el orden de turnos y detecta fin de partida
    // DIP: recibe listas inyectadas, sin Singleton
    public class TurnManager : MonoBehaviour
    {
        [SerializeField] GridManager gridManager;
        
        private List<ICharacter> players = new();
        private List<Enemy> enemies = new();
        private int currentPlayerIndex = 0;
        private int enemyRevealTurnsRemaining = 0;

        public GameState CurrentState { get; private set; } = GameState.WaitingForInput;

        public ICharacter ActivePlayer
            => GetAlivePlayers().ElementAtOrDefault(currentPlayerIndex);

        // Inyección de dependencias (DIP)
        public void Initialize(List<ICharacter> playerList, List<Enemy> enemyList)
        {
            players = playerList;
            enemies = enemyList;
            currentPlayerIndex = 0;
            CurrentState = GameState.WaitingForInput;
        }

        public void EndPlayerTurn()
        {
            if (CurrentState != GameState.WaitingForInput) return;

            currentPlayerIndex++;

            if (currentPlayerIndex >= GetAlivePlayers().Count)
            {
                currentPlayerIndex = 0;
                CurrentState = GameState.EnemyTurn;
            }
        }

        public void OnEnemyTurnComplete()
        {
            EvaluateGameOver();
            if (CurrentState != GameState.GameOver)
                CurrentState = GameState.WaitingForInput;
        }

        public void ActivateEnemyReveal(int turns)
        {
            enemyRevealTurnsRemaining = turns;
        }

        // Llamado al final de cada ronda completa (después del turno enemigo)
        public void OnRoundEnded()
        {
            if (enemyRevealTurnsRemaining > 0)
            {
                enemyRevealTurnsRemaining--;
                if (enemyRevealTurnsRemaining == 0)
                    gridManager.HideEnemyHighlights();
            }
        }

        private void EvaluateGameOver()
        {
            bool allEnemiesDead = enemies.All(e => !e.IsAlive);
            bool anyPlayerDead = players.Any(p => !p.IsAlive);

            if (allEnemiesDead || anyPlayerDead)
                CurrentState = GameState.GameOver;
        }

        public bool DidPlayersWin() => enemies.All(e => !e.IsAlive) && players.Any(p => p.IsAlive);
        public bool DidPlayersLose() => players.Any(p => !p.IsAlive) && enemies.Any(e => e.IsAlive);

        public List<ICharacter> GetAlivePlayers() => players.Where(p => p.IsAlive).ToList();
        public List<Enemy> GetAliveEnemies() => enemies.Where(e => e.IsAlive).ToList();
    }
}
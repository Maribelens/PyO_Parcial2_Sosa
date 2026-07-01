using System.Collections;
using UnityEngine;
using RPGCombat.Characters;
using RPGCombat.Combat;
using RPGCombat.Grid;
using RPGCombat.Player;
using RPGCombat.UI;
using RPGCombat;
using System.Collections.Generic;

namespace RPGCombat 
{
    // SRP: maneja el ciclo de vida completo de la partida (loop de turnos).
    // Es el único script que conoce el orden: Init -> turno jugador 1,2,3 ->
    // turno enemigos -> repetir -> game over.
    // DIP: todas sus dependencias vienen del Inspector, ninguna se autoinstancia.
}
public class GameFlowController : MonoBehaviour
{
    [Header("Dependencias (asignar en Inspector)")]
    [SerializeField] private GameInitializer gameInitializer;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private CombatActions combatActions;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private PlayerController playerMovement;
    [SerializeField] private PlayerTurnController turnController;
    [SerializeField] private UiGame gameUI;

    private int currentPlayerTurnIndex = 0;

    private IEnumerator Start()
    {
        // El orden importa: GameInitializer ya corrió su propio Start()
        // (instancia personajes e inyecta GridManager/TurnManager/EnemyAI).
        // Acá inyectamos lo restante: PlayerController y PlayerTurnController.

        yield return null;
        playerMovement.Initialize(gridManager);
        turnController.Initialize(gridManager, combatActions, turnManager, playerMovement);
        turnController.OnTurnEnded += OnPlayerTurnEnded;

        gameInitializer.ConnectHPDisplays(gameUI.GetHPDisplays());

        StartNextPlayerTurn();
    }

    private void OnDestroy()
    {
        turnController.OnTurnEnded -= OnPlayerTurnEnded;
    }

    private void StartNextPlayerTurn()
    {
        var alivePlayers = turnManager.GetAlivePlayers();

        if (currentPlayerTurnIndex >= alivePlayers.Count)
        {
            currentPlayerTurnIndex = 0;
            StartCoroutine(RunEnemyTurn());
            return;
        }

        var nextCharacter = alivePlayers[currentPlayerTurnIndex];
        turnController.StartTurnFor(nextCharacter);
    }

    private void OnPlayerTurnEnded()
    {
        currentPlayerTurnIndex++;

        if (CheckGameOver()) return;

        StartNextPlayerTurn();
    }

    private IEnumerator RunEnemyTurn()
    {
        yield return StartCoroutine(
            enemyAI.ExecuteAllEnemyTurns(turnManager.GetAliveEnemies(), turnManager.GetAlivePlayers())
        );

        if (CheckGameOver()) yield break;

        StartNextPlayerTurn();
    }

    private bool CheckGameOver()
    {
        if (turnManager.DidPlayersWin())
        {
            gameUI.ShowGameOver(playersWon: true);
            return true;
        }

        if (turnManager.DidPlayersLose())
        {
            gameUI.ShowGameOver(playersWon: false);
            return true;
        }

        return false;
    }
}
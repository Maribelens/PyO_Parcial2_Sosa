using RPGCombat.Characters;
using RPGCombat.Grid;
using UnityEngine;

namespace RPGCombat.Player
{
    // SRP: solo traduce input de teclado en movimiento sobre la grilla.
    // No sabe nada de turnos, combate ni UI.
    public class PlayerController : MonoBehaviour
    {
        private GridManager gridManager;

        private ICharacter activeCharacter;
        private int stepsRemaining;

        public bool HasMovedThisTurn { get; private set; }

        // DIP: GridManager inyectado, no buscado con FindObjectOfType ni Singleton
        public void Initialize(GridManager grid)
        {
            gridManager = grid;
        }

        //Llamado por PlayerTurnController al empezar el turno de un personaje
        public void BeginMovementPhase(ICharacter character)
        {
            activeCharacter = character;
            stepsRemaining = character.Speed;
            HasMovedThisTurn = false;
        }

        private void Update()
        {
            if (activeCharacter == null || stepsRemaining <= 0) return;

            Vector2Int direction = ReadDirectionInput();
            if (direction == Vector2Int.zero) return;

            TryStep(direction);
        }

        private Vector2Int ReadDirectionInput()
        {
            // WASD o flechas, como pide el enunciado
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                return Vector2Int.up;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                return Vector2Int.down;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                return Vector2Int.left;
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                return Vector2Int.right;

            return Vector2Int.zero;
        }

        private void TryStep(Vector2Int direction)
        {
            Vector2Int targetPos = activeCharacter.GridPosition + direction;

            if (!gridManager.TryMove(activeCharacter, targetPos))
                return; // celda ocupada o fuera de límites: no consume paso

            stepsRemaining--;

            if (stepsRemaining <= 0)
                HasMovedThisTurn = true;
        }

        // Permite terminar de moverse antes de gastar todos los pasos
        public void EndMovementPhase()
        {
            stepsRemaining = 0;
            HasMovedThisTurn = true;
        }

        public int GetStepsRemaining() => stepsRemaining;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPGCombat.Characters;
using RPGCombat.Combat;
using RPGCombat.Grid;

namespace RPGCombat.Player
{
    public enum TurnPhase { Movement, Action, Done }

    // SRP: orquesta el turno de UN jugador (mover -> actuar -> terminar).
    // No decide el orden general de turnos (eso es TurnManager) ni ejecuta las acciones en sí (eso es CombatActions).
    // Es el puente entre input, reglas de turno y la UI.
    public class PlayerTurnController : MonoBehaviour
    {
        private GridManager gridManager;
        private CombatActions combatActions;
        private TurnManager turnManager;
        private PlayerController playerMovement;

        public TurnPhase CurrentPhase { get; private set; } = TurnPhase.Done;
        public ICharacter ActiveCharacter { get; private set; }

        // Se dispara cuando cambia de fase, para que la UI se actualice
        public Action OnPhaseChanged;
        public Action OnTurnEnded;

        // DIP: todas las dependencias inyectadas, ninguna buscada/instanciada acá
        public void Initialize(GridManager grid, CombatActions actions, TurnManager turns, PlayerController movement)
        {
            gridManager = grid;
            combatActions = actions;
            turnManager = turns;
            playerMovement = movement;
        }

        public void StartTurnFor(ICharacter character)
        {
            ActiveCharacter = character;
            CurrentPhase = TurnPhase.Movement;

            playerMovement.BeginMovementPhase(character);

            OnPhaseChanged?.Invoke();
        }


        private void Update()
        {
            if (CurrentPhase != TurnPhase.Movement) return;

            // Movimiento obligatorio: pasa a fase de acción recién cuando
            // se gastaron todos los pasos de velocidad
            if (playerMovement.HasMovedThisTurn)
            {
                CurrentPhase = TurnPhase.Action;
                OnPhaseChanged?.Invoke();
            }
        }

        // Llamado por la UI (GameUI) cuando arma los botones de acción
        public List<AvailableAction> GetAvailableActions()
        {
            var result = new List<AvailableAction>();
            if (CurrentPhase != TurnPhase.Action) return result;

            var allCharacters = GetAllLivingCharacters();

            // Melee: todos los personajes pueden atacar cuerpo a cuerpo
            var meleeTargets = allCharacters
                .Where(c => c != ActiveCharacter && ActiveCharacter.CanMeleeAttack(c))
                .ToList();
            if (meleeTargets.Count > 0)
                result.Add(new AvailableAction(ActionType.Melee, meleeTargets));

            //Rango
            if (ActiveCharacter.HasRangeAttack)
            {
                var rangeTargets = allCharacters
                    .Where(c => c != ActiveCharacter && ActiveCharacter.CanRangeAttack(c))
                    .ToList();
                if(rangeTargets.Count > 0)
                    result.Add(new AvailableAction(ActionType.Range, rangeTargets));
            }

            //Curacion
            if (ActiveCharacter.CanHeal)
            {
                var healTargets = allCharacters
                    .Where(c => ActiveCharacter.CanHealTarget(c))
                    .ToList();
                if (healTargets.Count > 0)
                    result.Add(new AvailableAction(ActionType.Heal, healTargets));
            }

            return result;
        }

        public void ExecuteAction(ActionType type, ICharacter target)
        {
            if (CurrentPhase != TurnPhase.Action) return;

            bool success = type switch
            {
                ActionType.Melee => combatActions.TryMeleeAttack(ActiveCharacter, target),
                ActionType.Range => combatActions.TryRangeAttack(ActiveCharacter, target),
                ActionType.Heal => combatActions.TryHeal(ActiveCharacter, target),
                _ => false
            };

            if (!success) return; // acción inválida: no termina el turno, deja reintentar

            EndTurn();
        }

        private void EndTurn()
        {
            CurrentPhase = TurnPhase.Done;
            turnManager.EndPlayerTurn();
            OnTurnEnded?.Invoke();
        }

        private List<ICharacter> GetAllLivingCharacters()
        {
            var characters = new List<ICharacter>();
            characters.AddRange(turnManager.GetAlivePlayers());
            characters.AddRange(turnManager.GetAliveEnemies());
            return characters;
        }
    }
}
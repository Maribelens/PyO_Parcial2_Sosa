using System;
using System.Collections.Generic;
using RPGCombat.Characters;
using UnityEngine.InputSystem;

namespace RPGCombat.Player 
{
    public enum ActionType { Melee, Range, Heal }

    // Estructura simple de datos (no es una "data class" del code smell:
    // no tiene lógica propia, solo transporta info ya calculada por CombatActions/Character)
    public readonly struct AvailableAction
    {
        public readonly ActionType Type;
        public readonly List<ICharacter> ValidTargets;

        public AvailableAction(ActionType type, List<ICharacter> validTargets) 
        {
            Type = type;
            ValidTargets = validTargets;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using RPGCombat.Characters;
using RPGCombat.Grid;
using RPGCombat.Combat;

namespace RPGCombat 
{
    // Punto de entrada: instancia personajes e inyecta todas las dependencias (DIP)
    public class GameInitializer : MonoBehaviour
    {
        [Header("Prefabs de personajes")]
        [SerializeField] private Fighter fighterPrefab;
        [SerializeField] private Healer healerPrefab;
        [SerializeField] private Ranger rangerPrefab;
        [SerializeField] private Enemy enemyPrefab;

        [Header("Componentes de la escena (asignar en Inspector)")]
        [SerializeField] private GridManager gridManager;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private CombatActions combatActions;
        [SerializeField] private EnemyAI enemyAI;

        public List<ICharacter> Players { get; private set; } = new();
        public List<Enemy> Enemies { get; private set; } = new();

        private void Start()
        {
            SpawnCharacters();
            InjectDependencies();
        }

        private void SpawnCharacters()
        {
            var positions = gridManager.GetRandomFreePositions(5);

            var fighter = Instantiate(fighterPrefab);
            var healer = Instantiate(healerPrefab);
            var ranger = Instantiate(rangerPrefab);
            var enemy1 = Instantiate(enemyPrefab);
            var enemy2 = Instantiate(enemyPrefab);

            gridManager.RegisterCharacter(fighter, positions[0]);
            gridManager.RegisterCharacter(healer, positions[1]);
            gridManager.RegisterCharacter(ranger, positions[2]);
            gridManager.RegisterCharacter(enemy1, positions[3]);
            gridManager.RegisterCharacter(enemy2, positions[4]);

            Players = new List<ICharacter> { fighter, healer, ranger };
            Enemies = new List<Enemy> { enemy1, enemy2 };
        }

        private void InjectDependencies()
        {
            turnManager.Initialize(Players, Enemies);
            enemyAI.Initialize(gridManager, combatActions, turnManager);
        }
    }
}


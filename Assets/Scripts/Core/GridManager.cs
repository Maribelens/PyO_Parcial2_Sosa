using System.Collections.Generic;
using UnityEngine;
using RPGCombat.Characters;
using RPGCombat.Combat;
using System.Linq;

namespace RPGCombat.Grid
{
    // SRP gestiona grilla. No sabe combate ni turnos
    public class GridManager : MonoBehaviour
    {
        // Nombre del hijo del prefab de celda que actúa como highLight de predicción
        private const string PredictionHighlightName = "PredictionHighlight";
        public static readonly int Rows = 4;
        public static readonly int Cols = 6;

        [Header("Prefabs")]
        [SerializeField] private GameObject cellPrefab;
        [SerializeField] private float cellSize = 1.1f;

        private GameObject[,] cells;
        private Dictionary<Vector2Int, ICharacter> occupiedCells = new();

        private void Awake() => GenerateGrid();

        private void GenerateGrid()
        {
            cells = new GameObject[Rows, Cols];
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Cols; col++)
                {
                    Vector3 pos = GridToWorld(new Vector2Int(col, row));
                    cells[row, col] = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                    cells[row, col].name = $"Cell ({col},{row})";
                }
        }

        public Vector3 GridToWorld(Vector2Int gridPos)
            => new Vector3(gridPos.x * cellSize, gridPos.y * cellSize, 0f);

        public Vector2Int WorldToGrid(Vector3 worldPos)
        {
            return new Vector2Int(
                Mathf.RoundToInt(worldPos.x / cellSize),
                Mathf.RoundToInt(worldPos.y / cellSize)
                );
        }

        public bool IsInBounds(Vector2Int pos)
        => pos.x >= 0 && pos.x < Cols && pos.y >= 0 && pos.y < Rows;

        public bool IsOccupied(Vector2Int pos) => occupiedCells.ContainsKey(pos);

        public bool TryMove(ICharacter character, Vector2Int newPos)
        {
            if (!IsInBounds(newPos) || IsOccupied(newPos)) return false;

            occupiedCells.Remove(character.GridPosition);
            character.GridPosition = newPos;
            occupiedCells[newPos] = character;

            if (character is MonoBehaviour mb)
                mb.transform.position = GridToWorld(newPos);

            return true;
        }

        public void RegisterCharacter(ICharacter character, Vector2Int pos)
        {
            character.GridPosition = pos;
            occupiedCells[pos] = character;

            if (character is MonoBehaviour mb)
                mb.transform.position = GridToWorld(pos);
        }

        public void RemoveCharacter(ICharacter character)
            => occupiedCells.Remove(character.GridPosition);

        public ICharacter GetCharacterAt(Vector2Int pos)
        {
            occupiedCells.TryGetValue(pos, out var ch);
            return ch;
        }

        public List<Vector2Int> GetRandomFreePositions(int count)
        {
            var all = new List<Vector2Int>();
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    all.Add(new Vector2Int(c, r));

            var result = new List<Vector2Int>();
            while (result.Count < count && all.Count > 0)
            {
                int idx = Random.Range(0, all.Count);
                if (!IsOccupied(all[idx])) result.Add(all[idx]);
                all.RemoveAt(idx);
            }
            return result;
        }

        // Activa el highLight de predicción en las celdas destino de cada enemigo.
        // Recibe el diccionario calculado por EnemyAI.GetPredictedPositions().
        // SRP GridManager solo muestra/oculta highlights
        public void ShowPredictedMovement(Dictionary<Enemy, Vector2Int> predictions)
        {
            HidePredictedMovement(); // limpia highlights anteriores antes de mostrar

            foreach (var kvp in predictions)
            {
                Vector2Int targetPos = kvp.Value;
                if (!IsInBounds(targetPos)) continue;

                var cell = cells[targetPos.y, targetPos.x];
                SetHighlight(cell, true);
            }
        }

        // Desactiva todos los highlights de predicción activos
        public void HidePredictedMovement()
        {
            foreach (var cell in cells)
                SetHighlight(cell, false);
        }

        // Busca el hijo PredictionHighlight de la celda y lo activa/desactiva
        private void SetHighlight(GameObject cell, bool active)
        {
            var highLight = cell.GetComponent<CellView>();
            if (highLight != null)
                highLight.SetHighlight(active);
            else
                Debug.LogWarning($"{cell.name} no tiene CellView");
        }
    }
}
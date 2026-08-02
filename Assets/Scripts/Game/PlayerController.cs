using RPGCombat.Characters;
using RPGCombat.Grid;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGCombat.Player
{
    // SRP traduce input de teclado en movimiento sobre la grilla
    // No sabe turnos, combate ni UI
    public class PlayerController : MonoBehaviour
    {
        private GridManager gridManager;

        private ICharacter activeCharacter;
        private int stepsRemaining;

        public bool HasMovedThisTurn { get; private set; }

        // DIP GridManager inyectado, no buscado con FindObjectOfType ni Singleton
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
            // Input de teclado (PC) - sin cambios
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                return Vector2Int.up;
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                return Vector2Int.down;
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                return Vector2Int.left;
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                return Vector2Int.right;

            //Input touch (Android)
#if UNITY_ANDROID
            if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) 
            {
                Vector2 touchPos = Input.GetTouch(0).position;
                return GetDirectionFromTouchPosition(touchPos);
            }
#endif

            return Vector2Int.zero;
        }

#if UNITY_ANDROID
        private bool IsTouchOverUI(Vector2 touchPos) 
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current) 
            { position = touchPos };
            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }

        private Vector2Int GetDirectionFromTouchPosition(Vector2 touchScreenPos)
        {
            if (IsTouchOverUI(touchScreenPos)) return Vector2Int.zero;

            //conversion de posicion de pantalla a posicion en el mundo
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(
                new Vector3(touchScreenPos.x, touchScreenPos.y,
                Camera.main.nearClipPlane));

            //conversion de posicion del mundo a coordenadas de grilla
            Vector2Int touchedCell = gridManager.WorldToGrid(worldPos);

            //calculo direccion relativa al personaje activo
            Vector2Int currentPos = activeCharacter.GridPosition;
            Vector2Int delta = touchedCell - currentPos;

            if (delta == Vector2Int.zero) return Vector2Int.zero;

            //normalizar a una sola direccion cardinal
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
                return new Vector2Int((int)Mathf.Sign(delta.x), 0);
            else
                return new Vector2Int(0, (int)Mathf.Sign(delta.y));
        }
#endif

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
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private List<List<TerrainType>> map;
    private List<List<GameObject>> grid;
    private Vector2Int characterPosition;
    private GameObject player;

    public void Initialize(List<List<TerrainType>> map, List<List<GameObject>> grid, Vector2Int startPosition, GameObject player)
    {
        this.map = map;
        this.grid = grid;
        this.characterPosition = startPosition;
        this.player = player;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) TryMove(Vector2Int.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryMove(Vector2Int.right);
        if (Input.GetKeyDown(KeyCode.UpArrow)) TryMove(Vector2Int.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) TryMove(Vector2Int.down);
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int newPosition = characterPosition + direction;

        if (!IsInsideBounds(newPosition)) return;
        if (IsBlocked(newPosition)) return;

        characterPosition = newPosition;

        GameObject targetCell = grid[characterPosition.y][characterPosition.x];
        GameController.PlacePlayerOnCell(player, targetCell);

        if (IsFinish(characterPosition))
            OnPlayerWin();
    }

    private bool IsInsideBounds(Vector2Int position)
    {
        return position.x >= 0
               && position.y >= 0
               && position.y < map.Count
               && position.x < map[position.y].Count;
    }

    private bool IsBlocked(Vector2Int position)
    {
        return map[position.y][position.x] == TerrainType.TREE;
    }

    private bool IsFinish(Vector2Int position)
    {
        return map[position.y][position.x] == TerrainType.FINISH;
    }

    private void OnPlayerWin()
    {
        Debug.Log("YOU WIN!!!");
    }
}

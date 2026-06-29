using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private List<GameObject> terrainPrefabs;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private MapConfigSo mapConfig;

    private const float MAP_Z_POSITION = 1f;

    private void Start()
    {
        MapBuilder mapBuilder = new MapBuilder(mapConfig);
        List<List<TerrainType>> map = mapBuilder.GenerateMap();
        Vector2Int startPosition = mapBuilder.GetStartPosition();

        List<List<GameObject>> grid = InitializeMap(map, startPosition);

        playerController.Initialize(map, grid, startPosition, player);
    }

    private List<List<GameObject>> InitializeMap(
        List<List<TerrainType>> map,
        Vector2Int startPosition)
    {
        var grid = new List<List<GameObject>>();

        for (int row = 0; row < map.Count; row++)
        {
            var gridRow = new List<GameObject>();
            for (int column = 0; column < map[row].Count; column++)
            {
                var terrainType = map[row][column];
                var gridCell = Instantiate(
                    terrainPrefabs[(int)terrainType],
                    transform
                );

                gridCell.transform.localPosition = new Vector3(
                    column * mapConfig.gridCellSize,
                    row * mapConfig.gridCellSize,
                    MAP_Z_POSITION
                );
                gridRow.Add(gridCell);
            }
            grid.Add(gridRow);
        }

        PlacePlayerOnCell(player, grid[startPosition.y][startPosition.x]);
        return grid;
    }

    public static void PlacePlayerOnCell(GameObject player, GameObject cell)
    {
        player.transform.SetParent(cell.transform);
        player.transform.localPosition = Vector3.zero;
    }
}


using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    private static readonly Vector2Int StartPosition = Vector2Int.zero;
    private readonly MapConfigSo mapConfig;

    public MapBuilder(MapConfigSo mapConfig)
    {
        this.mapConfig = mapConfig;
    }

    public virtual Vector2Int GetStartPosition()
    {
        return StartPosition;
    }

    public virtual List<List<TerrainType>> GenerateMap()
    {
        var map = new List<List<TerrainType>>();

        for (int row = 0; row < mapConfig.gridHeight; row++)
        {
            map.Add(BuildRow());
        }

        SetTerrain(map, StartPosition, TerrainType.START);
        SetTerrain(map, GetFinishPosition(), TerrainType.FINISH);

        return map;
    }

    private List<TerrainType> BuildRow()
    {
        var row = new List<TerrainType>();
        for (int column = 0; column < mapConfig.gridWidth; column++)
            row.Add(GetRandomTerrain());
        return row;
    }

    private TerrainType GetRandomTerrain()
    {
        return Random.Range(0f, 1f) <= mapConfig.obstacleProbability
            ? TerrainType.TREE
            : TerrainType.GRASS;
    }

    private Vector2Int GetFinishPosition()
    {
        return new Vector2Int(mapConfig.gridWidth - 1, mapConfig.gridHeight - 1);
    }

    private void SetTerrain(List<List<TerrainType>> map, Vector2Int position, TerrainType terrain)
    {
        map[position.y][position.x] = terrain;
    }
}

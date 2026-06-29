using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "Game/Map Config")]

public class MapConfigSo : ScriptableObject
{
    public int gridWidth = 3;
    public int gridHeight = 4;
    public float gridCellSize = 1f;
    public float obstacleProbability = 0.2f;
}

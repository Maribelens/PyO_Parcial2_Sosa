using UnityEngine;
using RPGCombat.Utils;

public class CellView : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    public void SetHighlight(bool active)
    {
        if (highlight != null)
            highlight.SetActive(active);
        else
            Log.Warning($"Highlight no asignado en {name}");
    }
}

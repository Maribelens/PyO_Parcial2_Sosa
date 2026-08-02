using UnityEngine;

public class CellView : MonoBehaviour
{
    [SerializeField] private GameObject highlight;

    public void SetHighlight(bool active)
    {
        if (highlight != null)
            highlight.SetActive(active);
        else
            Debug.LogWarning($"Highlight no asignado en {name}");
    }
}

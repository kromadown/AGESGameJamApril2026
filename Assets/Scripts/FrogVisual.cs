using UnityEngine;

public class FrogVisual : MonoBehaviour
{
    [Header("Renderer Targets")]
    public Renderer[] renderers;

    public void ApplyMaterial(Material mat)
    {
        if (mat == null) return;

        foreach (var r in renderers)
        {
            if (r != null)
                r.material = mat;
        }
    }

    public void ApplyColor(Color color)
    {
        foreach (var r in renderers)
        {
            if (r != null)
                r.material.color = color;
        }
    }
}
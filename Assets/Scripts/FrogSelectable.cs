using UnityEngine;

public class FrogSelectable : MonoBehaviour
{
    private Outline outline;

    private bool isHovered;
    private bool isSelected;

    void Awake()
    {
        outline = GetComponent<Outline>();

        if (outline != null)
            outline.enabled = false; // 🔥 FORCE OFF at start
    }

    void OnMouseEnter()
    {
        isHovered = true;
        UpdateOutline();
    }

    void OnMouseExit()
    {
        isHovered = false;
        UpdateOutline();
    }

    void OnMouseDown()
    {
        // toggle selection
        if (FrogSelect.selectedFrog == gameObject)
            FrogSelect.selectedFrog = null;
        else
            FrogSelect.selectedFrog = gameObject;

        RefreshAllFrogs();
    }

    void Update()
    {
        isSelected = FrogSelect.selectedFrog == gameObject;
        UpdateOutline();
    }

    void UpdateOutline()
    {
        if (outline == null) return;

        // priority: selected > hover > none
        if (isSelected)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green;
        }
        else if (isHovered)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.yellow;
        }
        else
        {
            outline.enabled = false;
        }
    }

    void RefreshAllFrogs()
    {
        var frogs = FindObjectsOfType<FrogSelectable>();
        foreach (var f in frogs)
            f.UpdateOutline();
    }
}
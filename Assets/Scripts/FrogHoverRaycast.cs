using UnityEngine;
using UnityEngine.InputSystem;

public class FrogHoverRaycast : MonoBehaviour
{
    public Camera cam;

    private Outline currentHover;
    private Outline selected;

    void Update()
    {
        if (cam == null) return;

        HandleHover();
        HandleClick();
    }

    void HandleHover()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Outline outline = hit.transform.GetComponentInParent<Outline>();

            if (outline != currentHover)
            {
                // turn off previous hover (but NOT if it's selected)
                if (currentHover != null && currentHover != selected)
                    currentHover.enabled = false;

                currentHover = outline;

                // turn on hover (if not already selected)
                if (currentHover != null && currentHover != selected)
                    currentHover.enabled = true;
            }
        }
        else
        {
            if (currentHover != null && currentHover != selected)
            {
                currentHover.enabled = false;
                currentHover = null;
            }
        }
    }

    void HandleClick()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 🐸 CLICKED FROG
            Outline clicked = hit.transform.GetComponentInParent<Outline>();

            if (clicked != null)
            {
                // clicking same frog → deselect
                if (selected == clicked)
                {
                    selected.enabled = false;
                    selected = null;
                    return;
                }

                // deselect previous
                if (selected != null)
                    selected.enabled = false;

                // select new
                selected = clicked;
                selected.enabled = true;

                return;
            }

            // 📦 CLICKED BREEDING BOX
            FrogBreedingBox box = hit.transform.GetComponentInParent<FrogBreedingBox>();

            if (box != null && selected != null)
            {
                box.PlaceSelectedFrog(selected.gameObject);

                selected = null;
            }
        }
        else
        {
            // clicked empty space → deselect
            if (selected != null)
            {
                selected.enabled = false;
                selected = null;
            }
        }
    }
}
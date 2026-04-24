using UnityEngine;
using TMPro;

public class FrogHoverRaycast : MonoBehaviour
{
    public Camera cam;

    private FrogHoverUI currentHover;

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(MousePosition());

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            FrogHoverUI hover = hit.collider.GetComponentInParent<FrogHoverUI>();

            if (hover != currentHover)
            {
                if (currentHover != null)
                    currentHover.HideUI();

                currentHover = hover;

                if (currentHover != null)
                    currentHover.ShowUI();
            }
        }
        else
        {
            if (currentHover != null)
            {
                currentHover.HideUI();
                currentHover = null;
            }
        }
    }

    Vector2 MousePosition()
    {
        return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
    }
}
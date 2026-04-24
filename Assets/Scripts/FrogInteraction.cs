using UnityEngine;
using UnityEngine.InputSystem;

public class FrogInteraction : MonoBehaviour
{
    public Camera cam;

    void Update()
    {
        if (!FoodManager.Instance.isHoldingFood) return;

        // New Input System mouse click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                FrogFeeding frog = hit.collider.GetComponent<FrogFeeding>();

                if (frog != null && frog.CanBeFed())
                {
                    frog.Feed();
                    FoodManager.Instance.ClearFood();
                }
            }
        }
    }
}
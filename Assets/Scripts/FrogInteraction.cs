using UnityEngine;
using UnityEngine.InputSystem;

public class FrogInteraction : MonoBehaviour
{
    public Camera cam;

    void Update()
    {
        if (FoodManager.Instance == null || !FoodManager.Instance.isHoldingFood)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                FrogFeeding frog = hit.collider.GetComponentInParent<FrogFeeding>();

                if (frog != null && frog.CanBeFed())
                {
                    var manager = FoodManager.Instance;

                    FrogType foodType = manager.selectedFoodType;

                    Debug.Log($"[Interaction] Feeding {frog.name} with {foodType}");

                    // ✅ unified feeding
                    frog.Feed(foodType);

                    // ✅ consume AFTER feeding
                    manager.ConsumeSelectedFood();

                    // ❗ DO NOT ClearFood here
                    // FoodManager already handles:
                    // - depletion
                    // - toggle behavior
                }
            }
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class FrogHoverRaycast : MonoBehaviour
{
    public Camera cam;

    private Outline currentHover;
    private Outline selected;

    private FrogHoverUI currentUI;

    void Update()
    {
        HandleHover();
        HandleClick();
    }

    void HandleHover()
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        Outline newHover = null;
        FrogHoverUI newUI = null;
        FrogFeeding feeding = null;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            newHover = hit.transform.GetComponentInParent<Outline>();
            newUI = hit.transform.GetComponentInParent<FrogHoverUI>();
            feeding = hit.transform.GetComponentInParent<FrogFeeding>();
        }

        bool isHoldingFood = FoodManager.Instance != null && FoodManager.Instance.isHoldingFood;

        // =========================
        // 🎯 OUTLINE RULES
        // =========================
        if (newHover != null)
        {
            if (!isHoldingFood)
            {
                // 🟢 BREEDING MODE → only show if ready
                if (feeding != null && !feeding.IsReadyToMate())
                    newHover = null;
            }
            // 🟡 FOOD MODE → always allow hover outline
        }

        // =========================
        // APPLY OUTLINE
        // =========================
        if (newHover != currentHover)
        {
            if (currentHover != null && currentHover != selected)
                currentHover.enabled = false;

            currentHover = newHover;

            if (currentHover != null && currentHover != selected)
                currentHover.enabled = true;
        }

        // =========================
        // UI LOGIC (unchanged)
        // =========================
        if (newUI != currentUI)
        {
            if (currentUI != null)
                currentUI.HideUI();

            currentUI = newUI;

            if (currentUI != null && isHoldingFood)
                currentUI.ShowUI();
        }
        else
        {
            if (currentUI != null)
            {
                if (isHoldingFood)
                    currentUI.ShowUI();
                else
                    currentUI.HideUI();
            }
        }

        if (newUI == null && currentUI != null)
        {
            currentUI.HideUI();
            currentUI = null;
        }
    }

    void HandleClick()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        Outline clicked = hit.transform.GetComponentInParent<Outline>();
        FrogFeeding feeding = hit.transform.GetComponentInParent<FrogFeeding>();
        FrogBreedingBox box = hit.transform.GetComponent<FrogBreedingBox>();

        // =========================
        // 📦 BREEDING BOX FIRST
        // =========================
        if (box != null && selected != null)
        {
            // 🚫 block if frog is not ready
            FrogFeeding selectedFeeding = selected.GetComponent<FrogFeeding>();

            if (selectedFeeding != null && !selectedFeeding.IsReadyToMate())
            {
                Debug.Log("Frog is not ready to mate yet!");
                return;
            }

            box.PlaceSelectedFrog(selected.gameObject);

            selected.enabled = false;
            selected = null;
            return;
        }

        // =========================
        // 🍖 FEEDING MODE
        // =========================
        if (FoodManager.Instance.isHoldingFood)
        {
            if (feeding != null)
            {
                feeding.Feed();
                FoodManager.Instance.ClearFood();
            }
            return;
        }

        // =========================
        // 🐸 SELECTION MODE
        // =========================
        if (clicked != null)
        {
            FrogFeeding clickedFeeding = clicked.GetComponent<FrogFeeding>();

            // 🚫 BLOCK SELECTION IF NOT FULLY FED
            if (clickedFeeding != null && !clickedFeeding.IsReadyToMate())
            {
                Debug.Log("This frog needs more feeding before it can be selected!");
                return;
            }

            // toggle deselect
            if (selected == clicked)
            {
                selected.enabled = false;
                selected = null;
                return;
            }

            if (selected != null)
                selected.enabled = false;

            selected = clicked;
            selected.enabled = true;

            return;
        }

        // empty click
        if (selected != null)
        {
            selected.enabled = false;
            selected = null;
        }
    }

    // 🔥 RESTORED (needed by FoodManager)
    public void RefreshHoverUI()
    {
        if (currentUI == null) return;

        if (FoodManager.Instance.isHoldingFood)
            currentUI.ShowUI();
        else
            currentUI.HideUI();
    }
}
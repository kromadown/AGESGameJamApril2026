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

        // 🎯 OUTLINE RULES
        if (newHover != null)
        {
            if (!isHoldingFood)
            {
                if (feeding != null && !feeding.IsReadyToMate())
                    newHover = null;
            }
        }

        // APPLY OUTLINE
        if (newHover != currentHover)
        {
            if (currentHover != null && currentHover != selected)
                currentHover.enabled = false;

            currentHover = newHover;

            if (currentHover != null && currentHover != selected)
                currentHover.enabled = true;
        }

        // UI LOGIC
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

        // =========================
        // 🎁 GIFT BOX (TOP PRIORITY)
        // =========================
        FrogGiftBox gift = hit.transform.GetComponentInParent<FrogGiftBox>();

        if (gift != null)
        {
            Debug.Log("Gift clicked");
            gift.Open();
            return;
        }

        // =========================
        // COMPONENTS
        // =========================
        Outline clicked = hit.transform.GetComponentInParent<Outline>();
        FrogFeeding feeding = hit.transform.GetComponentInParent<FrogFeeding>();
        FrogBreedingBox box = hit.transform.GetComponentInParent<FrogBreedingBox>();
        FrogIdentity frog = hit.transform.GetComponentInParent<FrogIdentity>();

        // =========================
        // 📦 BREEDING BOX
        // =========================
        if (box != null && selected != null)
        {
            FrogFeeding selectedFeeding = selected.GetComponent<FrogFeeding>();

            if (selectedFeeding != null && !selectedFeeding.IsReadyToMate())
            {
                Debug.Log("Frog not ready");
                return;
            }

            box.PlaceSelectedFrog(selected.gameObject);

            selected.enabled = false;
            selected = null;
            return;
        }

        // =========================
        // 🍖 FEEDING MODE (UPDATED)
        // =========================
        if (FoodManager.Instance != null && FoodManager.Instance.isHoldingFood)
        {
            if (feeding != null)
            {
                // 🐸 feed frog
                feeding.Feed();

                // 🍖 consume actual food (THIS IS THE FIX)
                FoodManager.Instance.ConsumeSelectedFood();

                // 🎁 gift check
                if (feeding.GetFeedCount() >= 6)
                {
                    FrogGiftSystem system = FindFirstObjectByType<FrogGiftSystem>();

                    if (system != null && frog != null)
                    {
                        system.GiveGift(
                            frog.GetComponent<FrogIdentity>(),
                            frog.transform.position
                        );
                    }

                    feeding.ResetFeed();
                }

                return;
            }
        }

        // =========================
        // 🐸 SELECTION MODE
        // =========================
        if (clicked != null)
        {
            FrogFeeding clickedFeeding = clicked.GetComponent<FrogFeeding>();

            if (clickedFeeding != null && !clickedFeeding.IsReadyToMate())
            {
                Debug.Log("Not ready yet");
                return;
            }

            // toggle select
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

        // =========================
        // EMPTY CLICK
        // =========================
        if (selected != null)
        {
            selected.enabled = false;
            selected = null;
        }
    }

    public void RefreshHoverUI()
    {
        if (currentUI == null) return;

        if (FoodManager.Instance != null && FoodManager.Instance.isHoldingFood)
            currentUI.ShowUI();
        else
            currentUI.HideUI();
    }
}
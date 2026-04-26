using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodButtonUI : MonoBehaviour
{
    public FrogType type;
    public Button button;
    public TextMeshProUGUI countText;

    void Update()
    {
        var system = FindFirstObjectByType<FrogGiftSystem>();
        if (system == null) return;

        int count = system.GetFoodCount(type);
        int max = system.maxSpecialFood; // ✅ get limit

        // Enable / disable button
        button.interactable = count > 0;

        // Update text → "current / max"
        if (countText != null)
        {
            countText.text = $"{count}/{max}";
        }
    }
}
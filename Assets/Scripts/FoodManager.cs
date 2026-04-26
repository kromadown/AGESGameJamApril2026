using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance;

    public bool isHoldingFood = false;
    public FrogType selectedFoodType;

    // ✅ ADD THIS (was missing)
    public enum FoodMode
    {
        Normal,
        Special
    }

    public FoodMode currentMode;

    [Header("Cursor Settings")]
    public Texture2D normalCursor;
    public Texture2D foodCursor;

    public Vector2 hotspot = Vector2.zero;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetNormalCursor();
    }

    // =========================
    // 🌟 SPECIAL FOOD SELECTION
    // =========================
    public void SelectFood(FrogType type)
    {
        var system = FindFirstObjectByType<FrogGiftSystem>();

        if (system == null || system.GetFoodCount(type) <= 0)
        {
            Debug.Log("No food of this type!");
            return;
        }

        selectedFoodType = type;
        currentMode = FoodMode.Special;
        isHoldingFood = true;

        SetFoodCursor();
        FindFirstObjectByType<FrogHoverRaycast>()?.RefreshHoverUI();

        Debug.Log("Special food selected: " + type);
    }

    // =========================
    // 🍖 UI WRAPPERS
    // =========================
    public void SelectFoodA() => SelectFood(FrogType.A);
    public void SelectFoodB() => SelectFood(FrogType.B);
    public void SelectFoodC() => SelectFood(FrogType.C);

    // =========================
    // 🍖 NORMAL FOOD
    // =========================
    public void SelectNormalFood()
    {
        currentMode = FoodMode.Normal;
        isHoldingFood = true;

        selectedFoodType = FrogType.None; // IMPORTANT FIX

        SetFoodCursor();
        FindFirstObjectByType<FrogHoverRaycast>()?.RefreshHoverUI();

        Debug.Log("Normal food selected");
    }

    // =========================
    // 🍽 CONSUME FOOD AFTER FEED
    // =========================
    public void ConsumeSelectedFood()
    {
        var system = FindFirstObjectByType<FrogGiftSystem>();

        if (system == null)
            return;

        if (currentMode == FoodMode.Special && selectedFoodType != FrogType.None)
        {
            bool used = system.UseFood(selectedFoodType);

            if (!used)
                Debug.Log("Failed to consume special food");
        }
        else
        {
            Debug.Log("Normal food used (no inventory consumed)");
        }

        ClearFood();
    }

    // =========================
    // ❌ CLEAR STATE
    // =========================
    public void ClearFood()
    {
        isHoldingFood = false;
        selectedFoodType = FrogType.None;
        currentMode = FoodMode.Normal;

        SetNormalCursor();
        FindFirstObjectByType<FrogHoverRaycast>()?.RefreshHoverUI();
    }

    // =========================
    // 🎯 CURSOR
    // =========================
    void SetFoodCursor()
    {
        Cursor.SetCursor(foodCursor, hotspot, CursorMode.Auto);
    }

    void SetNormalCursor()
    {
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }
}
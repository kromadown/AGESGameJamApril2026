using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance;

    public bool isHoldingFood = false;
    public FrogType selectedFoodType;

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
    // 🌟 SPECIAL FOOD SELECTION (CORE)
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
        isHoldingFood = true;

        SetFoodCursor();
        FindFirstObjectByType<FrogHoverRaycast>()?.RefreshHoverUI();

        Debug.Log("Special food selected: " + type);
    }

    // =========================
    // 🍖 UI WRAPPERS (UNITY BUTTON FIX)
    // =========================
    public void SelectFoodA()
    {
        SelectFood(FrogType.A);
    }

    public void SelectFoodB()
    {
        SelectFood(FrogType.B);
    }

    public void SelectFoodC()
    {
        SelectFood(FrogType.C);
    }

    // =========================
    // 🍖 NORMAL FOOD
    // =========================
    public void SelectNormalFood()
    {
        selectedFoodType = FrogType.A; // placeholder (ignored in special logic)
        isHoldingFood = true;

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

        if (system != null)
        {
            bool used = system.UseFood(selectedFoodType);

            if (!used)
                Debug.Log("Failed to consume food");
        }

        ClearFood();
    }

    // =========================
    // ❌ CLEAR STATE
    // =========================
    public void ClearFood()
    {
        isHoldingFood = false;
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
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance;

    public bool isHoldingFood = false;

    public Texture2D normalCursor;
    public Texture2D foodCursor;

    public Vector2 hotspot = Vector2.zero;

    private FrogHoverRaycast hoverRaycast;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        hoverRaycast = FindFirstObjectByType<FrogHoverRaycast>();
        SetNormalCursor();
    }

    public void SelectFood()
    {
        isHoldingFood = true;
        SetFoodCursor();

        hoverRaycast?.RefreshHoverUI();
    }

    public void ClearFood()
    {
        isHoldingFood = false;
        SetNormalCursor();

        hoverRaycast?.RefreshHoverUI();
    }

    void SetFoodCursor()
    {
        Cursor.SetCursor(foodCursor, hotspot, CursorMode.Auto);
    }

    void SetNormalCursor()
    {
        Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
    }
}
using UnityEngine;

public class FoodManager : MonoBehaviour
{
    public static FoodManager Instance;

    public bool isHoldingFood = false;

    [Header("Cursor Settings")]
    public Texture2D normalCursor;
    public Texture2D foodCursor;

    [Header("Cursor Hotspot (click point)")]
    public Vector2 hotspot = Vector2.zero;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetNormalCursor();
    }

    public void SelectFood()
    {
        isHoldingFood = true;
        Debug.Log("FOOD SELECTED");
        SetFoodCursor();
    }

    public void ClearFood()
    {
        isHoldingFood = false;
        SetNormalCursor();
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
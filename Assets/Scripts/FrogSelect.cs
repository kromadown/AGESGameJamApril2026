using UnityEngine;

public class FrogSelect : MonoBehaviour
{
    public static GameObject selectedFrog;

    void OnMouseDown()
    {
        selectedFrog = gameObject;
        Debug.Log("Selected: " + gameObject.name);
    }
}
using UnityEngine;
using TMPro;

public class FrogHoverUI : MonoBehaviour
{
    public GameObject canvas;
    public TextMeshProUGUI hoverText;

    FrogFeeding feeding;
    Camera cam;

    void Start()
    {
        feeding = GetComponent<FrogFeeding>();
        cam = Camera.main;
        canvas.SetActive(false);
    }

    public void ShowUI()
    {
        if (FoodManager.Instance == null || !FoodManager.Instance.isHoldingFood)
            {
                canvas.SetActive(false);
                return;
            }

        canvas.SetActive(true);
        UpdateText();
    }

    public void HideUI()
    {
        canvas.SetActive(false);
    }

    public void Refresh()
    {
        if (canvas.activeSelf)
            UpdateText();
    }

    void Update()
    {
        if (!canvas.activeSelf || cam == null) return;

        canvas.transform.LookAt(
            canvas.transform.position + cam.transform.rotation * Vector3.forward,
            cam.transform.rotation * Vector3.up
        );
    }

    void UpdateText()
    {
        if (feeding.CanBeFed())
            hoverText.text = $"Feed ({feeding.GetFeedCount()}/{feeding.maxFeed})";
        else
            hoverText.text = "Full!";
    }
}
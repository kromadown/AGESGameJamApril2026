using UnityEngine;
using TMPro;

public class FrogHoverUI : MonoBehaviour
{
    public GameObject canvas;
    public TextMeshProUGUI hoverText;

    private FrogFeeding feeding;
    private Camera cam;

    void Start()
    {
        feeding = GetComponent<FrogFeeding>();
        cam = Camera.main;

        canvas.SetActive(false);
    }

    public void ShowUI()
    {
        if (!FoodManager.Instance.isHoldingFood) return;

        canvas.SetActive(true);
        UpdateText();
    }

    public void HideUI()
    {
        canvas.SetActive(false);
    }

    void Update()
    {
        FaceCamera();
    }

    void FaceCamera()
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
            hoverText.text = "Feed (" + feeding.GetFeedCount() + "/3)";
        else
            hoverText.text = "Full!";
    }
}
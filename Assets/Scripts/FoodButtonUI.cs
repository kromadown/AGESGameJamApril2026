using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodButtonUI : MonoBehaviour
{
    public FrogType type;
    public Button button;
    public TextMeshProUGUI countText;
    public Animator uIAnimA;
    public Animator uIAnimB;
    public Animator uIAnimC;
    public AudioSource specialFoodGet;

    void Update()
    {
        var system = FindFirstObjectByType<FrogGiftSystem>();
        if (system == null) return;

        int count = system.GetFoodCount(type);
        int max = system.maxSpecialFood; // ✅ get limit

        if (system.getSpecialFoodA & uIAnimA != null)
        {
            uIAnimA.SetTrigger("foodUiAnim");
            specialFoodGet.pitch = Random.Range(0.9f, 1.1f);
            specialFoodGet.PlayOneShot(specialFoodGet.clip);
            system.getSpecialFoodA = false;
        }

        if (system.getSpecialFoodB & uIAnimB != null)
        {
            uIAnimB.SetTrigger("foodUiAnim");
            specialFoodGet.pitch = Random.Range(0.9f, 1.1f);
            specialFoodGet.PlayOneShot(specialFoodGet.clip);
            system.getSpecialFoodB = false;
        }

        if (system.getSpecialFoodC & uIAnimC != null)
        {
            uIAnimC.SetTrigger("foodUiAnim");
            specialFoodGet.pitch = Random.Range(0.9f, 1.1f);
            specialFoodGet.PlayOneShot(specialFoodGet.clip);
            system.getSpecialFoodC = false;
        }

        // Enable / disable button
        button.interactable = count > 0;

        // Update text → "current / max"
        if (countText != null)
        {
            countText.text = $"{count}/{max}";
        }

    }
}
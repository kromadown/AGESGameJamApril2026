using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    public AudioSource audioSource;

    void Start()
    {
        Button btn = GetComponent<Button>();

        btn.onClick.AddListener(() =>
        {
            if (audioSource != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(audioSource.clip);
            }
        });
    }
}
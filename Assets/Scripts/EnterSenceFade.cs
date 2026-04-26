using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class EnterSceneFade : MonoBehaviour
{
    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    void Start()
    {
        SetFadeAlpha(1f);

        if (fadePanel != null)
        {
            fadePanel.raycastTarget = true;
        }

        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            SetFadeAlpha(1f - t);

            yield return null;
        }

        SetFadeAlpha(0f);

        if (fadePanel != null)
        {
            fadePanel.raycastTarget = false;
        }
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadePanel == null) return;

        Color color = fadePanel.color;
        color.a = alpha;
        fadePanel.color = color;
    }
}
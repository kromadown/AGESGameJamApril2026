using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class OpenningUI : MonoBehaviour
{
    public Button start;
    public Button quit;
    public Animator cameraMovement;

    [Header("Scene")]
    public string sceneName = "GameScene";

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    void Start()
    {
        start.onClick.AddListener(StartGame);
        quit.onClick.AddListener(QuitGame);

        SetFadeAlpha(0f);
    }

    void StartGame()
    {
        start.interactable = false;
        quit.interactable = false;

        cameraMovement.SetBool("Started", true);

        StartCoroutine(StartGameSequence());
    }

    IEnumerator StartGameSequence()
    {
        if (cameraMovement != null)
        {
            cameraMovement.SetTrigger("Start");
        }

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            SetFadeAlpha(t);

            yield return null;
        }

        SetFadeAlpha(1f);
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadePanel == null) return;

        Color color = fadePanel.color;
        color.a = alpha;
        fadePanel.color = color;
    }

    void QuitGame()
    {
        UnityEngine.Application.Quit();
    }
}
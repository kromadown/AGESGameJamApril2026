using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Transform center;
    public Button turnRight;
    public Button turnLeft;

    public Button pause;
    public Button resume;
    public Button mainMenu;

    public Button tutorialCheck;
    public Image tutorial;
    public Image mateTutorial;
    public Button mateTutotialCheck;

    public Button food;

    public Animator pauseMenu;

    public float rotateDuration = 0.3f;

    private bool isRotating = false;

    [Header("Scene")]
    public string sceneName = "GameScene";

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 1f;

    void Start()
    {
        tutorialCheck.onClick.AddListener(TutorialCheck);

        resume.interactable = false;
        mainMenu.interactable = false;

        mateTutorial.gameObject.SetActive(false);

        food.gameObject.SetActive(false);
    }

    void TutorialCheck()
    {
        tutorial.gameObject.SetActive(false);
        mateTutorial.gameObject.SetActive(true);
        mateTutotialCheck.onClick.AddListener(MateTutorialCheck);
    }

    void MateTutorialCheck()
    {
        mateTutorial.gameObject.SetActive(false);
        turnRight.onClick.AddListener(() => Rotate(-90));
        turnLeft.onClick.AddListener(() => Rotate(90));
        pause.onClick.AddListener(Pause);
        resume.onClick.AddListener(Resume);
        mainMenu.onClick.AddListener(MainMenu);

        food.gameObject.SetActive(true);
    }

    void Pause()
    {
        turnRight.interactable = false;
        turnLeft.interactable = false;

        resume.interactable = true;
        mainMenu.interactable = true;

        pauseMenu.SetBool("Paused", true);

        Time.timeScale = 0f;
    }

    void Resume()
    {
        turnRight.interactable = true;
        turnLeft.interactable = true;

        resume.interactable = false;
        mainMenu.interactable = false;

        pauseMenu.SetBool("Paused", false);
        pauseMenu.SetTrigger("Resumed");

        Time.timeScale = 1f;
    }

    void MainMenu()
    {
        resume.interactable = false;
        mainMenu.interactable = false;
        turnRight.interactable = false;
        turnLeft.interactable = false;
        pause.interactable = false;

        StartCoroutine(StartGameSequence());
    }

    IEnumerator StartGameSequence()
    {
        yield return StartCoroutine(FadeOut());

        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeOut()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
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

    void Rotate(float angle)
    {
        Debug.Log("Button clicked, angle = " + angle);

        if (!isRotating)
        {
            StartCoroutine(RotateSmooth(angle));
        }
    }

    IEnumerator RotateSmooth(float angle)
    {
        isRotating = true;

        Quaternion startRot = center.rotation;

        // 如果你要按照 Global Y 轴旋转，用这一行
        Quaternion endRot = Quaternion.Euler(0, angle, 0) * startRot;

        float time = 0;

        while (time < rotateDuration)
        {
            time += Time.deltaTime;
            float t = time / rotateDuration;

            center.rotation = Quaternion.Lerp(startRot, endRot, t);

            yield return null;
        }

        center.rotation = endRot;
        isRotating = false;
    }
}
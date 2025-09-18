using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Level1Manager : MonoBehaviour
{
    [Header("Tiempo del nivel")]
    [SerializeField, Min(1f)] float levelTimeSeconds = 150f;

    [Header("Jugador")]
    [SerializeField] PlayerHealth playerHealth;

    [Header("UI: Timer (TMP arriba-centro)")]
    [SerializeField] TMP_Text timerText;

    [Header("UI: Intro al iniciar")]
    [SerializeField] CanvasGroup introPanel;
    [SerializeField, Min(0.1f)] float introDurationSeconds = 5f;

    [Header("UI: Pausa")]
    [SerializeField] CanvasGroup pausePanel;
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;

    [Header("Escenas")]
    [SerializeField] string menuSceneName = "Menu Principal";

    float _timeLeft;
    int _chestsOpened;
    bool _isGameOver;
    bool _introActive;
    bool _pauseActive;

    void Awake()
    {
        _timeLeft = levelTimeSeconds;
        if (!playerHealth) playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
        SetGroupVisible(introPanel, false, true);
        SetGroupVisible(pausePanel, false, true);
    }

    void OnEnable()
    {
        ChestController.OnAnyChestOpened += OnChestOpened;
        if (playerHealth) playerHealth.OnDied += OnPlayerDied;
        StartCoroutine(IntroRoutine());
    }

    void OnDisable()
    {
        ChestController.OnAnyChestOpened -= OnChestOpened;
        if (playerHealth) playerHealth.OnDied -= OnPlayerDied;
    }

    void Update()
    {
        if (!_introActive)
        {
            bool externalFreeze = Time.timeScale == 0f && !_pauseActive;
            if (Input.GetKeyDown(pauseKey) && !_isGameOver && !externalFreeze)
                TogglePause();
        }

        if (_isGameOver) return;

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;

            if (_chestsOpened <= 0)
            {
                _isGameOver = true;
                GoToMenu();
            }
        }

        if (timerText) timerText.text = FormatTime(_timeLeft);
    }

    void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    System.Collections.IEnumerator IntroRoutine()
    {
        _introActive = true;
        float prev = Time.timeScale;
        Time.timeScale = 0f;
        SetGroupVisible(introPanel, true);
        yield return new WaitForSecondsRealtime(introDurationSeconds);
        SetGroupVisible(introPanel, false);
        Time.timeScale = prev <= 0f ? 1f : prev;
        _introActive = false;
    }

    void TogglePause()
    {
        if (_pauseActive) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        if (_pauseActive) return;
        _pauseActive = true;
        Time.timeScale = 0f;
        SetGroupVisible(pausePanel, true);
    }

    public void ResumeGame()
    {
        if (!_pauseActive) return;
        _pauseActive = false;
        SetGroupVisible(pausePanel, false);
        Time.timeScale = 1f;
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    void OnChestOpened(ChestController chest)
    {
        if (_isGameOver) return;
        _chestsOpened++;
    }

    void OnPlayerDied()
    {
        if (_isGameOver) return;
        _isGameOver = true;
    }

    static void SetGroupVisible(CanvasGroup g, bool visible, bool immediate = false)
    {
        if (!g) return;
        g.alpha = visible ? 1f : 0f;
        g.blocksRaycasts = visible;
        g.interactable = visible;
    }

    static string FormatTime(float seconds)
    {
        if (seconds < 0f) seconds = 0f;
        int total = Mathf.CeilToInt(seconds);
        int m = total / 60;
        int s = total % 60;
        return $"{m:00}:{s:00}";
    }
}

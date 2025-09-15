using UnityEngine;
using UnityEngine.SceneManagement;

public class Level1Manager : MonoBehaviour
{
    [Header("Tiempo del nivel")]
    [SerializeField, Min(1f)] float levelTimeSeconds = 150f;

    [Header("Escenas")]
    private string mainMenuSceneName = "Menu Principal"; // nombre exacto de tu menú

    [Header("UI (opcional)")]
    [SerializeField] ScreenFader fader; // si está asignado, usa fade

    [Header("Jugador")]
    [SerializeField] PlayerHealth playerHealth; // si no lo asignás, se busca en escena

    float _timeLeft;
    int _chestsOpened = 0;
    bool _hasWon = false;
    bool _isGameOver = false;

    public float TimeLeft => Mathf.Max(0f, _timeLeft);
    public bool HasWon => _hasWon;
    public bool IsGameOver => _isGameOver;
    public int ChestsOpened => _chestsOpened;

    void Awake()
    {
        _timeLeft = levelTimeSeconds;
        if (!playerHealth) playerHealth = Object.FindFirstObjectByType<PlayerHealth>();
    }

    void OnEnable()
    {
        ChestController.OnAnyChestOpened += OnChestOpened;
        if (playerHealth) playerHealth.OnDied += OnPlayerDied;
    }

    void OnDisable()
    {
        ChestController.OnAnyChestOpened -= OnChestOpened;
        if (playerHealth) playerHealth.OnDied -= OnPlayerDied;
    }

    void Update()
    {
        if (_isGameOver || _hasWon) return;

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;

            if (_chestsOpened <= 0)
            {
                Lose("Tiempo agotado sin abrir cofres");
            }
            else
            {
                Win("Se abrió al menos un cofre dentro del tiempo");
            }
        }
    }

    void OnChestOpened(ChestController chest)
    {
        if (_isGameOver || _hasWon) return;

        _chestsOpened++;

        if (_timeLeft > 0f && _chestsOpened >= 1)
        {
            Win("Se abrió al menos un cofre dentro del tiempo");
        }
    }

    void OnPlayerDied()
    {
        if (_isGameOver || _hasWon) return;
        Lose("El jugador se quedó sin vida");
    }

    // ===== Resultados =====

    void Win(string reason)
    {
        if (_hasWon || _isGameOver) return;
        _hasWon = true;
        Debug.Log("[Level1] Victoria: " + reason);
        LoadMenu();
    }

    void Lose(string reason)
    {
        if (_isGameOver || _hasWon) return;
        _isGameOver = true;
        Debug.Log("[Level1] Derrota: " + reason);
        LoadMenu();
    }

    void LoadMenu()
    {
        if (fader != null) fader.FadeOutThenLoad(mainMenuSceneName);
        else SceneManager.LoadScene(mainMenuSceneName);
    }
}

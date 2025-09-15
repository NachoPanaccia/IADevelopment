using UnityEngine;

public class Level1Manager : MonoBehaviour
{
    [Header("Tiempo del nivel")]
    [SerializeField, Min(1f)] float levelTimeSeconds = 150f;

    [Header("Jugador")]
    [SerializeField] PlayerHealth playerHealth; // si no se asigna, se busca en escena

    float _timeLeft;
    int _chestsOpened = 0;
    bool _isGameOver = false; // derrota

    public float TimeLeft => Mathf.Max(0f, _timeLeft);
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
        if (_isGameOver) return;

        _timeLeft -= Time.deltaTime;
        if (_timeLeft <= 0f)
        {
            _timeLeft = 0f;

            if (_chestsOpened <= 0)
            {
                _isGameOver = true;
                Debug.Log("Perdiste"); // <- pedido
            }
            else
            {
                // Victoria por abrir cofre antes del 0. La UI de recompensa ya apareció por evento.
                Debug.Log("Ganaste");  // <- pedido
            }
        }
    }

    void OnChestOpened(ChestController chest)
    {
        if (_isGameOver) return;
        _chestsOpened++;

        // Si querés loguear el éxito en el mismo momento (no hace daño):
        if (_timeLeft > 0f && _chestsOpened == 1)
            Debug.Log("Ganaste");
    }

    void OnPlayerDied()
    {
        if (_isGameOver) return;
        _isGameOver = true;
        Debug.Log("Perdiste"); // <- pedido
    }
}

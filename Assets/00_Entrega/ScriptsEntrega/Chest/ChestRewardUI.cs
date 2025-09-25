using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// UI modal que aparece al abrir un cofre:
/// - Muestra el nombre del premio y colorea según rareza.
/// - Congela el juego (Time.timeScale = 0) y desbloquea el cursor.
/// - Botones: “Volver a jugar” (01_Nivel) y “Menú principal”.
public class ChestRewardUI : MonoBehaviour
{
    [Header("Referencias UI (en el PANEL)")]
    [SerializeField] private CanvasGroup group;  // CanvasGroup del panel modal
    [SerializeField] private TMP_Text titleText; // Título (Felicidades / vacío)
    [SerializeField] private TMP_Text bodyText;  // Texto con el nombre del ítem
    [SerializeField] private Button replayButton;
    [SerializeField] private Button menuButton;

    [Header("Escenas")]
    [SerializeField] private string levelSceneName = "01_Nivel";
    [SerializeField] private string menuSceneName = "Menu Principal";

    [Header("Cursor")]
    [SerializeField] private bool manageCursor = true; // desbloquear/ocultar automáticamente

    private CursorLockMode _prevLock;
    private bool _prevVisible;
    private float _prevTimeScale = 1f;
    private bool _shown;

    private void Awake()
    {
        if (!group)
        {
            group = GetComponent<CanvasGroup>();
        }
        SetVisible(false, true);

        if (replayButton)
        {
            replayButton.onClick.AddListener(OnReplay);
        }
        if (menuButton)
        {
            menuButton.onClick.AddListener(OnMenu);
        }
    }

    private void OnEnable()
    {
        ChestPressedLogic.OnRewardRolled += OnRewardRolled;
    }

    private void OnDisable()
    {
        ChestPressedLogic.OnRewardRolled -= OnRewardRolled;
    }

    /// Callback del evento global de loot: actualiza textos/colores y muestra el panel.
    private void OnRewardRolled(ChestDropDB.DropDef item, ChestPressedLogic.Rarity rarity)
    {
        if (_shown)
        {
            return;
        }

        if (rarity == ChestPressedLogic.Rarity.Nada)
        {
            if (titleText)
            {
                titleText.text = "¡Haz encontrado el cofre! pero...";
            }
            if (bodyText)
            {
                bodyText.text = "Lamentablemente estaba vacío, ¡mala suerte!";
                bodyText.color = new Color(0.8f, 0.8f, 0.8f);
            }
        }
        else
        {
            if (titleText)
            {
                titleText.text = "¡Felicidades!";
            }
            if (bodyText)
            {
                bodyText.text = $"Has obtenido: {item.name}";
                bodyText.color = GetColorFor(rarity);
            }
        }

        Show();
    }

    private void Show()
    {
        _shown = true;

        if (manageCursor)
        {
            _prevLock = Cursor.lockState;
            _prevVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        _prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        SetVisible(true);
    }

    private void SetVisible(bool visible, bool immediate = false)
    {
        if (!group)
        {
            gameObject.SetActive(visible);
            return;
        }

        group.blocksRaycasts = visible;
        group.interactable = visible;
        group.alpha = visible ? 1f : 0f;
    }

    private void OnReplay()
    {
        RestoreCursor();
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSceneName);
    }

    private void OnMenu()
    {
        RestoreCursor();
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    private void RestoreCursor()
    {
        if (!manageCursor)
        {
            return;
        }

        Cursor.lockState = _prevLock;
        Cursor.visible = _prevVisible;
    }

    private Color GetColorFor(ChestPressedLogic.Rarity r)
    {
        switch (r)
        {
            case ChestPressedLogic.Rarity.Normal:
                {
                    return Color.white;           // Blanco
                }
            case ChestPressedLogic.Rarity.Rara:
                {
                    return Hex("#32CD32");       // Verde Lima
                }
            case ChestPressedLogic.Rarity.Epica:
                {
                    return Hex("#A020F0");       // Púrpura
                }
            case ChestPressedLogic.Rarity.Legendaria:
                {
                    return Hex("#FFD700");       // Dorado
                }
            default:
                {
                    return new Color(0.8f, 0.8f, 0.8f);
                }
        }
    }

    private static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }
}
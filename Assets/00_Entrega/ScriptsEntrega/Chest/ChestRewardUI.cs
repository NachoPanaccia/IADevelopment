using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using static ChestPressedLogic;

public class ChestRewardUI : MonoBehaviour
{
    [Header("Referencias UI (en el PANEL)")]
    [SerializeField] private CanvasGroup group;        // CanvasGroup en el PANEL
    [SerializeField] private TMP_Text titleText;       // "¡Felicidades!"
    [SerializeField] private TMP_Text bodyText;        // "Has obtenido: <nombre>"
    [SerializeField] private Button replayButton;
    [SerializeField] private Button menuButton;

    [Header("Escenas")]
    [SerializeField] private string levelSceneName = "01_Nivel";
    [SerializeField] private string menuSceneName = "Menu Principal";

    float _prevTimeScale = 1f;
    bool _shown = false;

    void Awake()
    {
        // Auto-get del CanvasGroup en el panel si no está asignado
        if (!group) group = GetComponent<CanvasGroup>();

        // Panel oculto desde el inicio (sin desactivar el GameObject)
        SetVisible(false, immediate: true);

        if (replayButton) replayButton.onClick.AddListener(OnReplay);
        if (menuButton) menuButton.onClick.AddListener(OnMenu);
    }

    void OnEnable()
    {
        ChestPressedLogic.OnRewardRolled += OnRewardRolled;
    }

    void OnDisable()
    {
        ChestPressedLogic.OnRewardRolled -= OnRewardRolled;
    }

    // ========= Llamado cuando un cofre entrega recompensa =========
    void OnRewardRolled(ChestDropDB.DropDef item, Rarity rarity)
    {
        if (_shown) return; // mostramos solo la primera recompensa

        if (rarity == Rarity.Nada)
        {
            if (titleText) titleText.text = "¡HAZ ENCONTRADO EL COFRE! pero...";
            if (bodyText)
            {
                bodyText.text = "Lamentablemente estaba vacío, ¡mala suerte!";
                bodyText.color = new Color(0.8f, 0.8f, 0.8f); // gris suave
            }
        }
        else
        {
            if (titleText) titleText.text = "¡Felicidades!";
            if (bodyText)
            {
                bodyText.text = $"Has obtenido: {item.name}";
                bodyText.color = GetColorFor(rarity);
            }
        }

        Show();
    }

    void Show()
    {
        _shown = true;
        _prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;        // congelar el juego
        SetVisible(true);
        Debug.Log("[ChestRewardUI] Juego pausado por recompensa.");
    }

    void SetVisible(bool visible, bool immediate = false)
    {
        if (!group)
        {
            // fallback si no hay CanvasGroup: activar/desactivar objeto
            gameObject.SetActive(visible);
            return;
        }

        group.blocksRaycasts = visible;
        group.interactable = visible;
        group.alpha = visible ? 1f : 0f;
    }

    // ========= Botones =========
    void OnReplay()
    {
        Time.timeScale = 1f; // restaurar antes de cargar
        SceneManager.LoadScene(levelSceneName);
    }

    void OnMenu()
    {
        Time.timeScale = 1f; // restaurar antes de cargar
        SceneManager.LoadScene(menuSceneName);
    }

    // ========= Colores por rareza =========
    Color GetColorFor(Rarity r)
    {
        switch (r)
        {
            case Rarity.Normal: return Color.white;
            case Rarity.Rara: return Hex("#32CD32"); // Verde Lima
            case Rarity.Epica: return Hex("#A020F0"); // Púrpura
            case Rarity.Legendaria: return Hex("#FFD700"); // Dorado brillante
            default: return new Color(0.8f, 0.8f, 0.8f);
        }
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }
}

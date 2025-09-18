using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ChestRewardUI : MonoBehaviour
{
    [Header("Referencias UI (en el PANEL)")]
    [SerializeField] private CanvasGroup group;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button menuButton;

    [Header("Escenas")]
    [SerializeField] private string levelSceneName = "01_Nivel";
    [SerializeField] private string menuSceneName = "Menu Principal";

    [Header("Cursor")]
    [SerializeField] private bool manageCursor = true;

    CursorLockMode _prevLock;
    bool _prevVisible;
    float _prevTimeScale = 1f;
    bool _shown;

    void Awake()
    {
        if (!group) group = GetComponent<CanvasGroup>();
        SetVisible(false, true);

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

    void OnRewardRolled(ChestDropDB.DropDef item, ChestPressedLogic.Rarity rarity)
    {
        if (_shown) return;

        if (rarity == ChestPressedLogic.Rarity.Nada)
        {
            if (titleText) titleText.text = "¡HAZ ENCONTRADO EL COFRE! pero...";
            if (bodyText)
            {
                bodyText.text = "Lamentablemente estaba vacío, ¡mala suerte!";
                bodyText.color = new Color(0.8f, 0.8f, 0.8f);
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

    void SetVisible(bool visible, bool immediate = false)
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

    void OnReplay()
    {
        RestoreCursor();
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSceneName);
    }

    void OnMenu()
    {
        RestoreCursor();
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    void RestoreCursor()
    {
        if (!manageCursor) return;
        Cursor.lockState = _prevLock;
        Cursor.visible = _prevVisible;
    }

    Color GetColorFor(ChestPressedLogic.Rarity r)
    {
        switch (r)
        {
            case ChestPressedLogic.Rarity.Normal: return Color.white;
            case ChestPressedLogic.Rarity.Rara: return Hex("#32CD32");
            case ChestPressedLogic.Rarity.Epica: return Hex("#A020F0");
            case ChestPressedLogic.Rarity.Legendaria: return Hex("#FFD700");
            default: return new Color(0.8f, 0.8f, 0.8f);
        }
    }

    static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }
}

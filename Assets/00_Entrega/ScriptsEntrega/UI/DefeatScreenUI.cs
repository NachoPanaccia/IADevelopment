using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatScreenUI : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] string levelSceneName = "01_Nivel";
    [SerializeField] string menuSceneName = "Menu Principal";

    [Header("Botones (opcional, también podés asignar las funciones por OnClick)")]
    [SerializeField] Button retryButton;
    [SerializeField] Button menuButton;

    [Header("Cursor")]
    [SerializeField] bool manageCursor = true;

    CursorLockMode _prevLock;
    bool _prevVisible;

    void Awake()
    {
        Time.timeScale = 1f;
        if (manageCursor)
        {
            _prevLock = Cursor.lockState;
            _prevVisible = Cursor.visible;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (retryButton) retryButton.onClick.AddListener(OnRetry);
        if (menuButton) menuButton.onClick.AddListener(OnMainMenu);
    }

    public void OnRetry()
    {
        Load(levelSceneName);
    }

    public void OnMainMenu()
    {
        Load(menuSceneName);
    }

    void Load(string scene)
    {
        if (manageCursor)
        {
            Cursor.lockState = _prevLock;
            Cursor.visible = _prevVisible;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(scene);
    }
}

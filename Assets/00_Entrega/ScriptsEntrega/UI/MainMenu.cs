using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private string level1SceneName = "01_Nivel";
    private string creditos = "creditos";

    public void Play()
    {
        SceneManager.LoadScene(level1SceneName);
    }
    public void Creditos()
    {
        SceneManager.LoadScene(creditos);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

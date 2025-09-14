using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private string level1SceneName = "01_Nivel";

    public void Play()
    {
        SceneManager.LoadScene(level1SceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}

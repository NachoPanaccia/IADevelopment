using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField] float fadeDuration = 0.75f;
    CanvasGroup _cg;

    void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        _cg.alpha = 0f;
        _cg.interactable = false;
        _cg.blocksRaycasts = false;
    }

    public IEnumerator FadeOutCoroutine()
    {
        _cg.blocksRaycasts = true;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _cg.alpha = Mathf.Clamp01(t / fadeDuration);
            yield return null;
        }
        _cg.alpha = 1f;
    }

    public void FadeOutThenLoad(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        yield return FadeOutCoroutine();
        SceneManager.LoadScene(sceneName);
    }
}

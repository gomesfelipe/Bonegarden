using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class FaderBehaviour : MonoBehaviour
{

    public CanvasGroup Fader;

    private bool _isFirst;

    private static FaderBehaviour _instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _instance = this;

        if (!_isFirst)
        {
            _isFirst = true;
            Fader.alpha = 1;
            Invoke("LoadMainMenu", 2);
        }
    }

    void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public static FaderBehaviour GetInstance()
    {
        return _instance;
    }

    void SetVolume(float value)
    {
        AudioListener.volume = value;
    }

    float GetVolume()
    {
        return AudioListener.volume;
    }

    public void ShowBlackPanel(float duration, float delay)
    {
        Fader.DOFade(1, duration).SetDelay(delay);
    }

    public void HideBlackPanel(float duration, float delay)
    {
        Fader.DOFade(0, duration).SetDelay(delay);
    }

    public void OnLevelWasLoaded(int level)
    {
        HideBlackPanel(3, 2);
        DOTween.To(GetVolume, SetVolume, 1f, 2f);
    }

    public void Changelevel(string level)
    {
        DOTween.To(GetVolume, SetVolume, 0f, 1f);
        Fader.DOFade(1, 1).OnComplete(delegate
        {
            SceneManager.LoadScene(level);
        });
    }

    public void QuitGame()
    {
        DOTween.To(GetVolume, SetVolume, 0f, 1f);
        Fader.DOFade(1, 1).OnComplete(delegate
        {
            Application.Quit();
        });
    }
}

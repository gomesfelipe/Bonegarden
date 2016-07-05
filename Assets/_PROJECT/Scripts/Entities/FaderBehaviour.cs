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
            Changelevel("MainMenu");
        }
    }

    public static FaderBehaviour GetInstance()
    {
        return _instance;
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
        HideBlackPanel(1, 0);
    }

    public void Changelevel(string level)
    {
        Fader.DOFade(1, 1).OnComplete(delegate
        {
            SceneManager.LoadScene(level);
        });
    }
}

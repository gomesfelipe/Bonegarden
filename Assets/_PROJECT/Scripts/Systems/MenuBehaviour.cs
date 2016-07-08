using UnityEngine;
using System.Collections;

public class MenuBehaviour : MonoBehaviour
{

    public void StartGame()
    {
        FaderBehaviour.GetInstance().Changelevel("Zoo2");
    }

    public void QuitGame()
    {
        FaderBehaviour.GetInstance().QuitGame();
    }
}

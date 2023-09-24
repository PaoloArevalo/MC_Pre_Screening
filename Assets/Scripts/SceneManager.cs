using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void ChangeScene(int scnInd)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scnInd);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

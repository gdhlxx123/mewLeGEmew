using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class menu : MonoBehaviour
{
     public void quit()
    {
        Application.Quit();
    }
    public void enterGame()
    {
        SceneManager.LoadScene(1);
    }
}

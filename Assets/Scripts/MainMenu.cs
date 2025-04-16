using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void GotoStartMenu()
    {
        Debug.Log("스타트메뉴로 고");
        SceneManager.LoadScene("StartMenu");
    }
    public void RestartGame()
    {
        Debug.Log("다시하기 고");
        SceneManager.LoadScene("Main 2");
    }


    public void QuitGame()
    {
        Application.Quit();
    }


}

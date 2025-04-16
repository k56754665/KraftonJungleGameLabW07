using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene_Script : MonoBehaviour
{
    public void StartGame()
    {
        // 절대 씬 이름 바꾸지 마세요(사실 바꿔도 됨)
        SceneManager.LoadScene("Main");
    }
}

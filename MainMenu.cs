using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // this loads the scene with the index after the current scene's index. typically, this just loads the RaceTrack scene which is at index 1
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // This closes the application but only works in a built version of the game running from an executable and not in the Unity Editor
    public void QuitGame()
    {
        Application.Quit();
    }
}


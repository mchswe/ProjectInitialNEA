using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    // boolean which tracks if the game is currently paused
    public bool isPaused;

    // disables the pauseMenu object to ensure that it is hidden when the game starts.
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // checks if the player has pressed the "Esc" key
    // Update is called once per frame
    void Update()
    {
        // if "Esc" is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {    
            // resume the game if the game is currently paused
            if (isPaused)
            {
                ResumeGame();
            }

            // if the game isn't paused, pause the game
            else
            {
                PauseGame();
            }
        }
    }

    // this method freezes the game. It enables the pauseMenu object, freezes any physics and animations and updates the isPaused boolean to true to reflect the game's current state
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    // this method unfreezes the game. Disables the pauseMenu object, unfreezes physics and animations and updates the isPaused boolean to false to reflect the game's current state.
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // this is a method for the Menu button in the Pause Menu. Sets timescale to 1 so that the Main Menu won't be frozen. Loads the Main Menu scene
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    // closes the application. only works in built versions and not in the Unity Editor
    public void QuitGame()
    {
        Application.Quit();
    }
}


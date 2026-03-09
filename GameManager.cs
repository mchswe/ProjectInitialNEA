using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // creates a singleton and is set to public to allow the GameManager entity to be accessed by other scripts
    public static GameManager Instance;

    // Static allows the Pause Menu object to be triggered from anywhere
    public static PauseMenu PpauseMenu;

    // runs when the gameManager object is enabled and subscribes to the sceneLoaded event so code can be ran
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // runs when the gameManager object is disabled and unsubscribes from the sceneLoaded event.
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // this is called whenever a new scene is finished loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance == this) return;
        // looks for any objects with the pausemenu script in the scene
        PpauseMenu = Object.FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);
    }
    
    void Start()
    {    
        // checks to see if a pause menu object exists in the scene. If not, it will look for any object within the scene with the PauseMenu.cs script attached
        if (PpauseMenu == null)
        {
            PpauseMenu = Object.FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);
        }

        if (GameManager.Instance != null)
        {
            
        }
    }
    
    [Range(0.01f, 0.1f)] // changes the range of values that the slider is allowed to take, 0.01 to 0.1
    public float steeringSensitivity = 0.03f; // this is initialised at a default but is connected to a slider to allow user's to change the sensitivity to their preference
    
    // ensures that only one GameManager instance exists in a scene and ensures peristance across scenes.
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy(gameObject);
            this.enabled = false;
        }
    }

    // this updates the steeringSensitivity value to whatever the user has changed the value to with the slider
    public void UpdateSteeringSensitivity(float newSteerSensitivity)
    {
        if (Instance && Instance != this)
        {
            Instance.steeringSensitivity = newSteerSensitivity;
        }
        
        this.steeringSensitivity = newSteerSensitivity;
    }

    // lets user toggle the unit that their speed is displayed in. False = KMH. True = MPH.
    public bool useImperial = false;

    // is attached to a toggle box which the user can use to choose what unit is used for speed.
    public void ToggleUnit(bool toggle)
    {
        if (Instance && Instance != this)
        {
            Instance.useImperial = toggle;
        }
        
        this.useImperial = toggle;
    }


}


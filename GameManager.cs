using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static PauseMenu PpauseMenu;
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (Instance == this) return;
        
        PpauseMenu = Object.FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);
    }

    void Start()
    {
        if (PpauseMenu == null)
        {
            PpauseMenu = Object.FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include);
        }

        if (GameManager.Instance != null)
        {
            
        }
    }
    [Range(0.01f, 0.1f)]
    public float steeringSensitivity = 0.03f;
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

    public void UpdateSteeringSensitivity(float newSteerSensitivity)
    {
        if (Instance && Instance != this)
        {
            Instance.steeringSensitivity = newSteerSensitivity;
        }
        
        this.steeringSensitivity = newSteerSensitivity;
    }

    public bool useImperial = false;

    public void ToggleUnit(bool toggle)
    {
        if (Instance && Instance != this)
        {
            Instance.useImperial = toggle;
        }
        
        this.useImperial = toggle;
    }


}

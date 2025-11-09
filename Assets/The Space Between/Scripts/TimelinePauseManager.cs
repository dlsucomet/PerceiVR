using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class TimelinePauseManager : MonoBehaviour
{
    [Header("Timeline Control")]
    [Tooltip("The PlayableDirector component that controls your main timeline.")]
    public PlayableDirector timelineDirector;

    [Header("UI")]
    [Tooltip("The Canvas or Panel that contains your pause menu UI.")]
    public GameObject pauseMenuCanvas;

    [Header("Input Action")]
    [Tooltip("The Input Action for the pause button (e.g., Left Hand Menu Button).")]
    public InputActionReference pauseButtonAction;

    private bool isPaused = false;

    private void Awake()
    {
        pauseMenuCanvas?.SetActive(false);
    }

    private void OnEnable()
    {
        pauseButtonAction.action.Enable(); 
        pauseButtonAction.action.performed += TogglePause;
    }

    private void OnDisable()
    {
        pauseButtonAction.action.performed -= TogglePause;
    }

    private void TogglePause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            timelineDirector.Pause();
            pauseMenuCanvas.SetActive(true);
            Debug.Log("Timeline Paused");
        }
        else
        {
            timelineDirector.Play();
            pauseMenuCanvas.SetActive(false);
            Debug.Log("Timeline Resumed");
        }
    }

    public void QuitApplication()
    {
        Debug.Log("Quitting application...");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
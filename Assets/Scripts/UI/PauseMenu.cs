using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuUI;
    [SerializeField]
    private GameObject optionMenuUI; // Reference to the options menu
    [SerializeField]
    private GameObject lorePanelUI; // Reference to the lore panel
    private EventInstance _pauseSnapshot;

    public static bool IsPaused { get; private set; } = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            RuntimeManager.PlayOneShot("event:/UI/Pause");
            if (IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (optionMenuUI != null)
        {
            optionMenuUI.SetActive(false); // Hide the options menu
        }
        if (lorePanelUI != null)
        {
            lorePanelUI.SetActive(false); // Hide the lore panel
        }
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        IsPaused = false;
        if (_pauseSnapshot.isValid())
        {
            _pauseSnapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); // or IMMEDIATE
            _pauseSnapshot.release();
            _pauseSnapshot.clearHandle();
        }
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsPaused = true;
        _pauseSnapshot = RuntimeManager.CreateInstance("snapshot:/Pause");
        _pauseSnapshot.start();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Method to handle the Continue button click
    public void ContinueGame()
    {
        Resume();
    }

    // Method to handle the Title Screen button click
    public void GoToTitleScreen()
    {
        Time.timeScale = 1f;

        // 1) Stop the ambience loop
        if (AmbientController.Instance != null)
            AmbientController.Instance.StopAmbience(false);  // or true if you want a fade-out

        // 2) Stop the pause snapshot (if still running)
        if (_pauseSnapshot.isValid())
        {
            _pauseSnapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _pauseSnapshot.release();
            _pauseSnapshot.clearHandle();
        }

        // 3) (Optional) stop other loops one by one here…

        // 4) Finally load your menu
        SceneManager.LoadScene("StartMenu");
    }


    // Method to handle the Options button click
    public void OpenOptions()
    {
        if (optionMenuUI.activeSelf)
        {
            optionMenuUI.SetActive(false);
        }
        else
        {
            optionMenuUI.SetActive(true);
        }
    }

    // Method to handle the Lore button click
    public void OpenLorePanel()
    {
        if (lorePanelUI.activeSelf)
        {
            lorePanelUI.SetActive(false);
        }
        else
        {
            lorePanelUI.SetActive(true);
        }
    }
}
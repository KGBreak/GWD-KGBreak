using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuUI;
    [SerializeField]
    private GameObject optionMenuUI; // Reference to the options menu
    [SerializeField]
    private GameObject lorePanelUI; // Reference to the lore panel

    public static bool IsPaused { get; private set; } = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        IsPaused = true;
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
        Time.timeScale = 1f; // Ensure the game is not paused
        SceneManager.LoadScene("StartMenu"); // Load the title screen scene
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
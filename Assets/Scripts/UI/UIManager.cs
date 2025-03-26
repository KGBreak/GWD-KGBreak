using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject optionMenu;
    // We should change this to index numbers of the scenes when we have them
    // Method to handle the Exit button
    public void ExitGame()
    {
        Application.Quit();
    }

    // Method to handle the Main Menu button
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }

    // Method to handle the Start Game button
    public void StartGame()
    {
        SceneManager.LoadScene("Asger_Greybox");
    }

    // Method to handle the Continue Game button
    public void ContinueGame()
    {
        // Logic to continue the game/ closing the menu
        Debug.Log("Continue Game");
    }

    // Method to handle the Options button
    public void OpenOptions()
    {
        if (optionMenu == null) return;

        if (optionMenu.activeSelf)
        {
            optionMenu.SetActive(false);
        }
        else optionMenu.SetActive(true);
    }
}
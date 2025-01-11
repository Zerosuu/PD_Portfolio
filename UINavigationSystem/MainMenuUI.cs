using UnityEngine;
using UnityEngine.SceneManagement;

// Subclass of BaseMenuUI used for UI Navigation and various functionality for the main menu
public class MainMenuUI : BaseMenuUI
{
    // Update function listening to button inputs to determine if program should go up a submenu
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwapLayer();
        }
    }

    // Function staring the game anew
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}

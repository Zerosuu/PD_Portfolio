using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : BaseMenuUI
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SwapLayer();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}